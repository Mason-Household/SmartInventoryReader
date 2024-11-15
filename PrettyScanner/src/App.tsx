import { CssBaseline } from '@mui/material';
import { SnackbarProvider } from 'notistack';
import { AnimatePresence } from 'framer-motion';
import { ThemeProvider } from './theme/ThemeContext';
import Layout from './components/Layout/Layout';
import Scanner from './components/Scanner/Scanner';
import ErrorBoundary from './components/Errors/ErrorBoundary';

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider>
        <SnackbarProvider 
          maxSnack={3}
          autoHideDuration={3000}
          anchorOrigin={{
            vertical: 'top',
            horizontal: 'center',
          }}
        >
          <CssBaseline />
          <AnimatePresence mode="wait">
            <Layout>
              <Scanner />
            </Layout>
          </AnimatePresence>
        </SnackbarProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;