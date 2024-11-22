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
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@dataclass
class ScanResult:
    type: str  # 'barcode', 'qrcode', 'image', 'text'
    name: Optional[str]
    suggested_price: Optional[float]
    actual_price: float
    stock_quantity: int
    low_stock_threshold: Optional[int]
    barcode: Optional[str]
    category_id: Optional[int]
    tag_names: List[str]
    notes: Optional[str]
    confidence: float
    additional_info: Dict
    text_found: List[str]

class PriceRecognitionSystem:
    def __init__(self):
        # Initialize various models and readers
        self.barcode_reader = cv2.barcode.BarcodeDetector()
        self.ocr_reader = easyocr.Reader(['en'])
        
        # Initialize vision transformer for product recognition
        self.image_classifier = pipeline(
            "image-classification",
            model="microsoft/dit-base-finetuned-rvlcdip",
            device=0 if torch.cuda.is_available() else -1
        )
        
        # Price extraction patterns
        self.price_pattern = re.compile(r'\$?\d+\.?\d*')
        
        # Category mappings (simplified example)
        self.category_mappings = {
            "shoe": 1,
            "sneaker": 1,
            "clothing": 2,
            "electronics": 3,
            "food": 4,
            "beverage": 5,
            "other": 6
        }
        
        # Default stock thresholds by category
        self.stock_thresholds = {
            1: 5,  # shoes
            2: 10, # clothing
            3: 3,  # electronics
            4: 15, # food
            5: 20, # beverage
            6: 5   # other
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

    def detect_category(self, predictions):
        for pred in predictions:
            label = pred['label'].lower()
            for category_name in self.category_mappings:
                if category_name in label:
                    return self.category_mappings[category_name]
        return self.category_mappings['other']

    def extract_product_name(self, texts):
        # Simple heuristic: take the longest text that's not a price
        valid_texts = [text for text in texts if not self.price_pattern.match(text)]
        return max(valid_texts, key=len, default=None) if valid_texts else None

    def suggest_stock_quantity(self, category_id):
        # Suggest initial stock based on category
        base_quantity = 10
        if category_id in [4, 5]:  # Food and beverages
            base_quantity = 20
        elif category_id in [1, 2]:  # Shoes and clothing
            base_quantity = 5
        return base_quantity

    async def process_image(self, file_contents: bytes) -> ScanResult:
        image = self.preprocess_image(file_contents)
        
        # Initialize default values
        scan_type = 'image'
        confidence = 0.5
        barcode_value = None
        prices = None
        texts = []
        
        # 1. Try barcode first
        barcode_result = self.detect_and_decode_barcode(image)
        if barcode_result:
            scan_type = 'barcode'
            confidence = 0.95
            barcode_value = str(barcode_result['decoded_info'][0])
            texts = [str(info) for info in barcode_result['decoded_info']]
        
        # 2. Try QR code if no barcode
        if not barcode_value:
            qr_result = self.decode_qr_code(image)
            if qr_result:
                scan_type = 'qrcode'
                confidence = 0.90
                texts = [obj['data'] for obj in qr_result]
                barcode_value = texts[0] if texts else None
        
        # 3. Process with OCR
        ocr_result = self.ocr_reader.readtext(image)
        texts.extend([text[1] for text in ocr_result])
        
        # Extract prices from all text sources
        prices = self.extract_price_from_text(texts)
        
        # Get image classification for category detection
        predictions = self.image_classifier(Image.fromarray(cv2.cvtColor(image, cv2.COLOR_BGR2RGB)))
        category_id = self.detect_category(predictions)
        
        # Extract or generate other required fields
        product_name = self.extract_product_name(texts)
        suggested_price = prices[0] if prices and len(prices) > 1 else None
        actual_price = prices[-1] if prices else 0.0
        stock_quantity = self.suggest_stock_quantity(category_id)
        low_stock_threshold = self.stock_thresholds.get(category_id)
        
        # Extract potential tags from text and predictions
        tag_names = set()
        for pred in predictions[:2]:  # Use top 2 predictions as tags
            tag_names.add(pred['label'].lower())
        
        return ScanResult(
            type=scan_type,
            name=product_name,
            suggested_price=suggested_price,
            actual_price=actual_price,
            stock_quantity=stock_quantity,
            low_stock_threshold=low_stock_threshold,
            barcode=barcode_value,
            category_id=category_id,
            tag_names=list(tag_names),
            notes=None,
            confidence=confidence,
            additional_info={
                'predictions': predictions,
                'ocr_found_price': bool(prices)
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
        "name": result.name,
        "suggested_price": result.suggested_price,
        "actual_price": result.actual_price,
        "stock_quantity": result.stock_quantity,
        "low_stock_threshold": result.low_stock_threshold,
        "barcode": result.barcode,
        "category_id": result.category_id,
        "tag_names": result.tag_names,
        "notes": result.notes,
        "confidence": result.confidence,
        "additional_info": result.additional_info,
        "text_found": result.text_found
    }
