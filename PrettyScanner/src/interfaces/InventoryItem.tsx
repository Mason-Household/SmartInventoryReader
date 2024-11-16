export interface InventoryItem {
	id?: number;
	name: string;
	suggestedPrice: number | null;
	actualPrice: number;
	stockQuantity: number;
	lowStockThreshold: number | null;
	barcode: string | null;
	categoryId: number | null;
	tagNames: string[];
	notes?: string;
	confidence: number;
	dateAdded?: string;
	source: 'scan' | 'manual';
	organizationId?: number;
}
