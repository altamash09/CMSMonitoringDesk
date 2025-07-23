import { useState, useEffect, useCallback, useRef } from 'react';

// API configuration
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

// API service for dashboard data
const dashboardAPI = {
  getSummary: async (date, isBacklog = false) => {
    const token = localStorage.getItem('authToken');
    
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
  },

  getAgents: async () => {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      throw new Error('No authentication token found');
    }

    const response = await fetch(`${API_BASE_URL}/agents`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch agents: ${response.status}`);
    }

    const data = await response.json();
    return data.success ? data.data : [];
  },

  getReviewers: async () => {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      throw new Error('No authentication token found');
    }

    const response = await fetch(`${API_BASE_URL}/reviewers`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch reviewers: ${response.status}`);
    }

    const data = await response.json();
    return data.success ? data.data : [];
  }
};

export const useDashboardData = (selectedDate, isBacklog) => {
  const [data, setData] = useState({
    monitoringStats: { stats: {} },
    slaData: [],
    agents: [],
    reviewers: [],
    agentStats: { total: 0, online: 0, idle: 0, offline: 0 },
    reviewerStats: { total: 0, online: 0, idle: 0, offline: 0 }
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [lastUpdated, setLastUpdated] = useState(new Date());
  
  // Use ref to prevent multiple simultaneous requests
  const isLoadingRef = useRef(false);

  const fetchDashboardData = useCallback(async (date, backlog) => {
    // Prevent multiple simultaneous requests
    if (isLoadingRef.current) {
      return;
    }

    try {
      isLoadingRef.current = true;
      setLoading(true);
      setError(null);
      
      console.log('Fetching dashboard data for:', { date, backlog });

      // Fetch dashboard summary which includes everything
      const dashboardData = await dashboardAPI.getSummary(date, backlog);
      
      console.log('Dashboard data received:', dashboardData);

      // Transform the data to match component expectations
      const transformedData = {
        monitoringStats: dashboardData.monitoringStats || { stats: {} },
        slaData: dashboardData.slaData || [],
        agents: dashboardData.agents || [],
        reviewers: dashboardData.reviewers || [],
        agentStats: dashboardData.agentStats || { total: 0, online: 0, idle: 0, offline: 0 },
        reviewerStats: dashboardData.reviewerStats || { total: 0, online: 0, idle: 0, offline: 0 }
      };

      setData(transformedData);
      setLastUpdated(new Date());
      
      console.log('Dashboard data updated successfully');
      
    } catch (err) {
      console.error('Error fetching dashboard data:', err);
      setError(err.message);
      
      // If authentication error, clear token and redirect to login
      if (err.message.includes('Authentication failed')) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
        window.location.href = '/login';
      }
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
    // Only fetch if we have a valid date
    if (selectedDate) {
      fetchDashboardData(selectedDate, isBacklog);
    }
  }, [selectedDate, isBacklog, fetchDashboardData]);

  // Auto-refresh every 30 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      if (!loading && !isLoadingRef.current) {
        console.log('Auto-refreshing dashboard data...');
        fetchDashboardData(selectedDate, isBacklog);
      }
    }, 30000); // 30 seconds

    return () => clearInterval(interval);
  }, [selectedDate, isBacklog, loading, fetchDashboardData]);

  return {
    data,
    loading,
    error,
    lastUpdated,
    refreshData
  };
};