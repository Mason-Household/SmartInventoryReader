// src/components/ThemeSelector.tsx
import React from 'react';
import {
  Card,
  CardActionArea,
  Dialog,
  DialogTitle,
  DialogContent,
  IconButton,
  Typography,
  useTheme,
  Tooltip,
  Zoom,
  Box,
} from '@mui/material';
import { Settings, Moon, Sun } from 'lucide-react';
import { motion } from 'framer-motion';
import { useCustomTheme } from '../../theme/ThemeContext';

const ThemeCard = motion(React.forwardRef<HTMLDivElement, any>((props, ref) => <Card {...props} ref={ref} component="div" />));

export const ThemeSelector: React.FC = () => {
  const [open, setOpen] = React.useState(false);
  const { currentTheme, setTheme, availableThemes } = useCustomTheme();
  const theme = useTheme();

  const handleThemeChange = (presetId: string) => {
    setTheme({ preset: presetId });
    setOpen(false);
  };

  const toggleMode = () => {
    setTheme({ mode: currentTheme.mode === 'light' ? 'dark' : 'light' });
  };

  return (
    <>
      <Box sx={{ display: 'flex', gap: 1 }}>
        <Tooltip title="Change theme" arrow TransitionComponent={Zoom}>
          <IconButton onClick={() => setOpen(true)} color="inherit">
            <Settings />
          </IconButton>
        </Tooltip>
        <Tooltip title="Toggle light/dark mode" arrow TransitionComponent={Zoom}>
          <IconButton onClick={toggleMode} color="inherit">
            {currentTheme.mode === 'dark' ? <Sun /> : <Moon />}
          </IconButton>
        </Tooltip>
      </Box>

      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        maxWidth="sm"
        fullWidth
        TransitionComponent={Zoom}
      >
        <DialogTitle>Select Theme</DialogTitle>
        <DialogContent>
          <Box container spacing={2} sx={{ mt: 1 }}>
            {availableThemes.map((preset: { id: string; primary: any; secondary: any; name: any; }) => {
              return (
                <Box item xs={12} sm={6} key={preset.id}>
                  <ThemeCard
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    sx={{
                      border: preset.id === currentTheme.preset ?
                        `2px solid ${theme.palette.primary.main}` :
                        '2px solid transparent'
                    }}
                  >
                    <CardActionArea
                      onClick={() => handleThemeChange(preset.id)}
                      sx={{ p: 2 }}
                    >
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Box
                          sx={{
                            width: 48,
                            height: 48,
                            borderRadius: '50%',
                            background: `linear-gradient(45deg, ${preset.primary}, ${preset.secondary})`,
                          }} />
                        <Box>
                          <Typography variant="h6">{preset.name}</Typography>
                          <Typography variant="body2" color="text.secondary">
                            {preset.id === currentTheme.preset ? 'Current theme' : 'Click to select'}
                          </Typography>
                        </Box>
                      </Box>
                    </CardActionArea>
                  </ThemeCard>
                </Box>
              );
            })}
          </Box>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default ThemeSelector;