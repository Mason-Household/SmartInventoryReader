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
from torchvision.models import resnet50
import torchvision.transforms as transforms
from fastapi import FastAPI, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost"],
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
    additional_info: Dict[str, Optional[bool]]
    text_found: List[str]

class ImageProcessor:
    def __init__(self):
        self.ocr_reader = easyocr.Reader(['en'])
        self.image_classifier = pipeline('image-classification', model='resnet50')
        self.transform = transforms.Compose([
            transforms.Resize((224, 224)),
            transforms.ToTensor(),
            transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]),
        ])
        self.model = resnet50(pretrained=True)
        self.model.eval()
        self.price_pattern = re.compile(r'\$\d+(\.\d{2})?')
        self.stock_thresholds = {1: 5, 2: 5, 4: 20, 5: 20}

    def preprocess_image(self, file_contents: bytes) -> np.ndarray:
        image = np.array(Image.open(io.BytesIO(file_contents)))
        return cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

    def detect_and_decode_barcode(self, image: np.ndarray) -> Optional[Dict[str, List[str]]]:
        decoded_objects = decode(image)
        if decoded_objects:
            return {'decoded_info': [obj.data.decode('utf-8') for obj in decoded_objects]}
        return None

    def decode_qr_code(self, image: np.ndarray) -> Optional[List[Dict[str, str]]]:
        decoded_objects = decode(image)
        if decoded_objects:
            return [{'data': obj.data.decode('utf-8')} for obj in decoded_objects if obj.type == 'QRCODE']
        return None

    def extract_price_from_text(self, texts: List[str]) -> List[float]:
        return [float(price[1:]) for text in texts for price in self.price_pattern.findall(text)]

    def extract_product_name(self, texts: List[str]) -> Optional[str]:
        valid_texts = [text for text in texts if not self.price_pattern.match(text)]
        return max(valid_texts, key=len, default=None) if valid_texts else None

    def suggest_stock_quantity(self, category_id: int) -> int:
        base_quantity = 10
        if category_id in [4, 5]:  # Food and beverages
            base_quantity = 20
        elif category_id in [1, 2]:  # Shoes and clothing
            base_quantity = 5
        return base_quantity

    def detect_category(self, predictions: List[Dict[str, str]]) -> int:
        # Implement this function to map predictions to category IDs
        # For simplicity, let's assume the first prediction is the category
        category_map = {
            'shoe': 1,
            'clothing': 2,
            'food': 4,
            'beverage': 5
        }
        for pred in predictions:
            if pred['label'].lower() in category_map:
                return category_map[pred['label'].lower()]
        return 0  # Default category ID if no match found

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
        
        # 4. Guess details about shoes or clothing apparel
        if not barcode_value and not qr_result:
            pil_image = Image.fromarray(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))
            input_tensor = self.transform(pil_image).unsqueeze(0)
            with torch.no_grad():
                output = self.model(input_tensor)
            _, predicted = torch.max(output, 1)
            category_id = predicted.item()
            category_name = self.get_category_name(category_id)  # Implement this function to map category_id to category_name
            texts.append(category_name)
        
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

    def get_category_name(self, category_id: int) -> str:
        category_map = {
            1: 'shoe',
            2: 'clothing',
            4: 'food',
            5: 'beverage'
        }
        return category_map.get(category_id, 'unknown')

image_processor = ImageProcessor()

@app.post("/analyze/")
async def analyze_image(file: UploadFile = File(...)):
    contents = await file.read()
    result = await image_processor.process_image(contents)
    
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
