import {
  Box,
  Button,
  Card,
  CardContent,
  Typography,
  useTheme,
} from '@mui/material';
import { motion } from 'framer-motion';
import React, { Component, ErrorInfo } from 'react';
import { AlertTriangle, RefreshCw, Home } from 'lucide-react';

interface Props {
  children: React.ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
}

const ErrorFallback: React.FC<{
  error: Error | null;
  resetError: () => void;
}> = ({ error, resetError }) => {
  const theme = useTheme();

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -20 }}
    >
      <Card
        sx={{
          maxWidth: 600,
          mx: 'auto',
          mt: 4,
          border: `1px solid ${theme.palette.error.main}`,
          backgroundColor: theme.palette.mode === 'light' 
            ? 'rgba(255, 0, 0, 0.02)' 
            : 'rgba(255, 0, 0, 0.1)',
        }}
      >
        <CardContent>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              textAlign: 'center',
              py: 4,
            }}
          >
            <motion.div
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{
                type: "spring",
                stiffness: 260,
                damping: 20
              }}
            >
              <AlertTriangle 
                size={64} 
                color={theme.palette.error.main}
                style={{ marginBottom: theme.spacing(2) }}
              />
            </motion.div>

            <Typography variant="h5" color="error" gutterBottom>
              Oops! Something went wrong
            </Typography>

            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              {error?.message || 'An unexpected error occurred'}
            </Typography>

            {import.meta.env.DEV && error && (
              <Box
                sx={{
                  p: 2,
                  mb: 3,
                  bgcolor: theme.palette.grey[100],
                  borderRadius: 1,
                  overflow: 'auto',
                  maxWidth: '100%',
                  '& pre': {
                    margin: 0,
                    fontFamily: 'monospace',
                    fontSize: '0.875rem',
                  },
                }}
              >
                <pre>{error.stack}</pre>
              </Box>
            )}

            <Box sx={{ display: 'flex', gap: 2 }}>
              <Button
                variant="contained"
                color="primary"
                startIcon={<RefreshCw />}
                onClick={resetError}
              >
                Try Again
              </Button>
              <Button
                variant="outlined"
                startIcon={<Home />}
                onClick={() => window.location.href = '/'}
              >
                Go to Home
              </Button>
            </Box>
          </Box>
        </CardContent>
      </Card>
    </motion.div>
  );
};

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: null,
    errorInfo: null,
  };

  public static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
      errorInfo: null,
    };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Error caught by error boundary:', error, errorInfo);
    this.setState({
      error,
      errorInfo,
    });
    
  }

  public resetError = () => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
    });
  };

  public render() {
    if (this.state.hasError) {
      return (
        <ErrorFallback
          error={this.state.error}
          resetError={this.resetError}
        />
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;