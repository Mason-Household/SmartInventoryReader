import React from 'react';
import { CardContent, Typography, Box, IconButton, LinearProgress, List, ListItem, ListItemText, Chip, Stack, Card } from '@mui/material';
import { motion } from 'framer-motion';
import { Copy, Share } from 'lucide-react';
import { ScanResult } from '../../interfaces/ScanResult'

const MotionCard = motion(React.forwardRef<HTMLDivElement, any>((props, ref) => <Card {...props} ref={ref} component="div" />));

interface ResultCardProps {
  result: ScanResult;
  copyResult: () => void;
  shareResult: () => void;
  theme: any;
}

const ResultCard: React.FC<ResultCardProps> = ({ result, copyResult, shareResult, theme }) => (
  <MotionCard initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} transition={{ duration: 0.3 }} sx={{ mb: 3 }}>
    <CardContent>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h6">Scan Results</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <IconButton onClick={copyResult} size="small">
            <Copy size={20} />
          </IconButton>
          {'share' in navigator && (
            <IconButton onClick={shareResult} size="small">
              <Share size={20} />
            </IconButton>
          )}
        </Box>
      </Box>

      <Box sx={{ mt: 2 }}>
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <Typography variant="subtitle2" color="textSecondary">Product Name</Typography>
          <Typography variant="h6" sx={{ mb: 2 }}>{result.name || 'Not detected'}</Typography>
        </motion.div>

        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
          <Typography variant="subtitle2" color="textSecondary">Price Information</Typography>
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <Box>
              <Typography variant="caption" color="textSecondary">Actual Price</Typography>
              <Typography variant="h5" color="primary">${result.actual_price?.toFixed(2)}</Typography>
            </Box>
            {result.suggested_price && (
              <Box>
                <Typography variant="caption" color="textSecondary">Suggested Price</Typography>
                <Typography variant="h5" color="secondary">${result.suggested_price.toFixed(2)}</Typography>
              </Box>
            )}
          </Box>
        </motion.div>

        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.4 }}>
          <Typography variant="subtitle2" color="textSecondary">Stock Information</Typography>
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <Box>
              <Typography variant="caption" color="textSecondary">Quantity</Typography>
              <Typography variant="h6">{result.stock_quantity}</Typography>
            </Box>
            {result.low_stock_threshold && (
              <Box>
                <Typography variant="caption" color="textSecondary">Low Stock Alert</Typography>
                <Typography variant="h6">{result.low_stock_threshold}</Typography>
              </Box>
            )}
          </Box>
        </motion.div>

        {result.tag_names.length > 0 && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.5 }}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>Tags</Typography>
            <Stack direction="row" spacing={1} sx={{ mb: 2, flexWrap: 'wrap', gap: 1 }}>
              {result.tag_names.map((tag: string | number | boolean | React.ReactElement<any, string | React.JSXElementConstructor<any>> | Iterable<React.ReactNode> | React.ReactPortal | null | undefined, index: React.Key | null | undefined) => (
                <Chip key={index} label={tag} size="small" />
              ))}
            </Stack>
          </motion.div>
        )}

        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.6 }}>
          <Typography variant="subtitle2" color="textSecondary" gutterBottom>Confidence</Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
            <Box sx={{ flex: 1 }}>
              <LinearProgress
                variant="determinate"
                value={result.confidence * 100}
                sx={{
                  height: 8,
                  borderRadius: 4,
                  backgroundColor: theme.palette.action.hover,
                  '& .MuiLinearProgress-bar': {
                    borderRadius: 4,
                  },
                }}
              />
            </Box>
            <Typography variant="body2" color="textSecondary">{(result.confidence * 100).toFixed(1)}%</Typography>
          </Box>
        </motion.div>

        {result.text_found.length > 0 && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.7 }}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>Text Found</Typography>
            <List dense>
              {result.text_found.map((text: string | number | boolean | React.ReactElement<any, string | React.JSXElementConstructor<any>> | Iterable<React.ReactNode> | React.ReactPortal | null | undefined, index: React.Key | null | undefined) => (
                <ListItem key={index}>
                  <ListItemText
                    primary={text}
                    sx={{
                      '& .MuiListItemText-primary': {
                        fontFamily: 'monospace',
                        fontSize: '0.9rem',
                      },
                    }}
                  />
                </ListItem>
              ))}
            </List>
          </motion.div>
        )}
      </Box>
    </CardContent>
  </MotionCard>
);

export default ResultCard;