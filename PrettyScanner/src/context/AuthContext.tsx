import React, { createContext, useContext, useState, useEffect } from 'react';
import {
  signInWithPopup,
  GoogleAuthProvider,
  OAuthProvider,
  signInWithEmailAndPassword,
  createUserWithEmailAndPassword,
  signOut,
  onAuthStateChanged,
  User,
} from 'firebase/auth';
import { auth } from '../config/firebase';

interface AuthContextType {
  user: User | null;
  organization: string | null;
  loginWithGoogle: () => Promise<void>;
  loginWithApple: () => Promise<void>;
  loginWithEmail: (email: string, password: string) => Promise<void>;
  registerWithEmail: (email: string, password: string) => Promise<void>;
  loginWithHuggingFace: (token: string, org: string) => Promise<void>;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [organization, setOrganization] = useState<string | null>(null);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, (user) => {
      setUser(user);
      // Get organization from user's claims or localStorage
      const org = localStorage.getItem('organization');
      setOrganization(org);
    });

    return () => unsubscribe();
  }, []);

  const loginWithGoogle = async () => {
    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(auth, provider);
      setUser(result.user);
      // You might want to handle organization selection after successful login
    } catch (error) {
      console.error('Google login failed:', error);
      throw error;
    }
  };

  const loginWithApple = async () => {
    try {
      const provider = new OAuthProvider('apple.com');
      const result = await signInWithPopup(auth, provider);
      setUser(result.user);
      // You might want to handle organization selection after successful login
    } catch (error) {
      console.error('Apple login failed:', error);
      throw error;
    }
  };

  const loginWithEmail = async (email: string, password: string) => {
    try {
      const result = await signInWithEmailAndPassword(auth, email, password);
      setUser(result.user);
      // You might want to handle organization selection after successful login
    } catch (error) {
      console.error('Email login failed:', error);
      throw error;
    }
  };

  const registerWithEmail = async (email: string, password: string) => {
    try {
      const result = await createUserWithEmailAndPassword(auth, email, password);
      setUser(result.user);
      // You might want to handle organization selection after successful registration
    } catch (error) {
      console.error('Email registration failed:', error);
      throw error;
    }
  };

  const loginWithHuggingFace = async (token: string, org: string) => {
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
      localStorage.setItem('organization', org);
      setOrganization(org);

      // Create a custom user object since we're not using Firebase auth for HuggingFace
      const hfUserData = await response.json();
      const customUser = {
        uid: `hf_${hfUserData.id}`,
        email: hfUserData.email,
        displayName: hfUserData.name,
        providerId: 'huggingface.co',
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
      localStorage.removeItem('organization');
      setUser(null);
      setOrganization(null);
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
        loginWithGoogle,
        loginWithApple,
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
