import { CssBaseline } from '@mui/material';
import { SnackbarProvider } from 'notistack';
import { AnimatePresence } from 'framer-motion';
import { ThemeProvider } from './theme/ThemeContext';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout/Layout';
import Scanner from './components/Scanner/Scanner';
import Login from './components/Login/Login';
import ErrorBoundary from './components/Errors/ErrorBoundary';

const AppContent = () => {
  const { isAuthenticated } = useAuth();

  return (
    <AnimatePresence mode="wait">
      {isAuthenticated ? (
        <Layout>
          <Scanner />
        </Layout>
      ) : (
        <Login />
      )}
    </AnimatePresence>
  );
};

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider>
        <AuthProvider>
          <SnackbarProvider 
            maxSnack={3}
            autoHideDuration={3000}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'center',
            }}
          >
            <CssBaseline />
            <AppContent />
          </SnackbarProvider>
        </AuthProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
