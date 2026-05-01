import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { walletService } from '../services/api';
import TransactionForm from './TransactionForm';
import TransactionHistory from './TransactionHistory';
import './Dashboard.css';

const Dashboard: React.FC = () => {
  const { student, logout, updateProfile } = useAuth();
  const [activeTab, setActiveTab] = useState('balance');
  const [loading, setLoading] = useState(false);

  const refreshProfile = async () => {
    if (!student) return;
    
    setLoading(true);
    try {
      const response = await walletService.getBalance(student.studentId);
      if (response.success) {
        updateProfile(response.data);
      }
    } catch (error) {
      console.error('Failed to refresh profile:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    refreshProfile();
  }, []);

  if (!student) return null;

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  };

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <div className="header-content">
          <div className="user-info">
            <h1>Welcome, {student.name}!</h1>
            <p>Student ID: {student.studentId}</p>
          </div>
          <button onClick={logout} className="logout-button">
            Logout
          </button>
        </div>
      </header>

      <div className="balance-card">
        <div className="balance-info">
          <h2>Current Balance</h2>
          <div className="balance-amount">
            {formatCurrency(student.balance)}
          </div>
          <button 
            onClick={refreshProfile} 
            disabled={loading}
            className="refresh-button"
          >
            {loading ? 'Refreshing...' : 'Refresh'}
          </button>
        </div>
      </div>

      <nav className="dashboard-nav">
        <button 
          className={activeTab === 'balance' ? 'active' : ''}
          onClick={() => setActiveTab('balance')}
        >
          Balance
        </button>
        <button 
          className={activeTab === 'deposit' ? 'active' : ''}
          onClick={() => setActiveTab('deposit')}
        >
          Deposit
        </button>
        <button 
          className={activeTab === 'pay' ? 'active' : ''}
          onClick={() => setActiveTab('pay')}
        >
          Pay
        </button>
        <button 
          className={activeTab === 'transfer' ? 'active' : ''}
          onClick={() => setActiveTab('transfer')}
        >
          Transfer
        </button>
        <button 
          className={activeTab === 'history' ? 'active' : ''}
          onClick={() => setActiveTab('history')}
        >
          History
        </button>
      </nav>

      <div className="dashboard-content">
        {activeTab === 'balance' && (
          <div className="balance-tab">
            <h3>Account Information</h3>
            <div className="info-grid">
              <div className="info-item">
                <label>Student ID:</label>
                <span>{student.studentId}</span>
              </div>
              <div className="info-item">
                <label>Wallet ID:</label>
                <span>{student.walletId}</span>
              </div>
              <div className="info-item">
                <label>Name:</label>
                <span>{student.name}</span>
              </div>
              <div className="info-item">
                <label>Current Balance:</label>
                <span className="balance-highlight">{formatCurrency(student.balance)}</span>
              </div>
              <div className="info-item">
                <label>Last Updated:</label>
                <span>{new Date(student.lastUpdated).toLocaleString()}</span>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'deposit' && (
          <TransactionForm 
            type="deposit" 
            onSuccess={refreshProfile}
          />
        )}

        {activeTab === 'pay' && (
          <TransactionForm 
            type="payment" 
            onSuccess={refreshProfile}
          />
        )}

        {activeTab === 'transfer' && (
          <TransactionForm 
            type="transfer" 
            onSuccess={refreshProfile}
          />
        )}

        {activeTab === 'history' && (
          <TransactionHistory />
        )}
      </div>
    </div>
  );
};

export default Dashboard;