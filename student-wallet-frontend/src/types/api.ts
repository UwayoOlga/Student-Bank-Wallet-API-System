// API Types for Student Wallet System

export interface LoginRequest {
  studentId: string;
  pin: string;
}

export interface StudentProfile {
  studentId: string;
  walletId: string;
  name: string;
  balance: number;
  lastUpdated: string;
  token?: string;
}

export interface DepositRequest {
  amount: number;
  description: string;
}

export interface PaymentRequest {
  amount: number;
  serviceType: ServiceType;
  description: string;
}

export interface TransferRequest {
  receiverStudentId: string;
  amount: number;
  description: string;
}

export interface Transaction {
  transactionId: string;
  type: string;
  amount: number;
  balanceAfter: number;
  description: string;
  serviceType?: string;
  receiverStudentId?: string;
  createdAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}

export enum ServiceType {
  Cafeteria = 0,
  Printing = 1,
  Transport = 2,
  Other = 3
}

export interface DailySummary {
  date: string;
  totalTransactions: number;
  totalDeposits: number;
  totalPayments: number;
  totalTransfers: number;
  netAmount: number;
}