import React, { useRef, useState, useCallback } from 'react';
import Webcam from 'react-webcam';
import {
  Box,
  Input,
  LinearProgress,
  Typography,
  Alert,
  Snackbar,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import { motion, AnimatePresence } from 'framer-motion';
import { ScanRes } from './ScanResult';
import CaptureDialog from '../CaptureDialog/CaptureDialog';
import ResultCard from '../ResultCard/ResultCard';
import UploadButtons from '../UploadButtons/UploadButtons';

const Scanner: React.FC = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [isCapturing, setIsCapturing] = useState(false);
  const [result, setResult] = useState<ScanRes | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showSnackbar, setShowSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const webcamRef = useRef<Webcam | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const LLM_SERVICE_URL = import.meta.env.VITE_LLM_SERVICE_URL || 'http://localhost:8000';

  const handleError = (error: Error | string) => {
    const errorMessage = error instanceof Error ? error.message : error;
    console.error('Error:', errorMessage);
    setError(errorMessage);
    setShowSnackbar(true);
    setSnackbarMessage(
      errorMessage.includes('CORS') 
        ? 'Network error: Unable to connect to the server' 
        : 'An error occurred while processing the image'
    );
    setLoading(false);
  };

  const uploadImage = async (file: File | Blob) => {
    setLoading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch(`${LLM_SERVICE_URL}/analyze`, {
        method: 'POST',
        body: formData,
        headers: {
          'Accept': 'application/json',
        },
      });
      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(
          errorData?.message || 
          `Server error: ${response.status} ${response.statusText}`
        );
      }
      const data = await response.json();
      if (!data) {
        throw new Error('Invalid response from server');
      }
      setResult(data);
      setShowSnackbar(true);
      setSnackbarMessage('Image processed successfully!');
    } catch (err) {
      if (err instanceof Error && err.message.includes('Failed to fetch')) {
        handleError('Unable to connect to the server. Please check your network connection.');
      } else {
        handleError(err as Error);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCapture = useCallback(async () => {
    if (webcamRef.current) {
      try {
        const imageSrc = webcamRef.current.getScreenshot();
        if (!imageSrc) {
          throw new Error('Failed to capture image from webcam');
        }
        const base64Response = await fetch(imageSrc);
        const blob = await base64Response.blob();
        await uploadImage(blob);
      } catch (err) {
        handleError(err as Error);
      }
    } else {
      handleError('Webcam not initialized');
    }
  }, [webcamRef]);

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      if (file.size > 10 * 1024 * 1024) { // 10MB limit
        handleError('File size too large. Please upload an image under 10MB.');
        return;
      }
      if (!file.type.startsWith('image/')) {
        handleError('Invalid file type. Please upload an image file.');
        return;
      }
      uploadImage(file);
    }
  };

  const toggleCamera = () => {
    setIsCapturing(prev => !prev);
    setResult(null);
    setError(null);
  };

  const copyResult = async () => {
    if (result) {
      try {
        await navigator.clipboard.writeText(JSON.stringify(result, null, 2));
        setShowSnackbar(true);
        setSnackbarMessage('Results copied to clipboard!');
      } catch (err) {
        handleError('Failed to copy to clipboard');
      }
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
        if ((err as Error).name !== 'AbortError') {
          handleError(err as Error);
        }
      }
    }
  };

  const handleButtonClick = () => {
    fileInputRef.current?.click();
  };

  return (
    <Box sx={{ width: '100%', maxWidth: 600, mx: 'auto', p: 2 }}>
      <AnimatePresence mode="wait">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -20 }}
          transition={{ duration: 0.2 }}
        >
          <UploadButtons
            toggleCamera={toggleCamera}
            handleButtonClick={handleButtonClick}
            isCapturing={isCapturing}
            isMobile={isMobile}
          />

          <Input
            type="file"
            inputRef={fileInputRef}
            onChange={handleFileUpload}
            inputProps={{ 
              accept: "image/jpeg,image/png,image/gif,image/webp",
              multiple: false
            }}
            style={{ display: 'none' }}
          />

          {isCapturing && (
            <CaptureDialog
              isCapturing={isCapturing}
              toggleCamera={toggleCamera}
              handleCapture={handleCapture}
              loading={loading}
              webcamRef={webcamRef}
              isMobile={isMobile}
            />
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
            <ResultCard
              result={result}
              copyResult={copyResult}
              shareResult={shareResult}
              theme={theme}
            />
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
