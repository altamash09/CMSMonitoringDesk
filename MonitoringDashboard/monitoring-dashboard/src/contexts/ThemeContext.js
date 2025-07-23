import React, { createContext, useContext, useState } from 'react';

const ThemeContext = createContext();

export const themes = {
  default: {
    name: 'Classic Gray',
    bgClasses: 'from-gray-100 via-slate-200 to-gray-300',
    cardBgClasses: 'from-white via-gray-50 to-slate-100',
    primaryClasses: 'from-gray-700 to-gray-800 hover:from-gray-800 hover:to-gray-900',
    accentClasses: 'from-gray-400 via-slate-500 to-gray-600',
    iconBgClasses: 'from-gray-700 to-gray-800',
    floatingClasses: ['from-gray-300 to-gray-400', 'from-slate-300 to-slate-400', 'from-gray-400 to-gray-500'],
    bulletColor: '#6b7280',
    navBgClasses: 'from-gray-800 to-gray-900',
    navItemClasses: 'hover:bg-gray-700'
  },
  ocean: {
    name: 'Ocean Blue',
    bgClasses: 'from-blue-100 via-cyan-200 to-blue-300',
    cardBgClasses: 'from-white via-blue-50 to-cyan-100',
    primaryClasses: 'from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800',
    accentClasses: 'from-blue-400 via-cyan-500 to-blue-600',
    iconBgClasses: 'from-blue-600 to-blue-700',
    floatingClasses: ['from-blue-300 to-blue-400', 'from-cyan-300 to-cyan-400', 'from-blue-400 to-blue-500'],
    bulletColor: '#3b82f6',
    navBgClasses: 'from-blue-800 to-blue-900',
    navItemClasses: 'hover:bg-blue-700'
  },
  sunset: {
    name: 'Sunset Orange',
    bgClasses: 'from-orange-100 via-amber-200 to-orange-300',
    cardBgClasses: 'from-white via-orange-50 to-amber-100',
    primaryClasses: 'from-orange-600 to-red-600 hover:from-orange-700 hover:to-red-700',
    accentClasses: 'from-orange-400 via-amber-500 to-orange-600',
    iconBgClasses: 'from-orange-600 to-red-600',
    floatingClasses: ['from-orange-300 to-orange-400', 'from-amber-300 to-amber-400', 'from-orange-400 to-orange-500'],
    bulletColor: '#ea580c',
    navBgClasses: 'from-orange-800 to-red-900',
    navItemClasses: 'hover:bg-orange-700'
  },
  nature: {
    name: 'Nature Green',
    bgClasses: 'from-green-100 via-emerald-200 to-green-300',
    cardBgClasses: 'from-white via-green-50 to-emerald-100',
    primaryClasses: 'from-green-600 to-emerald-700 hover:from-green-700 hover:to-emerald-800',
    accentClasses: 'from-green-400 via-emerald-500 to-green-600',
    iconBgClasses: 'from-green-600 to-emerald-700',
    floatingClasses: ['from-green-300 to-green-400', 'from-emerald-300 to-emerald-400', 'from-green-400 to-green-500'],
    bulletColor: '#059669',
    navBgClasses: 'from-green-800 to-emerald-900',
    navItemClasses: 'hover:bg-green-700'
  }
};

export const ThemeProvider = ({ children }) => {
  const [currentTheme, setCurrentTheme] = useState('default');
  
  const theme = themes[currentTheme];
  
  const changeTheme = (themeName) => {
    setCurrentTheme(themeName);
    // Optional: Save to localStorage
    localStorage.setItem('monitoring-theme', themeName);
  };
  
  // Load theme from localStorage on init
  React.useEffect(() => {
    const savedTheme = localStorage.getItem('monitoring-theme');
    if (savedTheme && themes[savedTheme]) {
      setCurrentTheme(savedTheme);
    }
  }, []);
  
  return (
    <ThemeContext.Provider value={{ theme, currentTheme, changeTheme, themes }}>
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};