export interface InventoryItem {
	id?: string;
	name: string;
	suggestedPrice: number | null;
	actualPrice: number;
	type: 'product' | 'menu_item';
	category?: string;
	confidence: number;
	dateAdded: string;
	source: 'scan' | 'manual';
	notes?: string;
	tags?: string[];
}
