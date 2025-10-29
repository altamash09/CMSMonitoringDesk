import { useState, useEffect, useCallback, useRef } from 'react';

// API configuration
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7042/api';

// API service for dashboard data
const dashboardAPI = {
  getSummary: async (date, isBacklog = false) => {
    const token = localStorage.getItem('auth-token');
    
    if (!token) {
      throw new Error('No authentication token found');
    }

    const params = new URLSearchParams();
    if (date) params.append('date', date);
    if (isBacklog) params.append('isBacklog', 'true');
    
    const response = await fetch(`${API_BASE_URL}/dashboard/summary?${params.toString()}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      if (response.status === 401) {
        throw new Error('Authentication failed. Please login again.');
      }
      throw new Error(`API Error: ${response.status}`);
    }

    const data = await response.json();
    
    // Your .NET API returns data wrapped in ApiResponse<T>
    if (data.success && data.data) {
      return data.data;
    } else {
      throw new Error(data.message || 'Failed to fetch dashboard data');
    }
  }
};

// Sample fallback data (only used if API is completely unavailable)
const generateSampleData = () => {
  return {
    monitoringStats: {
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
    },
    hourlyData: Array.from({ length: 24 }, (_, i) => ({
      hour: i.toString().padStart(2, '0'),
      completed: Math.floor(Math.random() * 50) + 10,
      percentage: Math.floor(Math.random() * 30) + 75
    })),
    agents: [
      { id: 1, name: 'John Smith', status: 'online', completed: 45, estimatedHours: 8.5, actualHours: 7.2, rank: 'Diamond' },
      { id: 2, name: 'Sarah Johnson', status: 'online', completed: 52, estimatedHours: 9.0, actualHours: 8.1, rank: 'Platinum' },
      { id: 3, name: 'Mike Chen', status: 'idle', completed: 38, estimatedHours: 7.5, actualHours: 6.8, rank: 'Gold' },
      { id: 4, name: 'Emma Davis', status: 'online', completed: 41, estimatedHours: 8.0, actualHours: 7.5, rank: 'Diamond' },
      { id: 5, name: 'Alex Rodriguez', status: 'offline', completed: 29, estimatedHours: 6.5, actualHours: 5.9, rank: 'Silver' },
      { id: 6, name: 'Lisa Wang', status: 'online', completed: 47, estimatedHours: 8.2, actualHours: 7.8, rank: 'Platinum' }
    ],
    reviewers: [
      { id: 1, name: 'Robert Wilson', status: 'online', completed: 28, estimatedHours: 6.5, actualHours: 5.8, rank: 'Diamond' },
      { id: 2, name: 'Jennifer Lee', status: 'online', completed: 32, estimatedHours: 7.0, actualHours: 6.2, rank: 'Platinum' },
      { id: 3, name: 'Mark Taylor', status: 'idle', completed: 25, estimatedHours: 6.0, actualHours: 5.5, rank: 'Gold' },
      { id: 4, name: 'Sophie Anderson', status: 'online', completed: 30, estimatedHours: 6.8, actualHours: 6.0, rank: 'Gold' },
      { id: 5, name: 'Chris Thompson', status: 'offline', completed: 18, estimatedHours: 5.0, actualHours: 4.2, rank: 'Bronze' },
      { id: 6, name: 'Maya Patel', status: 'online', completed: 26, estimatedHours: 6.2, actualHours: 5.7, rank: 'Silver' }
    ],
    agentStats: { total: 6, online: 4, idle: 1, offline: 1 },
    reviewerStats: { total: 6, online: 4, idle: 1, offline: 1 }
  };
};

export const useDashboardData = (selectedDate, isBacklog) => {
  const [data, setData] = useState(generateSampleData()); // Start with sample data
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [lastUpdated, setLastUpdated] = useState(new Date());
  const [useApiData, setUseApiData] = useState(false);
  
  // Use ref to prevent multiple simultaneous requests
  const isLoadingRef = useRef(false);

  const fetchDashboardData = useCallback(async (date, backlog) => {
    // Check if we have authentication token
    const token = localStorage.getItem('auth-token');
    
    if (!token) {
      console.log('No authentication token found, using sample data');
      setUseApiData(false);
      setData(generateSampleData());
      setLastUpdated(new Date());
      setError(null);
      return;
    }

    // Prevent multiple simultaneous requests
    if (isLoadingRef.current) {
      return;
    }

    try {
      isLoadingRef.current = true;
      setLoading(true);
      setError(null);
      
      console.log('Fetching dashboard data from API for:', { date, backlog });

      // Try to fetch from API
      const dashboardData = await dashboardAPI.getSummary(date, backlog);
      
      console.log('Dashboard data received from API:', dashboardData);

      // Transform the data to match component expectations
      const transformedData = {
        monitoringStats: dashboardData.monitoringStats?.stats || {},
        hourlyData: dashboardData.slaData || [],
        agents: dashboardData.agents || [],
        reviewers: dashboardData.reviewers || [],
        agentStats: dashboardData.agentStats || { total: 0, online: 0, idle: 0, offline: 0 },
        reviewerStats: dashboardData.reviewerStats || { total: 0, online: 0, idle: 0, offline: 0 }
      };

      setData(transformedData);
      setUseApiData(true);
      setLastUpdated(new Date());
      
      console.log('Dashboard data updated successfully from API');
      
    } catch (err) {
      console.error('Error fetching dashboard data:', err);
      
      // If it's an authentication error, redirect to login
      if (err.message.includes('Authentication failed') || err.message.includes('401')) {
        console.log('Authentication failed, redirecting to login');
        localStorage.removeItem('auth-token');
        localStorage.removeItem('user-data');
        window.location.href = '/login';
        return;
      }
      
      // For other errors, show error but keep existing data
      setError(err.message);
      setLastUpdated(new Date());
    } finally {
      setLoading(false);
      isLoadingRef.current = false;
    }
  }, []);

  // Manual refresh function
  const refreshData = useCallback(async () => {
    await fetchDashboardData(selectedDate, isBacklog);
  }, [selectedDate, isBacklog, fetchDashboardData]);

  // Fetch data when dependencies change
  useEffect(() => {
    if (selectedDate) {
      fetchDashboardData(selectedDate, isBacklog);
    }
  }, [selectedDate, isBacklog, fetchDashboardData]);

  // Auto-refresh every 30 seconds only if using API data
  useEffect(() => {
    if (!useApiData) {
      return; // Don't auto-refresh if using sample data
    }

    const interval = setInterval(() => {
      if (!loading && !isLoadingRef.current) {
        console.log('Auto-refreshing dashboard data...');
        fetchDashboardData(selectedDate, isBacklog);
      }
    }, 30000); // 30 seconds

    return () => clearInterval(interval);
  }, [selectedDate, isBacklog, loading, fetchDashboardData, useApiData]);

  return {
    data,
    loading,
    error,
    lastUpdated,
    refreshData,
    useApiData // Whether we're using API or sample data
  };
};