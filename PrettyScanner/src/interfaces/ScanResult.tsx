import { MenuItem } from "./MenuItem";

export interface ScanResult {
    name: string;
    actual_price: number | null;
    suggested_price: number | null;
    stock_quantity: number | null;
    low_stock_threshold: number | null;
    tag_names: string[];
	type: ScanType;
	price: number | null;
	confidence: number;
	additional_info: {
		menuItems?: MenuItem[];
		currency?: string;
		venue_name?: string;
		menu_type?: 'food' | 'drinks' | 'combined';
		detected_language?: string;
		[key: string]: any;
	};
	text_found: string[];
}

export type ScanType = 'barcode' | 'qrcode' | 'image' | 'text';
  
