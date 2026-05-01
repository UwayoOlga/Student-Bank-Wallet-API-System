import axios from 'axios';
import {
  LoginRequest,
  StudentProfile,
  DepositRequest,
  PaymentRequest,
  TransferRequest,
  Transaction,
  ApiResponse,
  DailySummary
} from '../types/api';

const API_BASE_URL = 'http://localhost:5156/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add a request interceptor to include the JWT token
api.interceptors.request.use(
  (config) => {
    const savedStudent = localStorage.getItem('student');
    if (savedStudent) {
      const student = JSON.parse(savedStudent);
      if (student.token) {
        config.headers.Authorization = `Bearer ${student.token}`;
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add a response interceptor to handle 401 errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('student');
      window.location.href = '/'; // Redirect to login
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (request: LoginRequest): Promise<ApiResponse<StudentProfile>> => {
    const response = await api.post('/auth/login', request);
    return response.data;
  },

  validateStudent: async (studentId: string): Promise<ApiResponse<string>> => {
    const response = await api.get(`/auth/validate/${studentId}`);
    return response.data;
  },

  unlockAccount: async (studentId: string): Promise<ApiResponse<string>> => {
    const response = await api.post(`/auth/unlock/${studentId}`);
    return response.data;
  }
};

export const walletService = {
  getBalance: async (studentId: string): Promise<ApiResponse<StudentProfile>> => {
    const response = await api.get(`/wallet/balance/${studentId}`);
    return response.data;
  },

  deposit: async (studentId: string, request: DepositRequest): Promise<ApiResponse<Transaction>> => {
    const response = await api.post(`/wallet/deposit/${studentId}`, request);
    return response.data;
  },

  payForService: async (studentId: string, request: PaymentRequest): Promise<ApiResponse<Transaction>> => {
    const response = await api.post(`/wallet/pay/${studentId}`, request);
    return response.data;
  },

  transferMoney: async (studentId: string, request: TransferRequest): Promise<ApiResponse<Transaction>> => {
    const response = await api.post(`/wallet/transfer/${studentId}`, request);
    return response.data;
  },

  getTransactionHistory: async (
    studentId: string, 
    page: number = 1, 
    pageSize: number = 10
  ): Promise<ApiResponse<Transaction[]>> => {
    const response = await api.get(`/wallet/history/${studentId}?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }
};

export const reportsService = {
  getDailySummary: async (date?: string): Promise<ApiResponse<DailySummary>> => {
    const url = date ? `/reports/daily/${date}` : '/reports/daily';
    const response = await api.get(url);
    return response.data;
  },

  getOverallSummary: async (): Promise<ApiResponse<any>> => {
    const response = await api.get('/reports/summary');
    return response.data;
  }
};