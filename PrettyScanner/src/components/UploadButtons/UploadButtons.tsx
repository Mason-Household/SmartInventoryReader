import React from 'react';
import { Box, Button, Zoom } from '@mui/material';
import { Camera, Upload } from 'lucide-react';

interface UploadButtonsProps {
  toggleCamera: () => void;
  handleButtonClick: () => void;
  isCapturing: boolean;
  isMobile: boolean;
}

const UploadButtons: React.FC<UploadButtonsProps> = ({ toggleCamera, handleButtonClick, isCapturing, isMobile }) => (
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
        onClick={handleButtonClick}
        size={isMobile ? "small" : "medium"}
      >
        Upload Image
      </Button>
    </Zoom>
  </Box>
);

export default UploadButtons;