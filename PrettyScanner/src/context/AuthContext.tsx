import React, { createContext, useContext, useState, useEffect } from 'react';
import {
  signInWithPopup,
  GoogleAuthProvider,
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  User,
} from 'firebase/auth';
import { auth } from '../config/firebase';
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
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [organization, setOrganization] = useState<Organization | null>(null);
  const [organizations, setOrganizations] = useState<Organization[]>([]);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (user: User | null) => {
      setUser(user);
      if (user) {
        await loadOrganizations(user);
      } else {
        setOrganizations([]);
        setOrganization(null);
        localStorage.removeItem('currentOrganizationId');
      }
    });

    return () => unsubscribe();
  }, []);

  const loadOrganizations = async (user: User) => {
    try {
      const response = await fetch('api/organizations', {
        headers: {
          Authorization: `Bearer ${await user.getIdToken()}`
        }
      });
      if (response.ok) {
        const orgs: Organization[] = await response.json();
        setOrganizations(orgs);
        
        // Load current organization from localStorage or use first available
        const savedOrgId = localStorage.getItem('currentOrganizationId');
        if (savedOrgId) {
          const currentOrg = orgs.find(org => org.id === parseInt(savedOrgId));
          if (currentOrg) {
            setOrganization(currentOrg);
          }
        } else if (orgs.length > 0) {
          setOrganization(orgs[0]);
          if (orgs[0].id !== null && orgs[0].id !== undefined) {
            localStorage.setItem('currentOrganizationId', orgs[0].id.toString());
          }
        }
      }
    } catch (error) {
      console.error('Failed to load organizations:', error);
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
      console.error('Google login failed:', error);
      throw error;
    }
  };

  const loginWithEmail = async (email: string, password: string) => {
    try {
      const result = await signInWithEmailAndPassword(auth, email, password);
      setUser(result.user);
      await loadOrganizations(result.user);
    } catch (error) {
      console.error('Email login failed:', error);
      throw error;
    }
  };

  const registerWithEmail = async (email: string, password: string, organizationName: string) => {
    try {
      const result = await createUserWithEmailAndPassword(auth, email, password);
      setUser(result.user);

      // Create organization
      const orgResponse = await fetch('api/organizations', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${await result.user.getIdToken()}`
        },
        body: JSON.stringify({ name: organizationName })
      });

      if (orgResponse.ok) {
        const org: Organization = await orgResponse.json();
        setOrganization(org);
        setOrganizations([org]);
        if (org.id !== null && org.id !== undefined) {
          localStorage.setItem('currentOrganizationId', org.id.toString());
        }
      } else {
        throw new Error('Failed to create organization');
      }
    } catch (error) {
      console.error('Email registration failed:', error);
      throw error;
    }
  };

  const loginWithHuggingFace = async (token: string, org: Organization) => {
    try {
      // Validate token by making a request to HuggingFace
      const response = await fetch('https://huggingface.co/api/whoami', {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      
      if (!response.ok) {
        throw new Error('Invalid token');
      }

      // Store the token and organization
      localStorage.setItem('hf_token', token);
      setOrganization(org);
      setOrganizations([org]);
      if (org.id !== null && org.id !== undefined
          && org.id !== organization?.id) {
        localStorage.setItem('currentOrganizationId', org.id.toString());
      }

      // Create a custom user object since we're not using Firebase auth for HuggingFace
      const hfUserData = await response.json();
      const customUser = {
        uid: `hf_${hfUserData.id}`,
        email: hfUserData.email,
        displayName: hfUserData.name,
        providerId: 'huggingface.co',
        getIdToken: async () => token, // Add this to match Firebase User interface
      } as unknown as User;
      
      setUser(customUser);
    } catch (error) {
      console.error('HuggingFace login failed:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await signOut(auth);
      localStorage.removeItem('hf_token');
      localStorage.removeItem('currentOrganizationId');
      setUser(null);
      setOrganization(null);
      setOrganizations([]);
    } catch (error) {
      console.error('Logout failed:', error);
      throw error;
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