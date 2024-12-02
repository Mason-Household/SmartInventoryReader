import React from 'react';
import { Dialog, DialogContent, IconButton, Button, CircularProgress } from '@mui/material';
import Webcam from 'react-webcam';
import { X as CloseIcon } from 'lucide-react';

interface CaptureDialogProps {
  isCapturing: boolean;
  toggleCamera: () => void;
  handleCapture: () => void;
  loading: boolean;
  webcamRef: React.RefObject<Webcam>;
  isMobile: boolean;
}

const CaptureDialog: React.FC<CaptureDialogProps> = ({
  isCapturing,
  toggleCamera,
  handleCapture,
  loading,
  webcamRef,
  isMobile,
}) => (
  <Dialog fullScreen={isMobile} open={isCapturing} onClose={toggleCamera}>
    <DialogContent sx={{ p: 0, position: 'relative' }}>
      <IconButton sx={{ position: 'absolute', top: 8, right: 8, zIndex: 1 }} onClick={toggleCamera}>
        <CloseIcon />
      </IconButton>
      <Webcam ref={webcamRef} audio={false} screenshotFormat="image/jpeg" style={{ width: '100%' }} />
      <Button
        variant="contained"
        color="primary"
        onClick={handleCapture}
        sx={{ position: 'absolute', bottom: 16, left: '50%', transform: 'translateX(-50%)' }}
        disabled={loading}
      >
        {loading ? <CircularProgress size={24} /> : 'Capture'}
      </Button>
    </DialogContent>
  </Dialog>
);

export default CaptureDialog;