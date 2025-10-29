import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Eye, EyeOff, User, Lock, Monitor, Shield, ChevronRight, Palette } from 'lucide-react';
import { useTheme } from '../../contexts/ThemeContext';
import { useAuth } from '../../contexts/AuthContext';

const LoginPage = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    rememberMe: false
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const { theme, currentTheme, changeTheme, themes } = useTheme();
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
    if (error) setError(''); // Clear error when user types
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    console.log("API URL:", process.env.REACT_APP_API_URL);


    try {
      const result = await login(formData.username, formData.password);
      if (result.success) {
        navigate('/dashboard');
      } else {
        setError(result.error || 'Login failed');
      }
    } catch (err) {
      setError('An unexpected error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const handleThemeChange = (themeName) => {
    changeTheme(themeName);
  };

  return (
    <div className={`min-h-screen bg-gradient-to-br ${theme.bgClasses} flex items-center justify-center p-6 transition-all duration-700`}>
      {/* Theme Selector */}
      <div className="absolute top-6 right-6 flex items-center gap-3 z-10">
        <div className="flex items-center gap-2 bg-white/20 backdrop-blur-sm rounded-full p-3 border border-white/30 shadow-lg">
          <Palette className="w-4 h-4 text-gray-700 mr-1" />
          {Object.entries(themes).map(([key, themeConfig]) => (
            <button
              key={key}
              onClick={() => handleThemeChange(key)}
              className={`w-7 h-7 rounded-full transition-all duration-300 hover:scale-110 border-2 relative ${
                currentTheme === key ? 'border-white shadow-lg scale-110' : 'border-white/50 opacity-80 hover:opacity-100'
              }`}
              style={{
                background: `linear-gradient(135deg, ${themeConfig.bulletColor}, ${themeConfig.bulletColor}dd)`
              }}
              title={themeConfig.name}
            >
              {currentTheme === key && (
                <div className="absolute inset-0 flex items-center justify-center">
                  <div className="w-2 h-2 bg-white rounded-full animate-pulse"></div>
                </div>
              )}
            </button>
          ))}
        </div>
      </div>

      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-5">
        <div className="absolute inset-0" style={{
          backgroundImage: `radial-gradient(circle at 25% 25%, #374151 2px, transparent 2px),
                           radial-gradient(circle at 75% 75%, #6b7280 2px, transparent 2px)`,
          backgroundSize: '50px 50px'
        }}></div>
      </div>

      {/* Login Container */}
      <div className="relative w-full max-w-md">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="flex justify-center mb-4">
            <div className={`p-4 bg-gradient-to-br ${theme.iconBgClasses} rounded-2xl shadow-2xl transition-all duration-500`}>
              <Monitor className="w-8 h-8 text-white" />
            </div>
          </div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">
            CMS Monitoring Desk
          </h1>
          <p className="text-gray-600 font-medium">
          Your gateway to operational oversight
          </p>
          <div className="flex justify-center mt-3">
            <div className={`h-1 w-20 bg-gradient-to-r ${theme.accentClasses} rounded-full transition-all duration-500`}></div>
          </div>
        </div>

        {/* Login Form */}
        <div className={`bg-gradient-to-br ${theme.cardBgClasses} backdrop-blur-sm rounded-3xl p-8 border-2 border-gray-200 shadow-2xl transition-all duration-500`}>
          {error && (
            <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded-lg text-sm">
              {error}
            </div>
          )}
          
          <div className="space-y-6">
            {/* Username Field */}
            <div>
              <label className="block text-sm font-bold text-gray-700 mb-2">
                Username
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <User className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="text"
                  name="username"
                  value={formData.username}
                  onChange={handleInputChange}
                  className="w-full pl-10 pr-4 py-3 border-2 border-gray-200 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-transparent bg-white/80 backdrop-blur-sm transition-all duration-300 text-gray-800 font-medium placeholder-gray-400"
                  placeholder="Enter your username"
                  required
                />
              </div>
            </div>

            {/* Password Field */}
            <div>
              <label className="block text-sm font-bold text-gray-700 mb-2">
                Password
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type={showPassword ? 'text' : 'password'}
                  name="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  className="w-full pl-10 pr-12 py-3 border-2 border-gray-200 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-transparent bg-white/80 backdrop-blur-sm transition-all duration-300 text-gray-800 font-medium placeholder-gray-400"
                  placeholder="Enter your password"
                  required
                />
                <button
                  type="button"
                  onClick={togglePasswordVisibility}
                  className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 transition-colors duration-200"
                >
                  {showPassword ? (
                    <EyeOff className="h-5 w-5" />
                  ) : (
                    <Eye className="h-5 w-5" />
                  )}
                </button>
              </div>
            </div>

            {/* Remember Me */}
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <input
                  type="checkbox"
                  name="rememberMe"
                  checked={formData.rememberMe}
                  onChange={handleInputChange}
                  className="h-4 w-4 text-gray-600 focus:ring-gray-500 border-gray-300 rounded transition-all duration-200"
                />
                <label className="ml-2 text-sm font-medium text-gray-700">
                  Remember me
                </label>
              </div>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              onClick={handleSubmit}
              className={`w-full bg-gradient-to-r ${theme.primaryClasses} text-white font-bold py-3 px-6 rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 flex items-center justify-center gap-2 disabled:opacity-70 disabled:cursor-not-allowed`}
            >
              {isLoading ? (
                <>
                  <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                  <span>Signing in...</span>
                </>
              ) : (
                <>
                  <span>Sign In</span>
                  <ChevronRight className="h-5 w-5" />
                </>
              )}
            </button>
          </div>

          {/* Additional Options */}
          <div className="mt-8 pt-6 border-t border-gray-200">
            <div className="text-center">
              <p className="text-sm text-gray-600 mb-3">
                Need help accessing your account?
              </p>
              <button className="inline-flex items-center gap-2 text-sm font-medium text-gray-600 hover:text-gray-800 transition-colors duration-200">
                <Shield className="h-4 w-4" />
                Contact Rebiz Support
              </button>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="text-center mt-8">
          <p className="text-sm text-gray-500">
            © 2025 Camera Monitoring System. All rights reserved.
          </p>
        </div>
      </div>

      {/* Floating Elements */}
      <div className={`absolute top-10 left-10 w-20 h-20 bg-gradient-to-br ${theme.floatingClasses[0]} rounded-full opacity-20 animate-pulse transition-all duration-700`}></div>
      <div className={`absolute bottom-10 right-10 w-16 h-16 bg-gradient-to-br ${theme.floatingClasses[1]} rounded-full opacity-20 animate-pulse transition-all duration-700`} style={{ animationDelay: '1s' }}></div>
      <div className={`absolute top-1/2 left-5 w-12 h-12 bg-gradient-to-br ${theme.floatingClasses[2]} rounded-full opacity-15 animate-pulse transition-all duration-700`} style={{ animationDelay: '2s' }}></div>
    </div>
  );
};

export default LoginPage;