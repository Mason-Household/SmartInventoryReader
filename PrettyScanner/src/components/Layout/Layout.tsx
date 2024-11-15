import React from 'react';
import {
  AppBar,
  Box,
  Container,
  IconButton,
  Toolbar,
  Typography,
  useMediaQuery,
  useTheme,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Tooltip,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Home,
  History,
  Settings,
  HelpOutline,
} from '@mui/icons-material';
import GitHubIcon from '@mui/icons-material/GitHub';
import { motion, AnimatePresence } from 'framer-motion';
import ThemeSelector from '../ThemeSelector/ThemeSelector';
import ErrorBoundary from '../ErrorBoundary/ErrorBoundary';
interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [drawerOpen, setDrawerOpen] = React.useState(false);

  const menuItems = [
    { icon: <Home size={20} />, text: 'Home', path: '/' },
    { icon: <History size={20} />, text: 'Scan History', path: '/history' },
    { icon: <Settings size={20} />, text: 'Settings', path: '/settings' },
    { icon: <HelpOutline size={20} />, text: 'Help', path: '/help' },
  ];

  const toggleDrawer = (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => {
    if (
      event.type === 'keydown' &&
      ((event as React.KeyboardEvent).key === 'Tab' ||
        (event as React.KeyboardEvent).key === 'Shift')
    ) {
      return;
    }
    setDrawerOpen(open);
  };

  return (
    <Box
      component={motion.div}
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      sx={{
        minHeight: '100vh',
        bgcolor: 'background.default',
        display: 'flex',
        flexDirection: 'column',
      }}
    >
      <AppBar
        position="sticky"
        elevation={0}
        sx={{
          backdropFilter: 'blur(8px)',
          backgroundColor: 'rgba(255, 255, 255, 0.8)',
          borderBottom: `1px solid ${theme.palette.divider}`,
        }}
      >
        <Toolbar>
          {isMobile && (
            <IconButton
              edge="start"
              color="inherit"
              aria-label="menu"
              onClick={toggleDrawer(true)}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}

          <motion.div
            initial={{ x: -20, opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3 }}
          >
            <Typography
              variant="h6"
              component="h1"
              sx={{
                fontWeight: 700,
                background: `linear-gradient(45deg, ${theme.palette.primary.main}, ${theme.palette.secondary.main})`,
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
              }}
            >
              Sneaker Scanner
            </Typography>
          </motion.div>

          <Box sx={{ flexGrow: 1 }} />

          {!isMobile && (
            <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
              {menuItems.map((item, index) => (
                <Tooltip key={item.text} title={item.text} arrow>
                  <IconButton
                    color="inherit"
                    component={motion.button}
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: index * 0.1 }}
                  >
                    {item.icon}
                  </IconButton>
                </Tooltip>
              ))}
            </Box>
          )}

          <Box sx={{ ml: 2 }}>
            <ThemeSelector />
          </Box>

          <Tooltip title="View on GitHub" arrow>
            <IconButton
              color="inherit"
              component="a"
              href="https://github.com/Mason-Household/SmartInventoryReader.git"
              target="_blank"
              rel="noopener noreferrer"
              sx={{ ml: 1 }}
            >
              <GitHubIcon />
            </IconButton>
          </Tooltip>
        </Toolbar>
      </AppBar>

      <Drawer
        anchor="left"
        open={drawerOpen}
        onClose={toggleDrawer(false)}
        variant="temporary"
        sx={{
          '& .MuiDrawer-paper': {
            width: 240,
            boxSizing: 'border-box',
            bgcolor: 'background.paper',
          },
        }}
      >
        <Box sx={{ p: 2 }}>
          <Typography variant="h6" color="primary" sx={{ fontWeight: 700 }}>
            Menu
          </Typography>
        </Box>
        <Divider />
        <List>
          {menuItems.map((item, index) => (
            <ListItem
              key={item.text}
              component={motion.div}
              whileHover={{ x: 10 }}
              whileTap={{ scale: 0.95 }}
              initial={{ x: -20, opacity: 0 }}
              animate={{ x: 0, opacity: 1 }}
              transition={{ delay: index * 0.1 }}
              onClick={toggleDrawer(false)}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
        </List>
      </Drawer>

      <Container
        component="main"
        maxWidth="lg"
        sx={{
          flex: 1,
          py: 4,
          px: { xs: 2, sm: 3 },
        }}
      >
        <ErrorBoundary>
          <AnimatePresence mode="wait">
            {children}
          </AnimatePresence>
        </ErrorBoundary>
      </Container>

      <Box
        component="footer"
        sx={{
          py: 3,
          px: 2,
          mt: 'auto',
          backgroundColor: theme.palette.mode === 'light'
            ? theme.palette.grey[100]
            : theme.palette.grey[900],
        }}
      >
        <Container maxWidth="lg">
          <Typography variant="body2" color="text.secondary" align="center">
            © {new Date().getFullYear()} Sneaker Scanner. All rights reserved.
          </Typography>
        </Container>
      </Box>
    </Box>
  );
};

export default Layout;