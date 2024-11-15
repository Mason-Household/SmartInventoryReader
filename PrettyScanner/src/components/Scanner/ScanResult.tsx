export interface ScanResult {
  type: string;
  price: number;
  confidence: number;
  text_found: string[];
}
