import io
import re
import cv2
import torch
import easyocr
import numpy as np
from PIL import Image
from pyzbar.pyzbar import decode
from transformers import pipeline
from dataclasses import dataclass
from typing import Optional, List, Dict
from fastapi import FastAPI, File, UploadFile

app = FastAPI()

@dataclass
class ScanResult:
    type: str  # 'barcode', 'qrcode', 'image', 'text
    price: Optional[float]
    confidence: float
    additional_info: Dict
    text_found: List[str]

class PriceRecognitionSystem:
    def __init__(self):
        # Initialize various models and readers
        self.barcode_reader = cv2.barcode.BarcodeDetector()
        self.ocr_reader = easyocr.Reader(['en'])
        
        # Initialize vision transformer for sneaker recognition
        self.image_classifier = pipeline(
            "image-classification",
            model="microsoft/dit-base-finetuned-rvlcdip",
            device=0 if torch.cuda.is_available() else -1
        )
        
        # Price extraction patterns
        self.price_pattern = re.compile(r'\$?\d+\.?\d*')
        
        # Load sneaker price reference database (simplified example)
        self.sneaker_price_ranges = {
            "athletic": (60, 120),
            "running": (80, 160),
            "basketball": (90, 200),
            "luxury": (200, 500),
            "sneaker": (50, 700)
        }

    def preprocess_image(self, image_bytes):
        # Convert bytes to numpy array for OpenCV processing
        nparr = np.frombuffer(image_bytes, np.uint8)
        image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        return image

    def detect_and_decode_barcode(self, image):
        # Try to detect and decode barcodes
        retval, decoded_info, decoded_type, points = self.barcode_reader.detectAndDecode(image)
        
        if retval:
            return {
                'decoded_info': decoded_info,
                'type': decoded_type,
                'points': points
            }
        return None

    def decode_qr_code(self, image):
        # Use pyzbar to decode QR codes
        decoded_objects = decode(image)
        if decoded_objects:
            return [{
                'type': obj.type,
                'data': obj.data.decode('utf-8'),
                'rect': obj.rect
            } for obj in decoded_objects]
        return None

    def extract_price_from_text(self, texts):
        prices = []
        for text in texts:
            matches = self.price_pattern.findall(text)
            prices.extend([float(price.replace('$', '')) for price in matches])
        return prices if prices else None

    def estimate_sneaker_price(self, image):
        # Convert OpenCV image to PIL for transformer
        image_pil = Image.fromarray(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))
        
        # Get image classification results
        predictions = self.image_classifier(image_pil)
        
        # Basic sneaker type classification (simplified)
        sneaker_type = "athletic"  # Default
        confidence = 0.5
        
        for pred in predictions:
            if "shoe" in pred['label'].lower() or "sneaker" in pred['label'].lower():
                confidence = pred['score']
                if "running" in pred['label'].lower():
                    sneaker_type = "running"
                elif "basketball" in pred['label'].lower():
                    sneaker_type = "basketball"
                elif "luxury" in pred['label'].lower():
                    sneaker_type = "luxury"
                elif "sneaker" in pred['label'].lower():
                    sneaker_type = "sneaker"
                break
        
        price_range = self.sneaker_price_ranges[sneaker_type]
        estimated_price = (price_range[0] + price_range[1]) / 2
        
        return estimated_price, confidence

    async def process_image(self, file_contents: bytes) -> ScanResult:
        image = self.preprocess_image(file_contents)
        
        # 1. Try barcode first
        barcode_result = self.detect_and_decode_barcode(image)
        if barcode_result:
            return ScanResult(
                type='barcode',
                price=self.extract_price_from_text([str(barcode_result['decoded_info'])]),
                confidence=0.95,
                additional_info=barcode_result,
                text_found=[str(info) for info in barcode_result['decoded_info']]
            )
        
        # 2. Try QR code
        qr_result = self.decode_qr_code(image)
        if qr_result:
            texts = [obj['data'] for obj in qr_result]
            return ScanResult(
                type='qrcode',
                price=self.extract_price_from_text(texts),
                confidence=0.90,
                additional_info={'qr_data': qr_result},
                text_found=texts
            )
        
        # 3. Process as general image
        # First try OCR to find any visible prices
        ocr_result = self.ocr_reader.readtext(image)
        texts = [text[1] for text in ocr_result]
        price_from_ocr = self.extract_price_from_text(texts)
        
        # Then try image classification for sneaker price estimation
        estimated_price, confidence = self.estimate_sneaker_price(image)
        
        # Prefer OCR price if found, otherwise use estimated price
        final_price = price_from_ocr[0] if price_from_ocr else estimated_price
        
        return ScanResult(
            type='image',
            price=final_price,
            confidence=confidence,
            additional_info={
                'ocr_found_price': bool(price_from_ocr),
                'estimated_price': estimated_price
            },
            text_found=texts
        )

# Initialize the system
price_system = PriceRecognitionSystem()

@app.post("/analyze")
async def analyze_image(file: UploadFile = File(...)):
    contents = await file.read()
    result = await price_system.process_image(contents)
    
    return {
        "type": result.type,
        "price": result.price,
        "confidence": result.confidence,
        "additional_info": result.additional_info,
        "text_found": result.text_found
    }

