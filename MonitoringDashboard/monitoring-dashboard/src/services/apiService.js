// src/services/apiService.js
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7042/api';

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
        // Try to get error message from response
        const errorData = await response.json().catch(() => ({}));
        
        if (response.status === 401) {
          // Handle authentication error
          localStorage.removeItem('auth-token');
          localStorage.removeItem('user-data');
          throw new Error('Authentication failed. Please login again.');
        }
        
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

  // Test endpoints
  async ping() {
    return this.request('/test/ping');
  }

  async testAuth() {
    return this.request('/test/auth-test');
  }

  async getSampleData() {
    return this.request('/test/sample-data', { method: 'POST' });
  }

  // Authentication endpoints
  async login(credentials) {
    const response = await this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
    
    // Store token if login successful
    if (response.success && response.data && response.data.token) {
      localStorage.setItem('auth-token', response.data.token);
      localStorage.setItem('user-data', JSON.stringify(response.data.user));
    }
    
    return response;
  }

  async logout() {
    try {
      await this.request('/auth/logout', {
        method: 'POST',
      });
    } finally {
      // Always clear local storage
      localStorage.removeItem('auth-token');
      localStorage.removeItem('user-data');
    }
  }

  async validateToken() {
    const token = localStorage.getItem('auth-token');
    if (!token) return { success: false };

    return this.request('/auth/validate-token', {
      method: 'POST',
      body: JSON.stringify(token),
    });
  }

  // Dashboard endpoints
  async getDashboardSummary(date = null, isBacklog = false) {
    const params = new URLSearchParams();
    if (date) params.append('date', date);
    if (isBacklog) params.append('isBacklog', 'true');
    
    return this.request(`/dashboard/summary?${params.toString()}`);
  }

  async getMonitoringStats(date = null, isBacklog = false) {
    const params = new URLSearchParams();
    if (date) params.append('date', date);
    if (isBacklog) params.append('backlog', 'true');
    
    return this.request(`/dashboard/monitoring-stats?${params.toString()}`);
  }

  async getSLAData(date = null) {
    const params = new URLSearchParams();
    if (date) params.append('date', date);
    
    return this.request(`/dashboard/sla-data?${params.toString()}`);
  }

  async getAgents() {
    return this.request('/agents');
  }

  async getReviewers() {
    return this.request('/reviewers');
  }

  // Agent Management
  async getAgentById(id) {
    return this.request(`/agents/${id}`);
  }

  async updateAgentStatus(id, status) {
    return this.request(`/agents/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify(status),
    });
  }

  async updateAgentPerformance(id, performanceData) {
    return this.request(`/agents/${id}/performance`, {
      method: 'PUT',
      body: JSON.stringify(performanceData),
    });
  }

  // Reviewer Management
  async getReviewerById(id) {
    return this.request(`/reviewers/${id}`);
  }

  async updateReviewerStatus(id, status) {
    return this.request(`/reviewers/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify(status),
    });
  }

  async updateReviewerPerformance(id, performanceData) {
    return this.request(`/reviewers/${id}/performance`, {
      method: 'PUT',
      body: JSON.stringify(performanceData),
    });
  }

  // SLA Management
  async getCurrentSLAPercentage() {
    return this.request('/sla/current-percentage');
  }

  async updateSLARecord(slaData) {
    return this.request('/sla/update', {
      method: 'PUT',
      body: JSON.stringify(slaData),
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