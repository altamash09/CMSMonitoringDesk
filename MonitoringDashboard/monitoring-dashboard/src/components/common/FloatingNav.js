import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { 
  Menu, 
  X, 
  Monitor, 
  Users, 
  Settings, 
  BarChart3, 
  Shield, 
  LogOut,
  Home,
  Bell,
  FileText
} from 'lucide-react';
import { useTheme } from '../../contexts/ThemeContext';
import { useAuth } from '../../contexts/AuthContext';
import clsx from 'clsx';

const FloatingNav = () => {
  const [isExpanded, setIsExpanded] = useState(false);
  const { theme } = useTheme();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const menuItems = [
    { 
      id: 'dashboard', 
      label: 'Dashboard', 
      icon: Monitor, 
      path: '/dashboard',
      color: 'text-blue-400'
    },
    { 
      id: 'users', 
      label: 'User Management', 
      icon: Users, 
      path: '/users',
      color: 'text-green-400'
    },
    { 
      id: 'analytics', 
      label: 'Analytics', 
      icon: BarChart3, 
      path: '/analytics',
      color: 'text-purple-400'
    },
    { 
      id: 'reports', 
      label: 'Reports', 
      icon: FileText, 
      path: '/reports',
      color: 'text-orange-400'
    },
    { 
      id: 'notifications', 
      label: 'Notifications', 
      icon: Bell, 
      path: '/notifications',
      color: 'text-yellow-400'
    },
    { 
      id: 'security', 
      label: 'Security', 
      icon: Shield, 
      path: '/security',
      color: 'text-red-400'
    },
    { 
      id: 'settings', 
      label: 'Settings', 
      icon: Settings, 
      path: '/settings',
      color: 'text-gray-400'
    }
  ];

  const handleNavigation = (path) => {
    navigate(path);
    setIsExpanded(false);
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
    setIsExpanded(false);
  };

  const toggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  const isActivePath = (path) => {
    return location.pathname === path;
  };

  return (
    <>
      {/* Backdrop */}
      {isExpanded && (
        <div 
          className="fixed inset-0 bg-black/20 backdrop-blur-sm z-40 transition-opacity duration-300"
          onClick={() => setIsExpanded(false)}
        />
      )}

      {/* Floating Navigation */}
      <div className="fixed bottom-6 right-6 z-50">
        {/* Main Menu Button */}
        <div className="relative">
          {/* Menu Items */}
          <div className={clsx(
            'absolute bottom-16 right-0 transition-all duration-500 ease-out transform',
            isExpanded 
              ? 'translate-y-0 opacity-100 scale-100 pointer-events-auto' 
              : 'translate-y-8 opacity-0 scale-95 pointer-events-none'
          )}>
            <div className={`bg-gradient-to-br ${theme.navBgClasses} backdrop-blur-xl rounded-2xl p-3 shadow-2xl border border-white/10 min-w-[280px]`}>
              {/* Header */}
              <div className="flex items-center justify-between mb-3 pb-3 border-b border-white/10">
                <h3 className="text-white font-bold text-sm">Navigation Menu</h3>
                <button
                  onClick={() => setIsExpanded(false)}
                  className="text-white/60 hover:text-white transition-colors p-1 hover:bg-white/10 rounded-lg"
                >
                  <X className="w-4 h-4" />
                </button>
              </div>

              {/* Menu Items */}
              <div className="space-y-1 mb-3">
                {menuItems.map((item, index) => {
                  const IconComponent = item.icon;
                  const isActive = isActivePath(item.path);
                  
                  return (
                    <button
                      key={item.id}
                      onClick={() => handleNavigation(item.path)}
                      className={clsx(
                        'w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-200',
                        isActive 
                          ? 'bg-white/20 text-white shadow-lg' 
                          : `text-white/80 hover:text-white ${theme.navItemClasses}`,
                        'transform hover:scale-105 hover:shadow-lg'
                      )}
                      style={{ 
                        animationDelay: `${index * 50}ms`,
                        animation: isExpanded ? 'slideInFromRight 0.3s ease-out' : ''
                      }}
                    >
                      <div className={clsx(
                        'p-1.5 rounded-lg transition-colors',
                        isActive ? 'bg-white/20' : 'bg-white/10'
                      )}>
                        <IconComponent className={clsx('w-4 h-4', isActive ? 'text-white' : item.color)} />
                      </div>
                      <span className="font-medium text-sm">{item.label}</span>
                      {isActive && (
                        <div className="ml-auto w-2 h-2 bg-white rounded-full animate-pulse" />
                      )}
                    </button>
                  );
                })}
              </div>

              {/* Logout Button */}
              <div className="pt-3 border-t border-white/10">
                <button
                  onClick={handleLogout}
                  className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-200 text-red-300 hover:text-red-200 hover:bg-red-500/20"
                >
                  <div className="p-1.5 rounded-lg bg-red-500/20">
                    <LogOut className="w-4 h-4" />
                  </div>
                  <span className="font-medium text-sm">Logout</span>
                </button>
              </div>
            </div>
          </div>

          {/* Main Button */}
          <button
            onClick={toggleExpanded}
            className={clsx(
              'relative w-14 h-14 rounded-2xl shadow-2xl transition-all duration-300 group',
              `bg-gradient-to-br ${theme.iconBgClasses}`,
              'hover:shadow-3xl hover:scale-110 active:scale-95',
              isExpanded && 'shadow-3xl scale-110'
            )}
          >
            <div className="absolute inset-0 rounded-2xl bg-white/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
            
            {/* Icon */}
            <div className="absolute inset-0 flex items-center justify-center">
              <div className={clsx(
                'transition-transform duration-300',
                isExpanded ? 'rotate-180 scale-110' : 'rotate-0 scale-100'
              )}>
                {isExpanded ? (
                  <X className="w-6 h-6 text-white" />
                ) : (
                  <Menu className="w-6 h-6 text-white" />
                )}
              </div>
            </div>

            {/* Ripple Effect */}
            <div className={clsx(
              'absolute inset-0 rounded-2xl transition-all duration-700',
              'animate-ping opacity-20',
              `bg-gradient-to-br ${theme.iconBgClasses}`,
              isExpanded ? 'scale-150 opacity-30' : 'scale-100 opacity-0'
            )} />
            
            {/* Pulse Ring */}
            <div className="absolute -inset-2 rounded-3xl bg-white/5 animate-pulse opacity-40" />
          </button>

          {/* Notification Badge (optional) */}
          <div className="absolute -top-1 -right-1 w-5 h-5 bg-red-500 rounded-full flex items-center justify-center animate-bounce">
            <span className="text-white text-xs font-bold">3</span>
          </div>
        </div>
      </div>

      <style jsx>{`
        @keyframes slideInFromRight {
          from {
            transform: translateX(20px);
            opacity: 0;
          }
          to {
            transform: translateX(0);
            opacity: 1;
          }
        }
      `}</style>
    </>
  );
};

export default FloatingNav;