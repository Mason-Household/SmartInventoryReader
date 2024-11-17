// src/components/Home.tsx
import React from 'react';
import {
  Card,
  CardContent,
  Box,
  Typography,
  Button,
  useTheme,
  useMediaQuery,
  Paper,
  Tooltip,
  alpha,
  Container,
} from '@mui/material';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Camera,
  Upload,
  History,
  QrCode,
  Image as ImageIcon,
  TrendingUp,
  Settings,
} from 'lucide-react';
import BarcodeIcon from 'feather-icons/dist/icons/barcode.svg';
import { useNavigate } from 'react-router-dom';

const MotionCard = motion(React.forwardRef<HTMLDivElement, any>((props, ref) => <Card {...props} ref={ref} component="div" />));
const MotionPaper = motion(React.forwardRef<HTMLDivElement, any>((props, ref) => <Paper {...props} ref={ref} />));

interface QuickActionProps {
  icon: React.ReactElement;
  title: string;
  description: string;
  onClick: () => void;
}

const QuickAction: React.FC<QuickActionProps> = ({
  icon,
  title,
  description,
  onClick,
}) => {
  const theme = useTheme();

  return (
    <MotionCard
      whileHover={{ scale: 1.02 }}
      whileTap={{ scale: 0.98 }}
      onClick={onClick}
      sx={{
        cursor: 'pointer',
        height: '100%',
        background: `linear-gradient(45deg, ${alpha(theme.palette.primary.main, 0.05)}, ${alpha(theme.palette.secondary.main, 0.05)})`,
      }}
    >
      <CardContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: 2 }}>
          <Box
            sx={{
              p: 2,
              borderRadius: '50%',
              backgroundColor: alpha(theme.palette.primary.main, 0.1),
              color: theme.palette.primary.main,
            }}
          >
            {icon}
          </Box>
          <Typography variant="h6">{title}</Typography>
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </Box>
      </CardContent>
    </MotionCard>
  );
};

const RecentScan: React.FC<{
  type: string;
  price: number;
  timestamp: string;
  confidence: number;
}> = ({ type, price, timestamp, confidence }) => {
  const theme = useTheme();

  return (
    <MotionPaper
      whileHover={{ scale: 1.01 }}
      sx={{ p: 2, mb: 2 }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
        {type === 'barcode' && <BarcodeIcon />}
        {type === 'qrcode' && <QrCode size={24} />}
        {type === 'image' && <ImageIcon size={24} />}
        <Box sx={{ flex: 1 }}>
          <Typography variant="subtitle2">
            ${price.toFixed(2)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {new Date(timestamp).toLocaleString()}
          </Typography>
        </Box>
        <Tooltip title={`${(confidence * 100).toFixed(1)}% confidence`}>
          <Box
            sx={{
              width: 40,
              height: 40,
              borderRadius: '50%',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              backgroundColor: alpha(theme.palette.success.main, confidence),
              color: theme.palette.success.main,
            }}
          >
            {(confidence * 100).toFixed(0)}%
          </Box>
        </Tooltip>
      </Box>
    </MotionPaper>
  );
};

const Home: React.FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  // Mock recent scans data
  const recentScans = [
    { type: 'barcode', price: 129.99, timestamp: '2024-02-14T10:30:00', confidence: 0.95 },
    { type: 'image', price: 199.99, timestamp: '2024-02-13T15:45:00', confidence: 0.85 },
    { type: 'qrcode', price: 89.99, timestamp: '2024-02-12T09:15:00', confidence: 0.92 },
  ];

  const quickActions = [
    {
      icon: <Camera size={32} />,
      title: 'Quick Scan',
      description: 'Scan a sneaker using your camera',
      onClick: () => navigate('/scanner'),
    },
    {
      icon: <Upload size={32} />,
      title: 'Upload Image',
      description: 'Upload an image for analysis',
      onClick: () => navigate('/scanner?mode=upload'),
    },
    {
      icon: <History size={32} />,
      title: 'Scan History',
      description: 'View your previous scans',
      onClick: () => navigate('/history'),
    },
    {
      icon: <Settings size={32} />,
      title: 'Settings',
      description: 'Configure app preferences',
      onClick: () => navigate('/settings'),
    },
  ];

  return (
    <Container maxWidth="lg">
      <Box
        component={motion.div}
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        exit={{ opacity: 0 }}
        sx={{ 
          p: { xs: 2, sm: 3 },
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        {/* Welcome Section */}
        <Box sx={{ mb: 4, textAlign: 'center', width: '100%' }}>
          <Typography variant="h4" gutterBottom>
            Welcome to Inventory Scanner
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Quickly scan and price check your sneakers using AI-powered image recognition
          </Typography>
        </Box>

        {/* Quick Actions */}
        <Box sx={{ mb: 4, width: '100%' }}>
          <Typography variant="h6" gutterBottom sx={{ textAlign: 'center' }}>
            Quick Actions
          </Typography>
          <Box sx={{ 
            display: 'flex', 
            flexWrap: 'wrap', 
            gap: isMobile ? 2 : 3,
            justifyContent: 'center',
          }}>
            {quickActions.map((action, index) => (
              <Box 
                key={action.title}
                sx={{ 
                  flex: isMobile ? '1 1 100%' : '1 1 calc(25% - 24px)',
                  minWidth: isMobile ? 'auto' : '200px',
                  maxWidth: isMobile ? 'none' : '280px',
                }}
              >
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.1 }}
                >
                  <QuickAction {...action} />
                </motion.div>
              </Box>
            ))}
          </Box>
        </Box>

        {/* Recent Scans and Stats */}
        <Box sx={{ 
          display: 'flex', 
          flexDirection: isMobile ? 'column' : 'row',
          gap: isMobile ? 2 : 3,
          width: '100%',
          justifyContent: 'center',
        }}>
          {/* Recent Scans */}
          <Box sx={{ flex: isMobile ? '1' : '2', width: '100%' }}>
            <Card sx={{ height: '100%' }}>
              <CardContent>
                <Typography variant="h6" gutterBottom sx={{ textAlign: 'center' }}>
                  Recent Scans
                </Typography>
                <AnimatePresence>
                  {recentScans.map((scan, index) => (
                    <motion.div
                      key={scan.timestamp}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      exit={{ opacity: 0, x: 20 }}
                      transition={{ delay: index * 0.1 }}
                    >
                      <RecentScan {...scan} />
                    </motion.div>
                  ))}
                </AnimatePresence>
                {recentScans.length > 0 && (
                  <Button
                    variant="outlined"
                    fullWidth
                    onClick={() => navigate('/history')}
                    sx={{ mt: 2 }}
                  >
                    View All Scans
                  </Button>
                )}
              </CardContent>
            </Card>
          </Box>

          {/* Stats */}
          <Box sx={{ flex: isMobile ? '1' : '1', width: '100%' }}>
            <Card sx={{ height: '100%' }}>
              <CardContent>
                <Typography variant="h6" gutterBottom sx={{ textAlign: 'center' }}>
                  Statistics
                </Typography>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, alignItems: 'center' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <TrendingUp size={24} />
                    <Box sx={{ textAlign: 'center' }}>
                      <Typography variant="h4">
                        {recentScans.length}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        Total Scans
                      </Typography>
                    </Box>
                  </Box>
                  <Box sx={{ 
                    display: 'flex', 
                    flexDirection: isMobile ? 'column' : 'row', 
                    gap: 2,
                    width: '100%',
                  }}>
                    <Box sx={{ 
                      flex: 1, 
                      textAlign: 'center', 
                      p: 2, 
                      bgcolor: alpha(theme.palette.primary.main, 0.1), 
                      borderRadius: 1 
                    }}>
                      <Typography variant="h6">
                        {recentScans.filter(s => s.confidence > 0.9).length}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        High Confidence
                      </Typography>
                    </Box>
                    <Box sx={{ 
                      flex: 1, 
                      textAlign: 'center', 
                      p: 2, 
                      bgcolor: alpha(theme.palette.secondary.main, 0.1), 
                      borderRadius: 1 
                    }}>
                      <Typography variant="h6">
                        ${recentScans.reduce((acc, curr) => acc + curr.price, 0).toFixed(2)}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        Total Value
                      </Typography>
                    </Box>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Box>
        </Box>
      </Box>
    </Container>
  );
};

export default Home;
