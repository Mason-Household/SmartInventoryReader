import React, { createContext, useContext, useState, useMemo, useEffect } from 'react';
import { ThemeProvider as MUIThemeProvider, createTheme, alpha } from '@mui/material';
import { motion, AnimatePresence } from 'framer-motion';
import { themePresets, type CustomTheme, type ThemePreset } from './types';

interface ThemeContextType {
  currentTheme: CustomTheme;
  setTheme: (theme: Partial<CustomTheme>) => void;
  availableThemes: typeof themePresets;
}

const ThemeContext = createContext<ThemeContextType>({
  currentTheme: { preset: 'default', mode: 'light' },
  setTheme: () => {},
  availableThemes: themePresets,
});

const createCustomTheme = (themeConfig: CustomTheme) => {
  const preset = themePresets.find(p => p.id === themeConfig.preset) || themePresets[0];
  
  return createTheme({
    palette: {
      mode: themeConfig.mode,
      primary: {
        main: themeConfig.mode === 'light' ? preset.primary : preset.accentLight,
      },
      secondary: {
        main: themeConfig.mode === 'light' ? preset.secondary : preset.accentDark,
      },
      background: {
        default: themeConfig.mode === 'light' ? preset.background : alpha(preset.background, 0.1),
        paper: themeConfig.mode === 'light' ? preset.paper : alpha(preset.paper, 0.1),
      },
    },
    components: {
      MuiPaper: {
        styleOverrides: {
          root: {
            transition: 'background-color 0.3s ease-in-out, color 0.3s ease-in-out',
          },
        },
      },
      MuiButton: {
        styleOverrides: {
          root: {
            textTransform: 'none',
            borderRadius: 8,
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            transition: 'transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out',
            '&:hover': {
              transform: 'translateY(-2px)',
            },
          },
        },
      },
      MuiIconButton: {
        styleOverrides: {
          root: {
            transition: 'transform 0.2s ease-in-out',
            '&:hover': {
              transform: 'scale(1.1)',
            },
          },
        },
      },
    },
  });
};

export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [currentTheme, setCurrentTheme] = useState<CustomTheme>(() => {
    const saved = localStorage.getItem('theme');
    return saved ? JSON.parse(saved) : { preset: 'default', mode: 'light' };
  });

  const theme = useMemo(() => createCustomTheme(currentTheme), [currentTheme]);

  useEffect(() => {
    localStorage.setItem('theme', JSON.stringify(currentTheme));
  }, [currentTheme]);

  const setTheme = (newTheme: Partial<CustomTheme>) => {
    setCurrentTheme((curr: any) => ({ ...curr, ...newTheme }));
  };

  return (
    <ThemeContext.Provider value={{ currentTheme, setTheme, availableThemes: themePresets }}>
      <MUIThemeProvider theme={theme}>
        <AnimatePresence mode="wait">
          <motion.div
            key={`${currentTheme.preset}-${currentTheme.mode}`}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
          >
            {children}
          </motion.div>
        </AnimatePresence>
      </MUIThemeProvider>
    </ThemeContext.Provider>
  );
};

export const useCustomTheme = () => useContext(ThemeContext);