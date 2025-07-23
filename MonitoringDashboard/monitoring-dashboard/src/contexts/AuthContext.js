import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    // Check for existing session on app start
    checkAuthStatus();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const token = localStorage.getItem('auth-token');
      if (token) {
        // Here you would validate token with your .NET Core API
        // const response = await fetch('/api/auth/validate', {
        //   headers: { Authorization: `Bearer ${token}` }
        // });
        
        // For demo, simulate token validation
        const userData = JSON.parse(localStorage.getItem('user-data') || '{}');
        if (userData.username) {
          setUser(userData);
          setIsAuthenticated(true);
        }
      }
    } catch (error) {
      console.error('Auth check failed:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (username, password) => {
    try {
      // Replace with actual API call to your .NET Core backend
      // const response = await fetch('/api/auth/login', {
      //   method: 'POST',
      //   headers: { 'Content-Type': 'application/json' },
      //   body: JSON.stringify({ username, password })
      // });
      
      // For demo purposes, simulate login
      await new Promise(resolve => setTimeout(resolve, 1500));
      
      if (username && password) {
        const userData = {
          id: 1,
          username,
          email: `${username}@company.com`,
          role: 'admin',
          permissions: ['dashboard', 'users', 'monitoring']
        };
        
        const token = 'demo-jwt-token-' + Date.now();
        
        localStorage.setItem('auth-token', token);
        localStorage.setItem('user-data', JSON.stringify(userData));
        
        setUser(userData);
        setIsAuthenticated(true);
        return { success: true };
      } else {
        throw new Error('Invalid credentials');
      }
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const logout = () => {
    localStorage.removeItem('auth-token');
    localStorage.removeItem('user-data');
    setUser(null);
    setIsAuthenticated(false);
  };

  const value = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};