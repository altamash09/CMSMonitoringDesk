// src/services/apiService.js
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5001/api';

class ApiService {
  constructor() {
    this.baseURL = API_BASE_URL;
  }

  // Generic request method
  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const token = localStorage.getItem('auth-token');
    
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(url, config);
      
      // Handle different response types
      if (response.status === 204) {
        return null; // No content
      }
      
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
      }
      
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        return await response.json();
      }
      
      return await response.text();
    } catch (error) {
      console.error(`API request failed: ${endpoint}`, error);
      throw error;
    }
  }

  // Authentication endpoints
  async login(credentials) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
  }

  async logout() {
    return this.request('/auth/logout', {
      method: 'POST',
    });
  }

  async refreshToken() {
    return this.request('/auth/refresh-token', {
      method: 'POST',
    });
  }

  async validateToken() {
    return this.request('/auth/validate-token', {
      method: 'GET',
    });
  }

  // Dashboard endpoints
  async getMonitoringStats(date = null, isBacklog = false) {
    const params = new URLSearchParams();
    if (date) params.append('date', date);
    if (isBacklog) params.append('backlog', 'true');
    
    return this.request(`/dashboard/monitoring-stats?${params.toString()}`);
  }

  async getHourlyData(date = null) {
    const params = new URLSearchParams();
    if (date) params.append('date', date);
    
    return this.request(`/dashboard/hourly-data?${params.toString()}`);
  }

  async getAgents() {
    return this.request('/dashboard/agents');
  }

  async getReviewers() {
    return this.request('/dashboard/reviewers');
  }

  async getDashboardOverview() {
    return this.request('/dashboard/overview');
  }

  // User Management endpoints
  async getUsers(page = 1, pageSize = 50, search = '', role = '', status = '') {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      ...(search && { search }),
      ...(role && { role }),
      ...(status && { status }),
    });
    
    return this.request(`/users?${params.toString()}`);
  }

  async getUserById(id) {
    return this.request(`/users/${id}`);
  }

  async createUser(userData) {
    return this.request('/users', {
      method: 'POST',
      body: JSON.stringify(userData),
    });
  }

  async updateUser(id, userData) {
    return this.request(`/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(userData),
    });
  }

  async deleteUser(id) {
    return this.request(`/users/${id}`, {
      method: 'DELETE',
    });
  }

  async updateUserStatus(id, status) {
    return this.request(`/users/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status }),
    });
  }

  // Analytics endpoints
  async getAnalyticsData(dateRange = '7d') {
    return this.request(`/analytics?range=${dateRange}`);
  }

  // Reports endpoints
  async getReports(type = 'all') {
    return this.request(`/reports?type=${type}`);
  }

  async generateReport(reportConfig) {
    return this.request('/reports/generate', {
      method: 'POST',
      body: JSON.stringify(reportConfig),
    });
  }

  async downloadReport(reportId) {
    return this.request(`/reports/${reportId}/download`);
  }

  // Notifications endpoints
  async getNotifications(unreadOnly = false) {
    const params = unreadOnly ? '?unreadOnly=true' : '';
    return this.request(`/notifications${params}`);
  }

  async markNotificationRead(id) {
    return this.request(`/notifications/${id}/read`, {
      method: 'PATCH',
    });
  }

  async markAllNotificationsRead() {
    return this.request('/notifications/mark-all-read', {
      method: 'PATCH',
    });
  }

  // Settings endpoints
  async getSettings() {
    return this.request('/settings');
  }

  async updateSettings(settings) {
    return this.request('/settings', {
      method: 'PUT',
      body: JSON.stringify(settings),
    });
  }

  // Health check endpoint
  async healthCheck() {
    return this.request('/health');
  }
}

// Create singleton instance
const apiService = new ApiService();

export default apiService;