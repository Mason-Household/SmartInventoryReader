export interface ScanResult {
  type: string;
  name: string | null;
  suggested_price: number | null;
  actual_price: number;
  stock_quantity: number;
  low_stock_threshold: number | null;
  barcode: string | null;
  category_id: number | null;
  tag_names: string[];
  notes: string | null;
  confidence: number;
  additional_info: {
    predictions?: Array<{
      label: string;
      score: number;
    }>;
    ocr_found_price: boolean;
  };
  text_found: string[];
}

export const mapScanResultToInventoryItem = (result: ScanResult) => {
  return {
    name: result.name || '',
    suggestedPrice: result.suggested_price,
    actualPrice: result.actual_price,
    stockQuantity: result.stock_quantity,
    lowStockThreshold: result.low_stock_threshold,
    barcode: result.barcode,
    categoryId: result.category_id,
    tagNames: result.tag_names,
    notes: result.notes || undefined,
    confidence: result.confidence,
    source: 'scan' as const
  };
};
