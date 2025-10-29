import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

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
        // Validate token with your .NET Core API
        const response = await fetch(`${API_BASE_URL}/auth/validate-token`, {
          method: 'POST',
          headers: { 
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(token)
        });
        
        if (response.ok) {
          const userData = JSON.parse(localStorage.getItem('user-data') || '{}');
          if (userData.username) {
            setUser(userData);
            setIsAuthenticated(true);
          }
        } else {
          // Token is invalid, clear it
          localStorage.removeItem('auth-token');
          localStorage.removeItem('user-data');
        }
      }
    } catch (error) {
      console.error('Auth check failed:', error);
      // Clear invalid data
      localStorage.removeItem('auth-token');
      localStorage.removeItem('user-data');
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (username, password) => {
    try {
      setIsLoading(true);
      
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify({ 
          username, 
          password,
          rememberMe: false 
        })
      });
  
      const data = await response.json();
      
      if (response.ok && data.success && data.data) {
        // Parse permissions from JSON string to array
        const userData = {
          ...data.data.user,
          permissions: JSON.parse(data.data.user.permissions || '[]')
        };
        
        // Store token and user data
        localStorage.setItem('auth-token', data.data.token);
        localStorage.setItem('user-data', JSON.stringify(userData));
        
        setUser(userData);
        setIsAuthenticated(true);
        
        console.log('Login successful:', userData);
        return { success: true };
      } else {
        const errorMessage = data.message || 'Invalid credentials';
        console.error('Login failed:', errorMessage);
        return { success: false, error: errorMessage };
      }
    } catch (error) {
      console.error('Login error:', error);
      return { 
        success: false, 
        error: 'Connection failed. Please check if the API is running.' 
      };
    } finally {
      setIsLoading(false);
    }
  };

  const logout = async () => {
    try {
      const token = localStorage.getItem('auth-token');
      if (token) {
        // Call logout endpoint
        await fetch(`${API_BASE_URL}/auth/logout`, {
          method: 'POST',
          headers: { 
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      // Always clear local storage
      localStorage.removeItem('auth-token');
      localStorage.removeItem('user-data');
      setUser(null);
      setIsAuthenticated(false);
    }
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