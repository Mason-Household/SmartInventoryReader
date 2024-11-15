export interface ScanResult {
	type: 'barcode' | 'qrcode' | 'image'| 'text';
	price: number | null;
	confidence: number;
	additional_info: Record<string, any>;
	text_found: string[];
}
  
