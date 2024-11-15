import { useSnackbar } from 'notistack';
import { useState, useCallback } from 'react';

interface ErrorState {
  message: string;
  code?: string;
  details?: unknown;
}

export const useErrorHandler = () => {
  const [error, setError] = useState<ErrorState | null>(null);
  const { enqueueSnackbar } = useSnackbar();

  const handleError = useCallback((error: unknown) => {
    let errorState: ErrorState;

    if (error instanceof Error) {
      errorState = {
        message: error.message,
        details: error,
      };
    } else if (typeof error === 'string') {
      errorState = {
        message: error,
      };
    } else {
      errorState = {
        message: 'An unknown error occurred',
        details: error,
      };
    }

    setError(errorState);
    enqueueSnackbar(errorState.message, { 
      variant: 'error',
      anchorOrigin: { vertical: 'top', horizontal: 'center' }
    });

    if (import.meta.env.MODE === 'development') {
      console.error('Error caught by useErrorHandler:', error);
    }
  }, [enqueueSnackbar]);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    error,
    handleError,
    clearError,
  };
};