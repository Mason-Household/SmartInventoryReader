export interface InventoryItem {
	id?: number;
	type: 'product' | 'service' | 'other' | 'menu-item' | 'menu' | 'category' | 'text';
	dateAdded?: string;
	organizationId?: number;
	name: string;
	suggestedPrice: number | null;
	actualPrice: number;
	stockQuantity: number;
	lowStockThreshold: number | null;
	barcode: string | null;
	categoryId: number | null;
	tagNames: string[];
	notes: string | null;
	confidence: number;
	source: 'scan';
	additionalInfo?: {
	  predictions?: Array<{
		label: string;
		score: number;
	  }>;
	  ocrFoundPrice: boolean;
	};
	textFound: string[];
  }
