import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { walletService } from '../services/api';
import { Transaction } from '../types/api';
import './TransactionHistory.css';

const TransactionHistory: React.FC = () => {
  const { student } = useAuth();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const pageSize = 10;

  const loadTransactions = async (pageNum: number = 1, reset: boolean = false) => {
    if (!student) return;

    setLoading(true);
    try {
      const response = await walletService.getTransactionHistory(
        student.studentId, 
        pageNum, 
        pageSize
      );
      
      if (response.success) {
        if (reset) {
          setTransactions(response.data);
        } else {
          setTransactions(prev => [...prev, ...response.data]);
        }
        setHasMore(response.data.length === pageSize);
      }
    } catch (error) {
      console.error('Failed to load transactions:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTransactions(1, true);
    setPage(1);
  }, [student]);

  const handleLoadMore = () => {
    const nextPage = page + 1;
    setPage(nextPage);
    loadTransactions(nextPage, false);
  };

  const handleRefresh = () => {
    setPage(1);
    loadTransactions(1, true);
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(Math.abs(amount));
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const getTransactionIcon = (type: string) => {
    return null;
  };

  const getTransactionColor = (transaction: Transaction) => {
    if (transaction.amount > 0) return 'transaction-positive';
    if (transaction.amount < 0) return 'transaction-negative';
    return 'transaction-neutral';
  };

  return (
    <div className="transaction-history">
      <div className="history-header">
        <h3>Transaction History</h3>
        <button onClick={handleRefresh} disabled={loading} className="refresh-button">
          {loading ? 'Loading...' : 'Refresh'}
        </button>
      </div>

      {transactions.length === 0 && !loading ? (
        <div className="no-transactions">
          <p>No transactions found.</p>
          <p>Start by making a deposit or payment!</p>
        </div>
      ) : (
        <>
          <div className="transactions-list">
            {transactions.map((transaction) => (
              <div key={transaction.transactionId} className="transaction-item">
                <div className="transaction-icon">
                  {getTransactionIcon(transaction.type)}
                </div>
                
                <div className="transaction-details">
                  <div className="transaction-main">
                    <span className="transaction-type">{transaction.type}</span>
                    <span className={`transaction-amount ${getTransactionColor(transaction)}`}>
                      {transaction.amount > 0 ? '+ ' : '- '}
                      {formatCurrency(transaction.amount)}
                    </span>
                  </div>
                  
                  <div className="transaction-description">
                    {transaction.description}
                  </div>
                  
                  <div className="transaction-meta">
                    <span className="transaction-date">
                      {formatDate(transaction.createdAt)}
                    </span>
                    <span className="transaction-balance">
                      Balance: {formatCurrency(transaction.balanceAfter)}
                    </span>
                  </div>
                  
                  {transaction.serviceType && (
                    <div className="transaction-service">
                      Service: {transaction.serviceType}
                    </div>
                  )}
                  
                  {transaction.receiverStudentId && (
                    <div className="transaction-receiver">
                      {transaction.type.toLowerCase() === 'transfer' && transaction.amount > 0 
                        ? `From: ${transaction.receiverStudentId}`
                        : `To: ${transaction.receiverStudentId}`
                      }
                    </div>
                  )}
                </div>
                
                <div className="transaction-id">
                  ID: {transaction.transactionId.substring(0, 8)}...
                </div>
              </div>
            ))}
          </div>

          {hasMore && (
            <div className="load-more">
              <button 
                onClick={handleLoadMore} 
                disabled={loading}
                className="load-more-button"
              >
                {loading ? 'Loading...' : 'Load More'}
              </button>
            </div>
          )}
        </>
      )}

      <div className="transaction-summary">
        <h4>Summary</h4>
        <div className="summary-stats">
          <div className="stat-item">
            <span className="stat-label">Total Transactions:</span>
            <span className="stat-value">{transactions.length}</span>
          </div>
          <div className="stat-item">
            <span className="stat-label">Deposits:</span>
            <span className="stat-value transaction-positive">
              {transactions.filter(t => t.type.toLowerCase() === 'deposit').length}
            </span>
          </div>
          <div className="stat-item">
            <span className="stat-label">Payments:</span>
            <span className="stat-value transaction-negative">
              {transactions.filter(t => t.type.toLowerCase() === 'payment').length}
            </span>
          </div>
          <div className="stat-item">
            <span className="stat-label">Transfers:</span>
            <span className="stat-value transaction-neutral">
              {transactions.filter(t => t.type.toLowerCase() === 'transfer').length}
            </span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TransactionHistory;