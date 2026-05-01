import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { walletService, authService } from '../services/api';
import { ServiceType } from '../types/api';
import './TransactionForm.css';

interface TransactionFormProps {
  type: 'deposit' | 'payment' | 'transfer';
  onSuccess: () => void;
}

const TransactionForm: React.FC<TransactionFormProps> = ({ type, onSuccess }) => {
  const { student } = useAuth();
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [serviceType, setServiceType] = useState<ServiceType>(ServiceType.Cafeteria);
  const [receiverStudentId, setReceiverStudentId] = useState('');
  const [receiverName, setReceiverName] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const validateReceiver = async (studentId: string) => {
    if (!studentId || studentId.trim().length === 0) {
      setReceiverName('');
      return;
    }

    // Only validate if it looks like a valid student ID format
    if (studentId.trim().length < 3) {
      setReceiverName('');
      return;
    }

    try {
      const response = await authService.validateStudent(studentId.trim());
      if (response.success) {
        setReceiverName(response.data);
        setError('');
      } else {
        setReceiverName('');
        setError('Receiver not found');
      }
    } catch (err) {
      setReceiverName('');
      setError('Failed to validate receiver');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!student) return;

    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const amountNum = parseFloat(amount);
      
      if (amountNum <= 0) {
        setError('Amount must be greater than 0');
        return;
      }

      let response;

      switch (type) {
        case 'deposit':
          response = await walletService.deposit(student.studentId, {
            amount: amountNum,
            description: description || 'Deposit'
          });
          break;

        case 'payment':
          response = await walletService.payForService(student.studentId, {
            amount: amountNum,
            serviceType,
            description: description || `Payment for ${ServiceType[serviceType]}`
          });
          break;

        case 'transfer':
          if (!receiverStudentId) {
            setError('Please enter receiver Student ID');
            return;
          }
          response = await walletService.transferMoney(student.studentId, {
            receiverStudentId,
            amount: amountNum,
            description: description || 'Money transfer'
          });
          break;

        default:
          throw new Error('Invalid transaction type');
      }

      if (response.success) {
        setSuccess(`${type.charAt(0).toUpperCase() + type.slice(1)} successful! New balance: $${response.data.balanceAfter.toFixed(2)}`);
        setAmount('');
        setDescription('');
        setReceiverStudentId('');
        setReceiverName('');
        onSuccess();
      } else {
        setError(response.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || `${type} failed. Please try again.`);
    } finally {
      setLoading(false);
    }
  };

  const getTitle = () => {
    switch (type) {
      case 'deposit': return 'Deposit Money';
      case 'payment': return 'Pay for Service';
      case 'transfer': return 'Transfer Money';
      default: return 'Transaction';
    }
  };

  const getServiceTypeLabel = (serviceType: ServiceType) => {
    switch (serviceType) {
      case ServiceType.Cafeteria: return 'Cafeteria';
      case ServiceType.Printing: return 'Printing';
      case ServiceType.Transport: return 'Transport';
      case ServiceType.Other: return 'Other';
      default: return 'Unknown';
    }
  };

  return (
    <div className="transaction-form">
      <h3>{getTitle()}</h3>
      
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="amount">Amount ($)</label>
          <input
            type="number"
            id="amount"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            placeholder="Enter amount"
            required
            min="0.01"
            max="10000"
            step="0.01"
          />
        </div>

        {type === 'payment' && (
          <div className="form-group">
            <label htmlFor="serviceType">Service Type</label>
            <select
              id="serviceType"
              value={serviceType}
              onChange={(e) => setServiceType(parseInt(e.target.value) as ServiceType)}
              required
            >
              <option value={ServiceType.Cafeteria}>Cafeteria</option>
              <option value={ServiceType.Printing}>Printing</option>
              <option value={ServiceType.Transport}>Transport</option>
              <option value={ServiceType.Other}>Other</option>
            </select>
          </div>
        )}

        {type === 'transfer' && (
          <div className="form-group">
            <label htmlFor="receiverStudentId">Receiver Student ID</label>
            <input
              type="text"
              id="receiverStudentId"
              value={receiverStudentId}
              onChange={(e) => {
                setReceiverStudentId(e.target.value);
                validateReceiver(e.target.value);
              }}
              placeholder="Enter receiver's Student ID"
              required
              maxLength={20}
            />
            {receiverName && (
              <div className="receiver-info">
                Receiver: {receiverName}
              </div>
            )}
          </div>
        )}

        <div className="form-group">
          <label htmlFor="description">Description (Optional)</label>
          <input
            type="text"
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder={`Enter ${type} description`}
            maxLength={200}
          />
        </div>

        {error && <div className="error-message">{error}</div>}
        {success && <div className="success-message">{success}</div>}

        <button type="submit" disabled={loading} className="submit-button">
          {loading ? 'Processing...' : `${type.charAt(0).toUpperCase() + type.slice(1)}`}
        </button>
      </form>

      {type === 'payment' && (
        <div className="service-info">
          <h4>Service Types:</h4>
          <ul>
            <li><strong>Cafeteria:</strong> Meals and snacks</li>
            <li><strong>Printing:</strong> Document printing services</li>
            <li><strong>Transport:</strong> Campus shuttle and transport</li>
            <li><strong>Other:</strong> Miscellaneous services</li>
          </ul>
        </div>
      )}
    </div>
  );
};

export default TransactionForm;