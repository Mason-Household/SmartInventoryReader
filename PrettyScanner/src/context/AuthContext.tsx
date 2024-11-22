import React, { createContext, useContext, useState, useEffect } from 'react';
import {
  signInWithPopup,
  GoogleAuthProvider,
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  User,
  AuthError,
} from 'firebase/auth';
import { auth } from '../../config/firebase';
import { Organization } from '../interfaces/Organization';

interface AuthContextType {
  user: User | null;
  organization: Organization | null;
  organizations: Organization[];
  setCurrentOrganization: (org: Organization) => void;
  loginWithGoogle: () => Promise<void>;
  loginWithEmail: (email: string, password: string) => Promise<void>;
  registerWithEmail: (email: string, password: string, organizationName: string) => Promise<void>;
  loginWithHuggingFace: (token: string, org: Organization) => Promise<void>;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const API_URL = import.meta.env.VITE_LLM_SERVICE_URL || 'http://localhost:5000';
const HF_API_URL = 'https://huggingface.co/api';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [organization, setOrganization] = useState<Organization | null>(null);
  const [organizations, setOrganizations] = useState<Organization[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (user: User | null) => {
      setUser(user);
      if (user) {
        try {
          await loadOrganizations(user);
        } catch (error) {
          console.error('Failed to load organizations:', error);
        }
      } else {
        clearLocalStorage();
      }
      setIsLoading(false);
    });

    return () => unsubscribe();
  }, []);

  const clearLocalStorage = () => {
    setOrganizations([]);
    setOrganization(null);
    localStorage.removeItem('currentOrganizationId');
    localStorage.removeItem('hf_token');
  };

  const handleAuthError = (error: unknown) => {
    if (error instanceof Error) {
      if ((error as AuthError).code === 'auth/network-request-failed') {
        throw new Error('Network error. Please check your connection.');
      }
      if ((error as AuthError).code === 'auth/invalid-credential') {
        throw new Error('Invalid credentials. Please check your email and password.');
      }
      throw error;
    }
    throw new Error('An unexpected error occurred');
  };

  const loadOrganizations = async (user: User) => {
    try {
      const token = await user.getIdToken();
      const response = await fetch(`${API_URL}/api/organizations`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to load organizations: ${response.statusText}`);
      }

      const orgs: Organization[] = await response.json();
      setOrganizations(orgs);
      
      const savedOrgId = localStorage.getItem('currentOrganizationId');
      if (savedOrgId) {
        const currentOrg = orgs.find(org => org.id === parseInt(savedOrgId));
        if (currentOrg) {
          setOrganization(currentOrg);
        } else if (orgs.length > 0) {
          setCurrentOrganization(orgs[0]);
        }
      } else if (orgs.length > 0) {
        setCurrentOrganization(orgs[0]);
      }
    } catch (error) {
      console.error('Failed to load organizations:', error);
      throw new Error('Failed to load organizations. Please try again later.');
    }
  };

  const setCurrentOrganization = (org: Organization) => {
    setOrganization(org);
    if (org.id !== null && org.id !== undefined) {
      localStorage.setItem('currentOrganizationId', org.id.toString());
    }
  };

  const loginWithGoogle = async () => {
    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(auth, provider);
      setUser(result.user);
      await loadOrganizations(result.user);
    } catch (error) {
      handleAuthError(error);
    }
  };

  const loginWithEmail = async (email: string, password: string) => {
    try {
      const result = await signInWithEmailAndPassword(auth, email, password);
      setUser(result.user);
      await loadOrganizations(result.user);
    } catch (error) {
      handleAuthError(error);
    }
  };

  const registerWithEmail = async (email: string, password: string, organizationName: string) => {
    try {
      const result = await createUserWithEmailAndPassword(auth, email, password);
      setUser(result.user);

      const token = await result.user.getIdToken();
      const orgResponse = await fetch(`${API_URL}/api/organizations`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ 
          name: organizationName,
          slug: organizationName.toLowerCase().replace(/\s+/g, '-'),
          isActive: true,
        }),
      });

      if (!orgResponse.ok) {
        throw new Error('Failed to create organization');
      }

      const org: Organization = await orgResponse.json();
      setOrganization(org);
      setOrganizations([org]);
      if (org.id !== null && org.id !== undefined) {
        localStorage.setItem('currentOrganizationId', org.id.toString());
      }
    } catch (error) {
      handleAuthError(error);
    }
  };

  const loginWithHuggingFace = async (token: string, org: Organization) => {
    try {
      const response = await fetch(`${HF_API_URL}/whoami`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });
      
      if (!response.ok) {
        throw new Error('Invalid HuggingFace token');
      }

      const hfUserData = await response.json();
      
      // Validate the response
      if (!hfUserData.id || !hfUserData.email) {
        throw new Error('Invalid response from HuggingFace API');
      }

      // Store token securely
      localStorage.setItem('hf_token', token);
      
      // Set organization
      setOrganization(org);
      setOrganizations([org]);
      if (org.id !== null && org.id !== undefined) {
        localStorage.setItem('currentOrganizationId', org.id.toString());
      }

      // Create custom user object
      const customUser = {
        uid: `hf_${hfUserData.id}`,
        email: hfUserData.email,
        displayName: hfUserData.name || hfUserData.email,
        providerId: 'huggingface.co',
        getIdToken: async () => token,
      } as unknown as User;
      
      setUser(customUser);
    } catch (error) {
      if (error instanceof Error) {
        if (error.message.includes('Failed to fetch')) {
          throw new Error('Unable to connect to HuggingFace. Please check your network connection.');
        }
        throw error;
      }
      throw new Error('Failed to authenticate with HuggingFace');
    }
  };

  const logout = async () => {
    try {
      await signOut(auth);
      clearLocalStorage();
    } catch (error) {
      console.error('Logout failed:', error);
      throw new Error('Failed to logout. Please try again.');
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        organization,
        organizations,
        setCurrentOrganization,
        loginWithGoogle,
        loginWithEmail,
        registerWithEmail,
        loginWithHuggingFace,
        logout,
        isAuthenticated: !!user,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
