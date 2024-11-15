// src/components/Settings.tsx
import React, { useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Switch,
  Select,
  MenuItem,
  FormControl,
  FormControlLabel,
  InputLabel,
  Alert,
  RadioGroup,
  Radio,
  useTheme,
  Button,
} from '@mui/material';
import {
  Camera,
  Monitor,
  Smartphone,
  Save,
  RotateCcw,
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useCustomTheme } from '../../theme/ThemeContext';

interface SettingsSection {
  title: string;
  icon: React.ReactNode;
}

const Settings: React.FC = () => {
  const theme = useTheme();
  const { currentTheme, setTheme, availableThemes } = useCustomTheme();
  const [cameraDevice, setCameraDevice] = useState<string>('default');
  const [resolution, setResolution] = useState<string>('720p');
  const [autoScan, setAutoScan] = useState<boolean>(true);
  const [showAlert, setShowAlert] = useState<boolean>(false);
  const [saveSuccess, setSaveSuccess] = useState<boolean>(false);

  // Mock available cameras
  const availableCameras = [
    { id: 'default', label: 'Default Camera' },
    { id: 'front', label: 'Front Camera' },
    { id: 'back', label: 'Back Camera' },
  ];

  const resolutionOptions = [
    { value: '480p', label: '480p (SD)' },
    { value: '720p', label: 'HD (720p)' },
    { value: '1080p', label: 'Full HD (1080p)' },
  ];

  const sections: SettingsSection[] = [
    { title: 'Theme Settings', icon: <Monitor size={24} /> },
    { title: 'Camera Settings', icon: <Camera size={24} /> },
    { title: 'Device Settings', icon: <Smartphone size={24} /> },
  ];

  const handleSave = () => {
    try {
      localStorage.setItem('scannerSettings', JSON.stringify({
        cameraDevice,
        resolution,
        autoScan,
        theme: currentTheme,
      }));
      setSaveSuccess(true);
      setTimeout(() => setSaveSuccess(false), 3000);
    } catch (error) {
      setShowAlert(true);
    }
  };

  const handleReset = () => {
    setCameraDevice('default');
    setResolution('720p');
    setAutoScan(true);
    setTheme({ preset: 'default', mode: 'light' });
  };

  return (
    <Box
      component={motion.div}
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0 }}
      sx={{ maxWidth: 800, mx: 'auto', p: 2 }}
    >
      <Typography variant="h4" gutterBottom sx={{ mb: 4 }}>
        Settings
      </Typography>

      <AnimatePresence>
        {showAlert && (
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
          >
            <Alert 
              severity="error" 
              onClose={() => setShowAlert(false)}
              sx={{ mb: 3 }}
            >
              Failed to save settings. Please try again.
            </Alert>
          </motion.div>
        )}

        {saveSuccess && (
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
          >
            <Alert 
              severity="success" 
              onClose={() => setSaveSuccess(false)}
              sx={{ mb: 3 }}
            >
              Settings saved successfully!
            </Alert>
          </motion.div>
        )}
      </AnimatePresence>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
        {/* Theme Settings */}
        <Card
          component={motion.div}
          whileHover={{ scale: 1.01 }}
          transition={{ duration: 0.2 }}
        >
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
              {sections[0].icon}
              <Typography variant="h6" sx={{ ml: 2 }}>
                {sections[0].title}
              </Typography>
            </Box>
            
            <FormControl component="fieldset" sx={{ width: '100%' }}>
              <Typography variant="subtitle2" gutterBottom>
                Color Theme
              </Typography>
              <RadioGroup
                value={currentTheme.preset}
                onChange={(e) => setTheme({ preset: e.target.value })}
              >
                <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))' }}>
                  {availableThemes.map((preset: { id: string; name: string | number | boolean | React.ReactElement<any, string | React.JSXElementConstructor<any>> | Iterable<React.ReactNode> | React.ReactPortal | null | undefined; primary: any; secondary: any; }) => (
                    <motion.div
                      key={preset.id}
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                    >
                      <Card
                        sx={{
                          border: preset.id === currentTheme.preset 
                            ? `2px solid ${theme.palette.primary.main}`
                            : `1px solid ${theme.palette.divider}`,
                          cursor: 'pointer',
                        }}
                        onClick={() => setTheme({ preset: preset.id })}
                      >
                        <CardContent>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                            <Radio value={preset.id} />
                            <Box>
                              <Typography variant="subtitle2">
                                {preset.name}
                              </Typography>
                              <Box
                                sx={{
                                  width: 50,
                                  height: 20,
                                  borderRadius: 1,
                                  background: `linear-gradient(to right, ${preset.primary}, ${preset.secondary})`,
                                  mt: 0.5,
                                }}
                              />
                            </Box>
                          </Box>
                        </CardContent>
                      </Card>
                    </motion.div>
                  ))}
                </Box>
              </RadioGroup>
            </FormControl>

            <Box sx={{ mt: 3 }}>
              <FormControlLabel
                control={
                  <Switch
                    checked={currentTheme.mode === 'dark'}
                    onChange={() => setTheme({ 
                      mode: currentTheme.mode === 'light' ? 'dark' : 'light' 
                    })}
                  />
                }
                label="Dark Mode"
              />
            </Box>
          </CardContent>
        </Card>

        <Card
          component={motion.div}
          whileHover={{ scale: 1.01 }}
          transition={{ duration: 0.2 }}
        >
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
              {sections[1].icon}
              <Typography variant="h6" sx={{ ml: 2 }}>
                {sections[1].title}
              </Typography>
            </Box>

            <FormControl fullWidth sx={{ mb: 3 }}>
              <InputLabel>Camera Device</InputLabel>
              <Select
                value={cameraDevice}
                label="Camera Device"
                onChange={(e) => setCameraDevice(e.target.value)}
              >
                {availableCameras.map((camera) => (
                  <MenuItem key={camera.id} value={camera.id}>
                    {camera.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControl fullWidth sx={{ mb: 3 }}>
              <InputLabel>Resolution</InputLabel>
              <Select
                value={resolution}
                label="Resolution"
                onChange={(e) => setResolution(e.target.value)}
              >
                {resolutionOptions.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControlLabel
              control={
                <Switch
                  checked={autoScan}
                  onChange={(e) => setAutoScan(e.target.checked)}
                />
              }
              label="Auto-scan when camera is active"
            />
          </CardContent>
        </Card>

        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', mt: 2 }}>
          <Button
            variant="outlined"
            startIcon={<RotateCcw size={20} />}
            onClick={handleReset}
          >
            Reset to Defaults
          </Button>
          <Button
            variant="contained"
            startIcon={<Save size={20} />}
            onClick={handleSave}
          >
            Save Settings
          </Button>
        </Box>
      </Box>
    </Box>
  );
};

export default Settings;