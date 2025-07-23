import React, { useState } from 'react';
import { 
  Users, 
  Plus, 
  Search, 
  Filter, 
  Edit3, 
  Trash2, 
  MoreVertical,
  Shield,
  CheckCircle,
  XCircle,
  Clock,
  Mail,
  Phone,
  Calendar
} from 'lucide-react';
import { useTheme } from '../../contexts/ThemeContext';
import FloatingNav from '../common/FloatingNav';

const UserManagement = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterRole, setFilterRole] = useState('all');
  const [filterStatus, setFilterStatus] = useState('all');
  const { theme } = useTheme();

  // Sample user data
  const [users, setUsers] = useState([
    {
      id: 1,
      name: 'John Smith',
      email: 'john.smith@company.com',
      phone: '+1 (555) 123-4567',
      role: 'admin',
      status: 'active',
      lastLogin: '2025-07-21',
      department: 'IT',
      avatar: 'JS'
    },
    {
      id: 2,
      name: 'Sarah Johnson',
      email: 'sarah.johnson@company.com',
      phone: '+1 (555) 234-5678',
      role: 'manager',
      status: 'active',
      lastLogin: '2025-07-20',
      department: 'Operations',
      avatar: 'SJ'
    },
    {
      id: 3,
      name: 'Mike Chen',
      email: 'mike.chen@company.com',
      phone: '+1 (555) 345-6789',
      role: 'agent',
      status: 'inactive',
      lastLogin: '2025-07-15',
      department: 'Monitoring',
      avatar: 'MC'
    },
    {
      id: 4,
      name: 'Emma Davis',
      email: 'emma.davis@company.com',
      phone: '+1 (555) 456-7890',
      role: 'reviewer',
      status: 'active',
      lastLogin: '2025-07-21',
      department: 'Quality',
      avatar: 'ED'
    },
    {
      id: 5,
      name: 'Alex Rodriguez',
      email: 'alex.rodriguez@company.com',
      phone: '+1 (555) 567-8901',
      role: 'agent',
      status: 'pending',
      lastLogin: null,
      department: 'Monitoring',
      avatar: 'AR'
    },
    {
      id: 6,
      name: 'Lisa Wang',
      email: 'lisa.wang@company.com',
      phone: '+1 (555) 678-9012',
      role: 'supervisor',
      status: 'active',
      lastLogin: '2025-07-21',
      department: 'Operations',
      avatar: 'LW'
    }
  ]);

  const getRoleColor = (role) => {
    switch (role) {
      case 'admin': return 'bg-red-100 text-red-800 border-red-200';
      case 'manager': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'supervisor': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'reviewer': return 'bg-green-100 text-green-800 border-green-200';
      case 'agent': return 'bg-gray-100 text-gray-800 border-gray-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'active': return <CheckCircle className="w-4 h-4 text-green-500" />;
      case 'inactive': return <XCircle className="w-4 h-4 text-red-500" />;
      case 'pending': return <Clock className="w-4 h-4 text-yellow-500" />;
      default: return <Clock className="w-4 h-4 text-gray-500" />;
    }
  };

  const filteredUsers = users.filter(user => {
    const matchesSearch = user.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         user.department.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesRole = filterRole === 'all' || user.role === filterRole;
    const matchesStatus = filterStatus === 'all' || user.status === filterStatus;
    
    return matchesSearch && matchesRole && matchesStatus;
  });

  const stats = {
    total: users.length,
    active: users.filter(u => u.status === 'active').length,
    inactive: users.filter(u => u.status === 'inactive').length,
    pending: users.filter(u => u.status === 'pending').length
  };

  return (
    <div className={`min-h-screen bg-gradient-to-br ${theme.bgClasses} p-6 transition-all duration-700`}>
      {/* Header */}
      <div className="mb-8">
        <div className="flex items-center justify-between mb-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-800 flex items-center gap-3">
              <div className={`p-3 bg-gradient-to-br ${theme.iconBgClasses} rounded-xl shadow-lg transition-all duration-500`}>
                <Users className="w-6 h-6 text-white" />
              </div>
              User Management
            </h1>
            <p className="text-gray-600 mt-2 ml-14">Manage system users, roles, and permissions</p>
          </div>
          <button className={`bg-gradient-to-r ${theme.primaryClasses} text-white px-6 py-3 rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-300 flex items-center gap-2`}>
            <Plus className="w-5 h-5" />
            Add New User
          </button>
        </div>
        
        <div className="flex justify-center">
          <div className={`h-1 w-32 bg-gradient-to-r ${theme.accentClasses} rounded-full shadow-sm transition-all duration-500`}></div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-4 gap-6 mb-8">
        <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl p-6 border-2 border-gray-200 shadow-xl transition-all duration-500`}>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 font-semibold text-sm">Total Users</p>
              <p className="text-3xl font-bold text-gray-800">{stats.total}</p>
            </div>
            <div className="p-3 bg-blue-100 rounded-xl">
              <Users className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl p-6 border-2 border-gray-200 shadow-xl transition-all duration-500`}>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 font-semibold text-sm">Active Users</p>
              <p className="text-3xl font-bold text-green-600">{stats.active}</p>
            </div>
            <div className="p-3 bg-green-100 rounded-xl">
              <CheckCircle className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl p-6 border-2 border-gray-200 shadow-xl transition-all duration-500`}>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 font-semibold text-sm">Inactive Users</p>
              <p className="text-3xl font-bold text-red-600">{stats.inactive}</p>
            </div>
            <div className="p-3 bg-red-100 rounded-xl">
              <XCircle className="w-6 h-6 text-red-600" />
            </div>
          </div>
        </div>

        <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl p-6 border-2 border-gray-200 shadow-xl transition-all duration-500`}>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 font-semibold text-sm">Pending Users</p>
              <p className="text-3xl font-bold text-yellow-600">{stats.pending}</p>
            </div>
            <div className="p-3 bg-yellow-100 rounded-xl">
              <Clock className="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Filters and Search */}
      <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl p-6 border-2 border-gray-200 shadow-xl mb-6 transition-all duration-500`}>
        <div className="flex items-center gap-4 flex-wrap">
          {/* Search */}
          <div className="flex-1 min-w-[300px]">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input
                type="text"
                placeholder="Search users by name, email, or department..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-3 border-2 border-gray-200 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-transparent bg-white/80 backdrop-blur-sm transition-all duration-300 text-gray-800 font-medium placeholder-gray-400"
              />
            </div>
          </div>

          {/* Role Filter */}
          <div className="flex items-center gap-2">
            <Filter className="w-5 h-5 text-gray-500" />
            <select
              value={filterRole}
              onChange={(e) => setFilterRole(e.target.value)}
              className="px-4 py-3 border-2 border-gray-200 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-transparent bg-white/80 backdrop-blur-sm text-gray-800 font-medium"
            >
              <option value="all">All Roles</option>
              <option value="admin">Admin</option>
              <option value="manager">Manager</option>
              <option value="supervisor">Supervisor</option>
              <option value="reviewer">Reviewer</option>
              <option value="agent">Agent</option>
            </select>
          </div>

          {/* Status Filter */}
          <select
            value={filterStatus}
            onChange={(e) => setFilterStatus(e.target.value)}
            className="px-4 py-3 border-2 border-gray-200 rounded-xl focus:ring-2 focus:ring-gray-500 focus:border-transparent bg-white/80 backdrop-blur-sm text-gray-800 font-medium"
          >
            <option value="all">All Statuses</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="pending">Pending</option>
          </select>
        </div>
      </div>

      {/* Users Table */}
      <div className={`bg-gradient-to-br ${theme.cardBgClasses} rounded-2xl border-2 border-gray-200 shadow-xl overflow-hidden transition-all duration-500`}>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50/50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">User</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Contact</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Role</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Department</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Status</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Last Login</th>
                <th className="px-6 py-4 text-left text-sm font-bold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {filteredUsers.map((user) => (
                <tr key={user.id} className="hover:bg-gray-50/30 transition-colors duration-200">
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-3">
                      <div className={`w-10 h-10 bg-gradient-to-br ${theme.iconBgClasses} rounded-full flex items-center justify-center text-white font-bold text-sm transition-all duration-300`}>
                        {user.avatar}
                      </div>
                      <div>
                        <p className="font-semibold text-gray-800">{user.name}</p>
                        <p className="text-sm text-gray-600">ID: {user.id}</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="space-y-1">
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Mail className="w-4 h-4" />
                        {user.email}
                      </div>
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Phone className="w-4 h-4" />
                        {user.phone}
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-3 py-1 rounded-lg text-sm font-semibold border ${getRoleColor(user.role)}`}>
                      {user.role.charAt(0).toUpperCase() + user.role.slice(1)}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className="text-gray-700 font-medium">{user.department}</span>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2">
                      {getStatusIcon(user.status)}
                      <span className="font-medium capitalize text-gray-700">
                        {user.status}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2 text-sm text-gray-600">
                      <Calendar className="w-4 h-4" />
                      {user.lastLogin || 'Never'}
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2">
                      <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors duration-200" title="Edit User">
                        <Edit3 className="w-4 h-4 text-blue-600" />
                      </button>
                      <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors duration-200" title="Delete User">
                        <Trash2 className="w-4 h-4 text-red-600" />
                      </button>
                      <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors duration-200" title="More Options">
                        <MoreVertical className="w-4 h-4 text-gray-600" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {filteredUsers.length === 0 && (
          <div className="text-center py-12">
            <Users className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-500 font-medium">No users found matching your criteria</p>
          </div>
        )}
      </div>

      {/* Floating Navigation */}
      <FloatingNav />
    </div>
  );
};

export default UserManagement;