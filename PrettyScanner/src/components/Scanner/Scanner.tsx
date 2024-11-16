import React, { useRef, useState, useCallback } from 'react';
import Webcam from 'react-webcam';
import {
  Box,
  Button,
  Card,
  CardContent,
  IconButton,
  LinearProgress,
  List,
  ListItem,
  ListItemText,
  Typography,
  useTheme,
  Zoom,
  Alert,
  Snackbar,
  CircularProgress,
  useMediaQuery,
  Dialog,
  DialogContent,
  Chip,
  Stack,
} from '@mui/material';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Camera,
  Upload,
  X as CloseIcon,
  Copy,
  Share,
} from 'lucide-react';
import { ScanResult } from './ScanResult';

const MotionCard = motion(React.forwardRef<HTMLDivElement, any>((props, ref) => <Card {...props} ref={ref} component="div" />));

const Scanner: React.FC = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [isCapturing, setIsCapturing] = useState(false);
  const [result, setResult] = useState<ScanResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showSnackbar, setShowSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const webcamRef = useRef<Webcam | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

  const handleError = (error: Error) => {
    console.error('Error:', error);
    setError(error.message);
    setShowSnackbar(true);
    setSnackbarMessage('An error occurred while processing the image');
    setLoading(false);
  };

  const uploadImage = async (file: File | Blob) => {
    setLoading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch(`${BASE_URL}/analyze`, {
        method: 'POST',
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      setResult(data);
      setShowSnackbar(true);
      setSnackbarMessage('Image processed successfully!');
    } catch (err) {
      handleError(err as Error);
    } finally {
      setLoading(false);
    }
  };

  const handleCapture = useCallback(async () => {
    if (webcamRef.current) {
      try {
        const imageSrc = webcamRef.current.getScreenshot();
        if (imageSrc) {
          const base64Response = await fetch(imageSrc);
          const blob = await base64Response.blob();
          await uploadImage(blob);
        }
      } catch (err) {
        handleError(err as Error);
      }
    }
  }, [webcamRef]);

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      uploadImage(file);
    }
  };

  const toggleCamera = () => {
    setIsCapturing(prev => !prev);
    setResult(null);
    setError(null);
  };

  const copyResult = () => {
    if (result) {
      navigator.clipboard.writeText(JSON.stringify(result, null, 2));
      setShowSnackbar(true);
      setSnackbarMessage('Results copied to clipboard!');
    }
  };

  const shareResult = async () => {
    if (result && navigator.share) {
      try {
        await navigator.share({
          title: 'Scan Result',
          text: JSON.stringify(result, null, 2),
        });
      } catch (err) {
        handleError(err as Error);
      }
    }
  };

  return (
    <Box sx={{ width: '100%', maxWidth: 600, mx: 'auto' }}>
      <AnimatePresence mode="wait">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -20 }}
          transition={{ duration: 0.2 }}
        >
          <Box sx={{ display: 'flex', gap: 2, mb: 3, justifyContent: 'center' }}>
            <Zoom in timeout={500}>
              <Button
                variant="contained"
                startIcon={<Camera />}
                onClick={toggleCamera}
                size={isMobile ? "small" : "medium"}
              >
                {isCapturing ? 'Stop Camera' : 'Start Camera'}
              </Button>
            </Zoom>
            <Zoom in timeout={500} style={{ transitionDelay: '100ms' }}>
              <Button
                variant="contained"
                startIcon={<Upload />}
                onClick={() => fileInputRef.current?.click()}
                size={isMobile ? "small" : "medium"}
              >
                Upload Image
              </Button>
            </Zoom>
          </Box>

          <input
            type="file"
            ref={fileInputRef}
            onChange={handleFileUpload}
            accept="image/*"
            style={{ display: 'none' }}
          />

          {isCapturing && (
            <Dialog
              fullScreen={isMobile}
              open={isCapturing}
              onClose={toggleCamera}
              TransitionComponent={Zoom}
            >
              <DialogContent sx={{ p: 0, position: 'relative' }}>
                <IconButton
                  sx={{ position: 'absolute', top: 8, right: 8, zIndex: 1 }}
                  onClick={toggleCamera}
                >
                  <CloseIcon />
                </IconButton>
                <Webcam
                  ref={webcamRef}
                  audio={false}
                  screenshotFormat="image/jpeg"
                  style={{ width: '100%' }}
                />
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleCapture}
                  sx={{
                    position: 'absolute',
                    bottom: 16,
                    left: '50%',
                    transform: 'translateX(-50%)',
                  }}
                  disabled={loading}
                >
                  {loading ? <CircularProgress size={24} /> : 'Capture'}
                </Button>
              </DialogContent>
            </Dialog>
          )}

          {loading && (
            <Box sx={{ width: '100%', mb: 3 }}>
              <LinearProgress />
              <Typography
                variant="body2"
                color="textSecondary"
                align="center"
                sx={{ mt: 1 }}
              >
                Processing image...
              </Typography>
            </Box>
          )}

          {result && (
            <MotionCard
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0, scale: 0.9 }}
              transition={{ duration: 0.3 }}
              sx={{ mb: 3 }}
            >
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
                  <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.2 }}
                  >
                    <Typography variant="subtitle2" color="textSecondary">
                      Product Name
                    </Typography>
                    <Typography variant="h6" sx={{ mb: 2 }}>
                      {result.name || 'Not detected'}
                    </Typography>
                  </motion.div>

                  <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.3 }}
                  >
                    <Typography variant="subtitle2" color="textSecondary">
                      Price Information
                    </Typography>
                    <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                      <Box>
                        <Typography variant="caption" color="textSecondary">
                          Actual Price
                        </Typography>
                        <Typography variant="h5" color="primary">
                          ${result.actual_price.toFixed(2)}
                        </Typography>
                      </Box>
                      {result.suggested_price && (
                        <Box>
                          <Typography variant="caption" color="textSecondary">
                            Suggested Price
                          </Typography>
                          <Typography variant="h5" color="secondary">
                            ${result.suggested_price.toFixed(2)}
                          </Typography>
                        </Box>
                      )}
                    </Box>
                  </motion.div>

                  <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.4 }}
                  >
                    <Typography variant="subtitle2" color="textSecondary">
                      Stock Information
                    </Typography>
                    <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                      <Box>
                        <Typography variant="caption" color="textSecondary">
                          Quantity
                        </Typography>
                        <Typography variant="h6">
                          {result.stock_quantity}
                        </Typography>
                      </Box>
                      {result.low_stock_threshold && (
                        <Box>
                          <Typography variant="caption" color="textSecondary">
                            Low Stock Alert
                          </Typography>
                          <Typography variant="h6">
                            {result.low_stock_threshold}
                          </Typography>
                        </Box>
                      )}
                    </Box>
                  </motion.div>

                  {result.tag_names.length > 0 && (
                    <motion.div
                      initial={{ opacity: 0, y: 10 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ delay: 0.5 }}
                    >
                      <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                        Tags
                      </Typography>
                      <Stack direction="row" spacing={1} sx={{ mb: 2, flexWrap: 'wrap', gap: 1 }}>
                        {result.tag_names.map((tag, index) => (
                          <Chip key={index} label={tag} size="small" />
                        ))}
                      </Stack>
                    </motion.div>
                  )}

                  <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.6 }}
                  >
                    <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                      Confidence
                    </Typography>
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
                      <Typography variant="body2" color="textSecondary">
                        {(result.confidence * 100).toFixed(1)}%
                      </Typography>
                    </Box>
                  </motion.div>

                  {result.text_found.length > 0 && (
                    <motion.div
                      initial={{ opacity: 0, y: 10 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ delay: 0.7 }}
                    >
                      <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                        Text Found
                      </Typography>
                      <List dense>
                        {result.text_found.map((text, index) => (
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
          )}

          <Snackbar
            open={showSnackbar}
            autoHideDuration={6000}
            onClose={() => setShowSnackbar(false)}
            anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
          >
            <Alert
              onClose={() => setShowSnackbar(false)}
              severity={error ? 'error' : 'success'}
              sx={{ width: '100%' }}
            >
              {snackbarMessage}
            </Alert>
          </Snackbar>
        </motion.div>
      </AnimatePresence>
    </Box>
  );
};

export default Scanner;
