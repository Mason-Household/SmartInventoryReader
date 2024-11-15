import React, { useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  InputAdornment,
  Chip,
  Stack,
  Alert,
} from '@mui/material';
import { motion } from 'framer-motion';
import {
  DollarSign,
  Tag,
  Archive,
  Trash2,
} from 'lucide-react';
import { ScanResult } from '../../interfaces/ScanResult';

interface InventoryItem {
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

interface ResultActionsProps {
  scanResult: ScanResult;
  onSave: (item: InventoryItem) => Promise<void>;
  onDiscard: () => void;
}

const ResultActions: React.FC<ResultActionsProps> = ({
  scanResult,
  onSave,
  onDiscard,
}) => {
  const [isSaving, setIsSaving] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const [itemData, setItemData] = useState<Partial<InventoryItem>>({
    suggestedPrice: scanResult.price,
    actualPrice: scanResult.price || 0,
    type: 'product',
    confidence: scanResult.confidence,
    dateAdded: new Date().toISOString(),
    source: 'scan',
    tags: [],
  });

  const [newTag, setNewTag] = useState('');

  const handlePriceChange = (value: number) => {
    setItemData(prev => ({
      ...prev,
      actualPrice: value,
    }));
  };

  const handleAddTag = () => {
    if (newTag && !itemData.tags?.includes(newTag)) {
      setItemData(prev => ({
        ...prev,
        tags: [...(prev.tags || []), newTag],
      }));
      setNewTag('');
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setItemData(prev => ({
      ...prev,
      tags: prev.tags?.filter(tag => tag !== tagToRemove),
    }));
  };

  const handleSave = async () => {
    try {
      setIsSaving(true);
      setError(null);
      
      // Validate required fields
      if (!itemData.actualPrice) {
        throw new Error('Please set an actual price');
      }

      await onSave(itemData as InventoryItem);
      setShowConfirm(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save item');
    } finally {
      setIsSaving(false);
    }
  };

  const priceDifference = (itemData.actualPrice || 0) - (itemData.suggestedPrice || 0);

  return (
    <Card
      component={motion.div}
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -20 }}
      sx={{ mb: 3 }}
    >
      <CardContent>
        <Box sx={{ mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Add to Inventory
          </Typography>
          <Typography color="text.secondary" variant="body2">
            Review and adjust details before saving to your inventory
          </Typography>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        <Stack spacing={3}>
          {/* Price Section */}
          <Box>
            <Typography variant="subtitle2" gutterBottom>
              Price Information
            </Typography>
            <Box sx={{ 
              display: 'flex', 
              gap: 2, 
              alignItems: 'flex-start',
              flexWrap: 'wrap' 
            }}>
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
                error={!itemData.actualPrice}
                helperText={!itemData.actualPrice ? 'Required' : ''}
              />
              {itemData.actualPrice !== itemData.suggestedPrice && (
                <Chip
                  label={`${priceDifference > 0 ? '+' : ''}$${priceDifference.toFixed(2)} difference`}
                  color={priceDifference > 0 ? 'success' : 'error'}
                  size="small"
                  sx={{ mt: 1 }}
                />
              )}
            </Box>
          </Box>

          {/* Category and Tags */}
          <Box>
            <Typography variant="subtitle2" gutterBottom>
              Categories & Tags
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
              <FormControl size="small" sx={{ minWidth: 200 }}>
                <InputLabel>Category</InputLabel>
                <Select
                  value={itemData.category || ''}
                  label="Category"
                  onChange={(e) => setItemData(prev => ({
                    ...prev,
                    category: e.target.value,
                  }))}
                >
                  <MenuItem value="sneakers">Sneakers</MenuItem>
                  <MenuItem value="clothing">Clothing</MenuItem>
                  <MenuItem value="accessories">Accessories</MenuItem>
                  <MenuItem value="other">Other</MenuItem>
                </Select>
              </FormControl>
            </Box>
            
            <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
              {itemData.tags?.map((tag) => (
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
            </Box>
          </Box>

          {/* Notes */}
          <TextField
            label="Notes"
            multiline
            rows={2}
            value={itemData.notes || ''}
            onChange={(e) => setItemData(prev => ({
              ...prev,
              notes: e.target.value,
            }))}
          />

          {/* Action Buttons */}
          <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
            <Button
              variant="outlined"
              color="error"
              startIcon={<Trash2 />}
              onClick={onDiscard}
            >
              Discard
            </Button>
            <Button
              variant="contained"
              startIcon={<Archive />}
              onClick={handleSave}
              disabled={isSaving || !itemData.actualPrice}
            >
              Save to Inventory
            </Button>
          </Box>
        </Stack>
      </CardContent>

      {/* Confirmation Dialog */}
      <Dialog
        open={showConfirm}
        onClose={() => setShowConfirm(false)}
      >
        <DialogTitle>Item Saved Successfully</DialogTitle>
        <DialogContent>
          <Typography>
            The item has been added to your inventory.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowConfirm(false)}>
            Close
          </Button>
          <Button
            variant="contained"
            onClick={() => {
              setShowConfirm(false);
              // Navigate to inventory or wherever needed
            }}
          >
            View Inventory
          </Button>
        </DialogActions>
      </Dialog>
    </Card>
  );
};

export default ResultActions;