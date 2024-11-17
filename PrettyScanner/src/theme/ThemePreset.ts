export type ThemePreset = {
  id: string;
  name: string;
  primary: string;
  secondary: string;
  background: string;
  paper: string;
  accentLight: string;
  accentDark: string;
};

export type CustomTheme = {
  preset: string;
  mode: 'light' | 'dark';
};

export const themePresets: ThemePreset[] = [
  {
    id: 'default',
    name: 'Midnight',
    primary: '#3f51b5',
    secondary: '#7c4dff',
    background: '#303030',
    paper: '#424242',
    accentLight: '#7986cb',
    accentDark: '#b388ff'
  },
  {
    id: 'nature',
    name: 'Nature',
    primary: '#4caf50',
    secondary: '#8bc34a',
    background: '#f1f8e9',
    paper: '#ffffff',
    accentLight: '#81c784',
    accentDark: '#aed581'
  },
  {
    id: 'sunset',
    name: 'Sunset',
    primary: '#ff9800',
    secondary: '#ff5722',
    background: '#fbe9e7',
    paper: '#ffffff',
    accentLight: '#ffb74d',
    accentDark: '#ff8a65'
  },
  {
    id: 'ocean',
    name: 'Ocean',
    primary: '#00acc1',
    secondary: '#26a69a',
    background: '#e0f7fa',
    paper: '#ffffff',
    accentLight: '#4dd0e1',
    accentDark: '#80cbc4'
  }
];