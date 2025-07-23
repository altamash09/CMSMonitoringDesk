import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider } from './contexts/ThemeContext';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/common/ProtectedRoute';
import LoginPage from './components/auth/LoginPage';
import MonitoringDashboard from './components/dashboard/MonitoringDashboard';
import UserManagement from './components/dashboard/UserManagement';

// Placeholder components for other routes
const Analytics = () => (
  <div className="min-h-screen bg-gradient-to-br from-gray-100 via-slate-200 to-gray-300 flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-3xl font-bold text-gray-800 mb-4">Analytics Dashboard</h1>
      <p className="text-gray-600">Coming Soon...</p>
    </div>
  </div>
);

const Reports = () => (
  <div className="min-h-screen bg-gradient-to-br from-gray-100 via-slate-200 to-gray-300 flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-3xl font-bold text-gray-800 mb-4">Reports</h1>
      <p className="text-gray-600">Coming Soon...</p>
    </div>
  </div>
);

const Notifications = () => (
  <div className="min-h-screen bg-gradient-to-br from-gray-100 via-slate-200 to-gray-300 flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-3xl font-bold text-gray-800 mb-4">Notifications</h1>
      <p className="text-gray-600">Coming Soon...</p>
    </div>
  </div>
);

const Security = () => (
  <div className="min-h-screen bg-gradient-to-br from-gray-100 via-slate-200 to-gray-300 flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-3xl font-bold text-gray-800 mb-4">Security Settings</h1>
      <p className="text-gray-600">Coming Soon...</p>
    </div>
  </div>
);

const Settings = () => (
  <div className="min-h-screen bg-gradient-to-br from-gray-100 via-slate-200 to-gray-300 flex items-center justify-center">
    <div className="text-center">
      <h1 className="text-3xl font-bold text-gray-800 mb-4">Settings</h1>
      <p className="text-gray-600">Coming Soon...</p>
    </div>
  </div>
);

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <Router>
          <div className="App">
            <Routes>
              {/* Public routes */}
              <Route path="/login" element={<LoginPage />} />
              
              {/* Protected routes */}
              <Route
                path="/dashboard"
                element={
                  <ProtectedRoute requiredPermissions={['dashboard']}>
                    <MonitoringDashboard />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/users"
                element={
                  <ProtectedRoute requiredPermissions={['users']}>
                    <UserManagement />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/analytics"
                element={
                  <ProtectedRoute>
                    <Analytics />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/reports"
                element={
                  <ProtectedRoute>
                    <Reports />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/notifications"
                element={
                  <ProtectedRoute>
                    <Notifications />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/security"
                element={
                  <ProtectedRoute requiredPermissions={['security']}>
                    <Security />
                  </ProtectedRoute>
                }
              />
              
              <Route
                path="/settings"
                element={
                  <ProtectedRoute>
                    <Settings />
                  </ProtectedRoute>
                }
              />
              
              {/* Default redirects */}
              <Route path="/" element={<Navigate to="/dashboard" replace />} />
              <Route path="*" element={<Navigate to="/dashboard" replace />} />
            </Routes>
          </div>
        </Router>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;