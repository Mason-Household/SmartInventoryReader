import React from 'react';
import { Tag, DollarSign } from 'lucide-react';
import { InventoryItem } from '@/interfaces/InventoryItem';
import { Chip, InputAdornment, TextField } from '@mui/material';



interface ResultActionsProps {
  itemData: InventoryItem;
  handleRemoveTag: (tag: string) => void;
  handleAddTag: () => void;
  newTag: string;
  setNewTag: (tag: string) => void;
  handlePriceChange: (price: number) => void;
}

const ResultActions: React.FC<ResultActionsProps> = ({
  itemData,
  handleRemoveTag,
  handleAddTag,
  newTag,
  setNewTag,
  handlePriceChange,
}) => {
  return (
    <div>
      <h2>Item Details</h2>
      <TextField
        label="Name"
        value={itemData.name || ''}
        disabled
        size="small"
      />
      <TextField
        label="Suggested Price"
        value={itemData.suggestedPrice || ''}
        disabled
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <DollarSign size={20} />
            </InputAdornment>
          ),
        }}
        size="small"
      />
      <TextField
        label="Actual Price"
        type="number"
        value={itemData.actualPrice}
        onChange={(e) => handlePriceChange(Number(e.target.value))}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <DollarSign size={20} />
            </InputAdornment>
          ),
        }}
        size="small"
      />
      <TextField
        label="Stock Quantity"
        value={itemData.stockQuantity}
        disabled
        size="small"
      />
      <TextField
        label="Low Stock Threshold"
        value={itemData.lowStockThreshold || ''}
        disabled
        size="small"
      />
      <TextField
        label="Barcode"
        value={itemData.barcode || ''}
        disabled
        size="small"
      />
      <TextField
        label="Category ID"
        value={itemData.categoryId || ''}
        disabled
        size="small"
      />
      <TextField
        label="Notes"
        value={itemData.notes || ''}
        disabled
        size="small"
      />
      <TextField
        label="Confidence"
        value={itemData.confidence}
        disabled
        size="small"
      />
      <TextField
        label="Text Found"
        value={itemData.textFound.join(', ')}
        disabled
        size="small"
      />
      {itemData.additionalInfo?.predictions && (
        <div>
          <h3>Predictions</h3>
          {itemData.additionalInfo.predictions.map((prediction, index) => (
            <TextField
              key={index}
              label={prediction.label}
              value={prediction.score}
              disabled
              size="small"
            />
          ))}
        </div>
      )}
      <TextField
        label="OCR Found Price"
        value={itemData.additionalInfo?.ocrFoundPrice ? 'Yes' : 'No'}
        disabled
        size="small"
      />
      <h3>Tags</h3>
      {itemData.tagNames?.filter((tag): tag is string => typeof tag === 'string').map((tag: string) => (
        <Chip
          key={tag}
          label={tag}
          onDelete={() => handleRemoveTag(tag)}
          size="small"
        />
      ))}
      <TextField
        size="small"
        placeholder="Add tag"
        value={newTag}
        onChange={(e) => setNewTag(e.target.value)}
        onKeyPress={(e) => {
          if (e.key === 'Enter') {
            e.preventDefault();
            handleAddTag();
          }
        }}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <Tag size={16} />
            </InputAdornment>
          ),
        }}
      />
    </div>
  );
};

export default ResultActions;