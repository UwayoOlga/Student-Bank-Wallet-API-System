import React, { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import LandingPage from './components/LandingPage';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import './App.css';

const AppContent: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [showLogin, setShowLogin] = useState(false);
  
  if (isAuthenticated) {
    return <Dashboard />;
  }
  
  if (showLogin) {
    return <Login onBackToHome={() => setShowLogin(false)} />;
  }
  
  return <LandingPage onGetStarted={() => setShowLogin(true)} />;
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <div className="App">
        <AppContent />
      </div>
    </AuthProvider>
  );
};

export default App;
