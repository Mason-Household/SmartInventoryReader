import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  Container,
  Alert,
  Link,
  Tabs,
  Tab,
  Divider,
} from '@mui/material';
import { Chrome } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { Organization } from '../../interfaces/Organization';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`auth-tabpanel-${index}`}
      aria-labelledby={`auth-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

const Login: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState(0);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [hfToken, setHfToken] = useState('');
  const [organization, setOrganization] = useState('');
  const [error, setError] = useState('');
  const [isRegistering, setIsRegistering] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const { 
    loginWithGoogle, 
    loginWithEmail, 
    registerWithEmail, 
    loginWithHuggingFace,
    isAuthenticated
  } = useAuth();

  // Add effect to handle navigation after successful authentication
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/');
    }
  }, [isAuthenticated, navigate]);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
    setError('');
  };

  const validateEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const validatePassword = (password: string): boolean => {
    return password.length >= 6;
  };

  const validateOrganization = (org: string): boolean => {
    return org.length >= 2 && /^[a-zA-Z0-9\s-]+$/.test(org);
  };

  const handleEmailSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    // Validation
    if (!validateEmail(email)) {
      setError('Please enter a valid email address');
      return;
    }

    if (!validatePassword(password)) {
      setError('Password must be at least 6 characters long');
      return;
    }

    if (!validateOrganization(organization)) {
      setError('Organization name must be at least 2 characters and contain only letters, numbers, spaces, and hyphens');
      return;
    }

    setIsSubmitting(true);

    try {
      if (isRegistering) {
        await registerWithEmail(email, password, organization);
      } else {
        await loginWithEmail(email, password);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 
        isRegistering ? 'Registration failed. Please try again.' : 'Invalid email or password.';
      setError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleHuggingFaceSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!validateOrganization(organization)) {
      setError('Organization name must be at least 2 characters and contain only letters, numbers, spaces, and hyphens');
      return;
    }

    if (!hfToken.trim()) {
      setError('Please enter your HuggingFace token');
      return;
    }

    setIsSubmitting(true);

    try {
      const org: Organization = {
        name: organization,
        slug: organization.toLowerCase().replace(/\s/g, '-'),
        createdAt: new Date(),
        isActive: true,
        id: null,
        subscriptionTier: 'free'
      };
      await loginWithHuggingFace(hfToken, org);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 
        'Invalid credentials. Please check your token and organization.';
      setError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleGoogleLogin = async () => {
    setError('');
    setIsSubmitting(true);
    try {
      await loginWithGoogle();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 
        'Google sign-in failed. Please try again.';
      setError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container component="main" maxWidth="sm" sx={{ 
      height: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center'
    }}>
      <Paper
        elevation={3}
        sx={{
          p: 4,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          width: '100%',
        }}
      >
        <Typography component="h1" variant="h5" sx={{ mb: 3 }}>
          Sign in to Inventory Scanner
        </Typography>

        {error && (
          <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box sx={{ width: '100%', mb: 3 }}>
          <Button
            fullWidth
            variant="outlined"
            startIcon={<Chrome size={20} />}
            onClick={handleGoogleLogin}
            disabled={isSubmitting}
            sx={{ mb: 2 }}
          >
            Continue with Google
          </Button>
        </Box>

        <Divider sx={{ width: '100%', mb: 3 }}>
          <Typography color="text.secondary">or</Typography>
        </Divider>

        <Box sx={{ width: '100%', mb: 2 }}>
          <Tabs value={activeTab} onChange={handleTabChange} centered>
            <Tab label="Email" />
            <Tab label="HuggingFace" />
          </Tabs>
        </Box>

        <TabPanel value={activeTab} index={0}>
          <Box component="form" onSubmit={handleEmailSubmit} sx={{ width: '100%' }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="organization"
              label="Organization"
              name="organization"
              autoComplete="organization"
              value={organization}
              onChange={(e) => setOrganization(e.target.value)}
              disabled={isSubmitting}
              error={!!error && error.includes('Organization')}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="email"
              label="Email Address"
              name="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={isSubmitting}
              error={!!error && error.includes('email')}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="Password"
              type="password"
              id="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isSubmitting}
              error={!!error && error.includes('Password')}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={isSubmitting}
              sx={{ mt: 3, mb: 2 }}
            >
              {isRegistering ? 'Register' : 'Sign In'}
            </Button>
            <Box sx={{ textAlign: 'center' }}>
              <Link
                component="button"
                variant="body2"
                onClick={() => {
                  setIsRegistering(!isRegistering);
                  setError('');
                }}
                disabled={isSubmitting}
              >
                {isRegistering
                  ? 'Already have an account? Sign in'
                  : "Don't have an account? Register"}
              </Link>
            </Box>
          </Box>
        </TabPanel>

        <TabPanel value={activeTab} index={1}>
          <Box component="form" onSubmit={handleHuggingFaceSubmit} sx={{ width: '100%' }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="hfToken"
              label="HuggingFace Token"
              name="hfToken"
              type="password"
              autoComplete="off"
              value={hfToken}
              onChange={(e) => setHfToken(e.target.value)}
              disabled={isSubmitting}
              error={!!error && error.includes('token')}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="organization"
              label="Organization"
              name="organization"
              autoComplete="off"
              value={organization}
              onChange={(e) => setOrganization(e.target.value)}
              disabled={isSubmitting}
              error={!!error && error.includes('Organization')}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={isSubmitting}
              sx={{ mt: 3, mb: 2 }}
            >
              Sign In with HuggingFace
            </Button>
            <Box sx={{ textAlign: 'center' }}>
              <Link
                href="https://huggingface.co/settings/tokens"
                target="_blank"
                rel="noopener noreferrer"
                variant="body2"
              >
                Get a HuggingFace token
              </Link>
            </Box>
          </Box>
        </TabPanel>
      </Paper>
    </Container>
  );
};

export default Login;
