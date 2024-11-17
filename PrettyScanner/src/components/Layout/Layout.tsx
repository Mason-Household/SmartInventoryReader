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
  LogOut,
  Github as GitHubIcon,
} from 'lucide-react';
import ErrorBoundary from '../Errors/ErrorBoundary';
import HelpOutline from '../HelpOutline/HelpOutline';
import { motion, AnimatePresence } from 'framer-motion';
import ThemeSelector from '../ThemeSelector/ThemeSelector';
import { useAuth } from '../../context/AuthContext';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [drawerOpen, setDrawerOpen] = React.useState(false);
  const { logout, organization } = useAuth();

  const iconColor = theme.palette.mode === 'light' ? 'black' : 'inherit';

  const menuItems = [
    { icon: <Home color={iconColor} />, text: 'Home', path: '/' },
    { icon: <History color={iconColor} />, text: 'Scan History', path: '/history' },
    { icon: <Settings color={iconColor} />, text: 'Settings', path: '/settings' },
    { icon: <Box color={iconColor}><HelpOutline /></Box>, text: 'Help', path: '/help' },
  ];

  const handleDrawerToggle = () => {
    setDrawerOpen(!drawerOpen);
  };

  const handleDrawerClose = () => {
    setDrawerOpen(false);
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
        style={{
          color: theme.palette.mode === 'light' ? 'black' : 'inherit',
        }}
      >
        <Toolbar>
          {isMobile && (
            <IconButton
              edge="start"
              color="inherit"
              aria-label="menu"
              onClick={handleDrawerToggle}
              sx={{ mr: 2 }}
            >
              <MenuIcon color={iconColor} />
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
              Inventory Scanner
            </Typography>
          </motion.div>

          {organization && (
            <Typography
              variant="body2"
              sx={{
                ml: 2,
                color: 'text.secondary',
                display: { xs: 'none', sm: 'block' },
              }}
            >
              {organization.name}
            </Typography>
          )}

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

          <Box sx={{ ml: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <ThemeSelector />
            <Tooltip title="Logout" arrow>
              <IconButton color="inherit" onClick={logout}>
                <LogOut color={iconColor} />
              </IconButton>
            </Tooltip>
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
              <GitHubIcon color={iconColor} />
            </IconButton>
          </Tooltip>
        </Toolbar>
      </AppBar>

      <Drawer
        anchor="left"
        open={drawerOpen}
        onClose={handleDrawerClose}
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
          {organization && (
            <Typography variant="body2" color="text.secondary">
              {organization.name}
            </Typography>
          )}
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
              onClick={handleDrawerClose}
              button
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
          <Divider sx={{ my: 1 }} />
          <ListItem
            component={motion.div}
            whileHover={{ x: 10 }}
            whileTap={{ scale: 0.95 }}
            onClick={() => {
              logout();
              handleDrawerClose();
            }}
            button
          >
            <ListItemIcon><LogOut color={iconColor} /></ListItemIcon>
            <ListItemText primary="Logout" />
          </ListItem>
        </List>
      </Drawer>

      <Container
        component="main"
        maxWidth="lg"
        sx={{
          flex: 1,
          py: 4,
          px: { xs: 2, sm: 3 },
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <ErrorBoundary>
          <AnimatePresence mode="wait">
            <Box
              sx={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                gap: 3,
              }}
            >
              {children}
            </Box>
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
          <iframe src="https://github.com/sponsors/Mason-Household/card" title="Sponsor Mason-Household" height="225" width="600" style={{ border: 0 }}></iframe>
          <Typography variant="body2" color="text.secondary" align="center">
            © {new Date().getFullYear()} Inventory Scanner. All rights reserved.
          </Typography>
        </Container>
      </Box>
    </Box>
  );
};

export default Layout;