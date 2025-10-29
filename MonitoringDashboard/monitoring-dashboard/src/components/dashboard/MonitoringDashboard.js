import React, { useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Calendar, Users, Monitor, Activity, Clock, CheckCircle, AlertCircle, Maximize, RefreshCw } from 'lucide-react';
import { useTheme } from '../../contexts/ThemeContext';
import { useAuth } from '../../contexts/AuthContext';
import { useDashboardData } from '../../hooks/useDashboardData';
import FloatingNav from '../common/FloatingNav';

const MonitoringDashboard = () => {
  const [selectedDate, setSelectedDate] = useState('2025-07-18');
  const [isBacklog, setIsBacklog] = useState(false);
  const [showAllStats, setShowAllStats] = useState(false);
  
  const { theme } = useTheme();
  const { user } = useAuth();
  
  // Use the dashboard data hook
  const { data, loading, error, lastUpdated, refreshData } = useDashboardData(selectedDate, isBacklog);
  
  // Safely destructure with default values
  const {
    monitoringStats = {},
    hourlyData = [],
    agents = [],
    reviewers = []
  } = data || {};

  // Fallback data structure for development/demo
  const fallbackStats = {
    'Un-Monitored': { count: 45, color: 'from-red-100 to-red-200', mandatory: true },
    'Monitoring In Process': { count: 23, color: 'from-amber-100 to-yellow-200', mandatory: true },
    'Not Ready': { count: 12, color: 'from-gray-100 to-gray-200', mandatory: true },
    'WFR': { count: 8, color: 'from-orange-100 to-orange-200', mandatory: true },
    'Live': { count: 156, color: 'from-emerald-100 to-green-200', mandatory: true },
    'Review In Process': { count: 34, color: 'from-blue-100 to-blue-200', mandatory: true },
    'Rejected': { count: 7, color: 'from-red-200 to-red-300', mandatory: true },
    'Tickets Opened': { count: 28, color: 'from-cyan-100 to-teal-200', mandatory: true },
    'DVR Down': { count: 3, color: 'from-purple-100 to-purple-200', mandatory: false },
    'Archive Datalost': { count: 2, color: 'from-pink-100 to-rose-200', mandatory: false },
    'On-Holiday': { count: 15, color: 'from-indigo-100 to-indigo-200', mandatory: false }
  };

  const fallbackAgents = [
    { id: 1, name: 'John Smith', status: 'online', completed: 45, estimatedHours: 8.5, actualHours: 7.2, rank: 'Diamond' },
    { id: 2, name: 'Sarah Johnson', status: 'online', completed: 52, estimatedHours: 9.0, actualHours: 8.1, rank: 'Platinum' },
    { id: 3, name: 'Mike Chen', status: 'idle', completed: 38, estimatedHours: 7.5, actualHours: 6.8, rank: 'Gold' },
    { id: 4, name: 'Emma Davis', status: 'online', completed: 41, estimatedHours: 8.0, actualHours: 7.5, rank: 'Diamond' },
    { id: 5, name: 'Alex Rodriguez', status: 'offline', completed: 29, estimatedHours: 6.5, actualHours: 5.9, rank: 'Silver' },
    { id: 6, name: 'Lisa Wang', status: 'online', completed: 47, estimatedHours: 8.2, actualHours: 7.8, rank: 'Platinum' }
  ];

  const fallbackReviewers = [
    { id: 1, name: 'Robert Wilson', status: 'online', completed: 28, estimatedHours: 6.5, actualHours: 5.8, rank: 'Diamond' },
    { id: 2, name: 'Jennifer Lee', status: 'online', completed: 32, estimatedHours: 7.0, actualHours: 6.2, rank: 'Platinum' },
    { id: 3, name: 'Mark Taylor', status: 'idle', completed: 25, estimatedHours: 6.0, actualHours: 5.5, rank: 'Gold' },
    { id: 4, name: 'Sophie Anderson', status: 'online', completed: 30, estimatedHours: 6.8, actualHours: 6.0, rank: 'Gold' },
    { id: 5, name: 'Chris Thompson', status: 'offline', completed: 18, estimatedHours: 5.0, actualHours: 4.2, rank: 'Bronze' },
    { id: 6, name: 'Maya Patel', status: 'online', completed: 26, estimatedHours: 6.2, actualHours: 5.7, rank: 'Silver' }
  ];

  // Use API data or fallback data with null checks
  const displayMonitoringStats = (monitoringStats && Object.keys(monitoringStats).length > 0) ? monitoringStats : fallbackStats;
  const displayAgents = (agents && agents.length > 0) ? agents : fallbackAgents;
  const displayReviewers = (reviewers && reviewers.length > 0) ? reviewers : fallbackReviewers;
  const displayHourlyData = (hourlyData && hourlyData.length > 0) ? hourlyData : Array.from({ length: 24 }, (_, i) => ({
    hour: i.toString().padStart(2, '0'),
    completed: Math.floor(Math.random() * 50) + 10,
    percentage: Math.floor(Math.random() * 30) + 75
  }));
  
  // Filter stats based on view mode
  const mandatoryStats = Object.fromEntries(
    Object.entries(displayMonitoringStats).filter(([key, value]) => value.mandatory)
  );
  const displayStats = showAllStats ? displayMonitoringStats : mandatoryStats;

  const toggleFullScreen = () => {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen();
    } else {
      document.exitFullscreen();
    }
  };

  const handleRefresh = async () => {
    await refreshData();
  };

  // Calculate agent stats with null safety
  const agentStats = {
    total: displayAgents.length,
    online: displayAgents.filter(a => a.status === 'online').length,
    idle: displayAgents.filter(a => a.status === 'idle').length,
    offline: displayAgents.filter(a => a.status === 'offline').length
  };

  // Calculate reviewer stats with null safety
  const reviewerStats = {
    total: displayReviewers.length,
    online: displayReviewers.filter(r => r.status === 'online').length,
    idle: displayReviewers.filter(r => r.status === 'idle').length,
    offline: displayReviewers.filter(r => r.status === 'offline').length
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'online': return 'text-emerald-600';
      case 'idle': return 'text-amber-600';
      case 'offline': return 'text-red-600';
      default: return 'text-gray-600';
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'online': return <CheckCircle className="w-5 h-5" />;
      case 'idle': return <Clock className="w-5 h-5" />;
      case 'offline': return <AlertCircle className="w-5 h-5" />;
      default: return <Activity className="w-5 h-5" />;
    }
  };

  const getRankBadge = (rank) => {
    const rankIcons = {
      'Diamond': '💎',
      'Platinum': '⚪',
      'Gold': '🥇',
      'Silver': '🥈',
      'Bronze': '🥉'
    };
    
    return (
      <span 
        className="text-lg"
        title={`${rank} Rank`}
      >
        {rankIcons[rank] || rankIcons['Bronze']}
      </span>
    );
  };

  return (
    <div className={`min-h-screen bg-gradient-to-br ${theme.bgClasses} p-6 transition-all duration-700`}>
      {/* Error Display */}
      {error && (
        <div className="mb-4 p-4 bg-red-100 border-l-4 border-red-500 text-red-700 rounded-lg">
          <div className="flex items-center">
            <AlertCircle className="w-5 h-5 mr-2" />
            <span className="font-medium">Error: </span>
            <span className="ml-1">{error}</span>
            <button
              onClick={handleRefresh}
              className="ml-auto px-3 py-1 bg-red-600 text-white rounded hover:bg-red-700 transition-colors duration-200 text-sm"
            >
              Retry
            </button>
          </div>
        </div>
      )}

      {/* Header */}
      <div className="mb-5 relative">
        <h1 className="text-3xl font-bold text-gray-800 text-center mb-2 tracking-wide">
          Monitoring Dashboard
        </h1>
        <div className="flex justify-center mb-2">
          <div className={`h-1.5 w-32 bg-gradient-to-r ${theme.accentClasses} rounded-full shadow-lg transition-all duration-500`}></div>
        </div>
        
        {/* Welcome Message and Status */}
        {user && (
          <div className="text-center text-gray-600 mb-4">
            <p className="font-medium">Welcome back, <span className="font-bold text-gray-800">{user.username}</span></p>
            <div className="flex items-center justify-center gap-4 mt-2 text-sm">
              <div className="flex items-center gap-2">
                <div className={`w-2 h-2 ${loading ? 'bg-yellow-500 animate-pulse' : error ? 'bg-red-500' : 'bg-green-500'} rounded-full`}></div>
                <span>{loading ? 'Loading...' : error ? 'Connection Error' : 'System Online'}</span>
              </div>
              <span>•</span>
              <span>Last updated: {lastUpdated.toLocaleTimeString()}</span>
            </div>
          </div>
        )}

        {/* Action Buttons */}
        <div className="absolute top-0 right-0 flex items-center gap-3">
          <button
            onClick={handleRefresh}
            disabled={loading}
            className={`p-2.5 rounded-xl shadow-lg transition-all duration-300 hover:shadow-xl ${
              loading 
                ? 'bg-gray-400 cursor-not-allowed' 
                : 'bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white'
            }`}
            title="Refresh Data"
          >
            <RefreshCw className={`w-5 h-5 ${loading ? 'animate-spin' : ''}`} />
          </button>
          <button
            onClick={toggleFullScreen}
            className="bg-gradient-to-br from-gray-200 to-gray-300 hover:from-gray-300 hover:to-gray-400 text-gray-700 hover:text-gray-900 p-2.5 rounded-xl border-2 border-gray-300 shadow-lg transition-all duration-300 hover:shadow-xl"
            title="Toggle Fullscreen"
          >
            <Maximize className="w-5 h-5" />
          </button>
        </div>
      </div>

      {/* Main Grid Layout */}
      <div className="grid grid-cols-4 gap-5 h-[calc(100vh-120px)]">
        {/* Statistics Cards Section */}
        <div className={`col-span-2 bg-gradient-to-br ${theme.cardBgClasses} backdrop-blur-sm rounded-3xl p-5 border-2 border-gray-200 shadow-2xl transition-all duration-500`}>
          <div className="flex items-center justify-between mb-5">
            <h2 className="text-lg font-bold text-gray-800 flex items-center gap-2">
              <div className={`p-1.5 bg-gradient-to-br ${theme.iconBgClasses} rounded-lg shadow-lg transition-all duration-500`}>
                <Monitor className="w-4 h-4 text-white" />
              </div>
              Monitoring Statistics
            </h2>
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2 bg-white/80 rounded-lg px-2.5 py-1.5 border border-gray-200 shadow-md">
                <Calendar className="w-4 h-4 text-gray-600" />
                <input
                  type="date"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  className="bg-transparent text-gray-800 text-sm font-medium focus:outline-none"
                />
              </div>
              {/* Toggle Switch for Backlog */}
              <div className="flex items-center gap-2">
                <span className="text-gray-700 text-sm font-semibold">Backlog</span>
                <button
                  onClick={() => setIsBacklog(!isBacklog)}
                  className={`relative inline-flex h-6 w-11 items-center rounded-full transition-all duration-300 shadow-inner ${
                    isBacklog ? 'bg-gradient-to-r from-blue-500 to-blue-600' : 'bg-gradient-to-r from-gray-300 to-gray-400'
                  }`}
                >
                  <span
                    className={`inline-block h-4 w-4 transform rounded-full bg-white shadow-lg transition-transform duration-300 ${
                      isBacklog ? 'translate-x-6' : 'translate-x-1'
                    }`}
                  />
                </button>
              </div>
            </div>
          </div>
          
          <div className={`grid gap-3 ${showAllStats ? 'grid-cols-3' : 'grid-cols-4'} transition-all duration-500`}>
            {Object.entries(displayStats).map(([key, value]) => (
              <div key={key} className={`bg-gradient-to-br ${value.color} rounded-xl p-3 border border-white/50 shadow-lg hover:shadow-xl transition-all duration-300 ${!showAllStats ? 'min-h-[90px] flex flex-col justify-center' : ''}`}>
                <div className="flex items-center justify-between mb-1.5">
                  <div className={`${!showAllStats ? 'w-4 h-4' : 'w-3 h-3'} rounded-full bg-white/80 shadow-md transition-all duration-300`}></div>
                  <span className={`${!showAllStats ? 'text-2xl' : 'text-xl'} font-bold text-gray-800 transition-all duration-300`}>{value.count}</span>
                </div>
                <p className={`${!showAllStats ? 'text-sm' : 'text-xs'} text-gray-700 font-semibold transition-all duration-300`}>{key}</p>
              </div>
            ))}
          </div>
          
          {/* Show More/Less Button */}
          <div className="flex justify-center mt-4">
            <button
              onClick={() => setShowAllStats(!showAllStats)}
              className={`bg-gradient-to-r ${theme.primaryClasses} text-white px-4 py-2 rounded-lg font-semibold text-sm shadow-lg hover:shadow-xl transition-all duration-300 flex items-center gap-2`}
            >
              {showAllStats ? (
                <>
                  Show Less
                  <div className="w-1.5 h-1.5 bg-white rounded-full"></div>
                </>
              ) : (
                <>
                  View All Details
                  <div className="flex gap-1">
                    <div className="w-1.5 h-1.5 bg-white rounded-full"></div>
                    <div className="w-1.5 h-1.5 bg-white rounded-full"></div>
                    <div className="w-1.5 h-1.5 bg-white rounded-full"></div>
                  </div>
                </>
              )}
            </button>
          </div>
        </div>

        {/* SLA Chart Section */}
        <div className={`col-span-2 bg-gradient-to-br ${theme.cardBgClasses} backdrop-blur-sm rounded-3xl p-5 border-2 border-gray-200 shadow-2xl transition-all duration-500`}>
          <h2 className="text-lg font-bold text-gray-800 mb-4 flex items-center gap-2">
            <div className={`p-1.5 bg-gradient-to-br ${theme.iconBgClasses} rounded-lg shadow-lg transition-all duration-500`}>
              <Activity className="w-4 h-4 text-white" />
            </div>
            Client SLA - Hourly Completion
          </h2>
          <div className="h-[calc(100%-60px)]">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={displayHourlyData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#d1d5db" />
                <XAxis 
                  dataKey="hour" 
                  stroke="#4b5563"
                  fontSize={11}
                  fontWeight="600"
                />
                <YAxis 
                  stroke="#4b5563"
                  fontSize={11}
                  fontWeight="600"
                />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: 'white', 
                    border: '2px solid #d1d5db',
                    borderRadius: '12px',
                    color: '#374151',
                    fontSize: '12px',
                    fontWeight: '600',
                    boxShadow: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 10px 10px -5px rgb(0 0 0 / 0.04)'
                  }}
                  formatter={(value, name) => [
                    `${value} ${name === 'completed' ? 'completed' : '%'}`,
                    name === 'completed' ? 'Monitoring' : 'Success Rate'
                  ]}
                />
                <Bar 
                  dataKey="completed" 
                  fill="url(#colorGradient)"
                  radius={[4, 4, 0, 0]}
                />
                <defs>
                  <linearGradient id="colorGradient" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#6b7280" stopOpacity={0.9}/>
                    <stop offset="95%" stopColor="#374151" stopOpacity={0.9}/>
                  </linearGradient>
                </defs>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Agents Section */}
        <div className={`col-span-2 bg-gradient-to-br ${theme.cardBgClasses} backdrop-blur-sm rounded-3xl p-5 border-2 border-gray-200 shadow-2xl transition-all duration-500`}>
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-bold text-gray-800 flex items-center gap-2">
              <div className={`p-1.5 bg-gradient-to-br ${theme.iconBgClasses} rounded-lg shadow-lg transition-all duration-500`}>
                <Users className="w-4 h-4 text-white" />
              </div>
              Agents
            </h2>
            <div className="flex items-center gap-4 text-xs">
              <div className="flex items-center gap-1 bg-emerald-100 px-2 py-1 rounded-lg border border-emerald-200">
                <div className="w-2 h-2 bg-emerald-500 rounded-full shadow-sm"></div>
                <span className="text-emerald-700 font-semibold">Online: {agentStats.online}</span>
              </div>
              <div className="flex items-center gap-1 bg-amber-100 px-2 py-1 rounded-lg border border-amber-200">
                <div className="w-2 h-2 bg-amber-500 rounded-full shadow-sm"></div>
                <span className="text-amber-700 font-semibold">Idle: {agentStats.idle}</span>
              </div>
              <div className="flex items-center gap-1 bg-red-100 px-2 py-1 rounded-lg border border-red-200">
                <div className="w-2 h-2 bg-red-500 rounded-full shadow-sm"></div>
                <span className="text-red-700 font-semibold">Offline: {agentStats.offline}</span>
              </div>
              <div className="flex items-center gap-1 bg-gray-100 px-2 py-1 rounded-lg border border-gray-200">
                <span className="text-gray-800 font-bold">Total: {agentStats.total}</span>
              </div>
            </div>
          </div>
          <div className="overflow-y-auto h-[calc(100%-60px)] pr-1" style={{
            scrollbarWidth: 'thin',
            scrollbarColor: '#cbd5e1 #f1f5f9'
          }}>
            <div className="space-y-1.5">
              {displayAgents.map((agent, index) => (
                <div key={agent.id || index} className="bg-gradient-to-r from-white to-gray-50 rounded-lg p-2.5 border border-gray-200 shadow-sm hover:shadow-md hover:from-gray-50 hover:to-white transition-all duration-300">
                  <div className="flex items-center justify-between mb-1">
                    <div className="flex items-center gap-2">
                      <div className={`flex items-center gap-1 ${getStatusColor(agent.status)}`}>
                        {getStatusIcon(agent.status)}
                      </div>
                      <span className="text-gray-800 font-bold text-sm">{agent.name}</span>
                      {getRankBadge(agent.rank)}
                    </div>
                    <div className="flex items-center">
                      <span className="text-lg font-bold text-gray-800 bg-gradient-to-br from-blue-100 to-blue-200 px-2 py-0.5 rounded-md text-sm">{agent.completed}</span>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-1.5 text-xs">
                    <div className="bg-gray-100 rounded-md px-2 py-1">
                      <p className="text-gray-600 font-semibold">Est: {agent.estimatedHours}h</p>
                    </div>
                    <div className="bg-gray-100 rounded-md px-2 py-1">
                      <p className="text-gray-600 font-semibold">Act: {agent.actualHours}h</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Reviewers Section */}
        <div className={`col-span-2 bg-gradient-to-br ${theme.cardBgClasses} backdrop-blur-sm rounded-3xl p-5 border-2 border-gray-200 shadow-2xl transition-all duration-500`}>
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-bold text-gray-800 flex items-center gap-2">
              <div className={`p-1.5 bg-gradient-to-br ${theme.iconBgClasses} rounded-lg shadow-lg transition-all duration-500`}>
                <Users className="w-4 h-4 text-white" />
              </div>
              Reviewers
            </h2>
            <div className="flex items-center gap-4 text-xs">
              <div className="flex items-center gap-1 bg-emerald-100 px-2 py-1 rounded-lg border border-emerald-200">
                <div className="w-2 h-2 bg-emerald-500 rounded-full shadow-sm"></div>
                <span className="text-emerald-700 font-semibold">Online: {reviewerStats.online}</span>
              </div>
              <div className="flex items-center gap-1 bg-amber-100 px-2 py-1 rounded-lg border border-amber-200">
                <div className="w-2 h-2 bg-amber-500 rounded-full shadow-sm"></div>
                <span className="text-amber-700 font-semibold">Idle: {reviewerStats.idle}</span>
              </div>
              <div className="flex items-center gap-1 bg-red-100 px-2 py-1 rounded-lg border border-red-200">
                <div className="w-2 h-2 bg-red-500 rounded-full shadow-sm"></div>
                <span className="text-red-700 font-semibold">Offline: {reviewerStats.offline}</span>
              </div>
              <div className="flex items-center gap-1 bg-gray-100 px-2 py-1 rounded-lg border border-gray-200">
                <span className="text-gray-800 font-bold">Total: {reviewerStats.total}</span>
              </div>
            </div>
          </div>
          <div className="overflow-y-auto h-[calc(100%-60px)] pr-1" style={{
            scrollbarWidth: 'thin',
            scrollbarColor: '#cbd5e1 #f1f5f9'
          }}>
            <div className="space-y-1.5">
              {displayReviewers.map((reviewer, index) => (
                <div key={reviewer.id || index} className="bg-gradient-to-r from-white to-gray-50 rounded-lg p-2.5 border border-gray-200 shadow-sm hover:shadow-md hover:from-gray-50 hover:to-white transition-all duration-300">
                  <div className="flex items-center justify-between mb-1">
                    <div className="flex items-center gap-2">
                      <div className={`flex items-center gap-1 ${getStatusColor(reviewer.status)}`}>
                        {getStatusIcon(reviewer.status)}
                      </div>
                      <span className="text-gray-800 font-bold text-sm">{reviewer.name}</span>
                      {getRankBadge(reviewer.rank)}
                    </div>
                    <div className="flex items-center">
                      <span className="text-lg font-bold text-gray-800 bg-gradient-to-br from-cyan-100 to-cyan-200 px-2 py-0.5 rounded-md text-sm">{reviewer.completed}</span>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-1.5 text-xs">
                    <div className="bg-gray-100 rounded-md px-2 py-1">
                      <p className="text-gray-600 font-semibold">Est: {reviewer.estimatedHours}h</p>
                    </div>
                    <div className="bg-gray-100 rounded-md px-2 py-1">
                      <p className="text-gray-600 font-semibold">Act: {reviewer.actualHours}h</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Floating Navigation */}
      <FloatingNav />
    </div>
  );
};

export default MonitoringDashboard;