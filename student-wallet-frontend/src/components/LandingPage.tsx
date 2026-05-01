import React from 'react';
import './LandingPage.css';

interface LandingPageProps {
  onGetStarted: () => void;
}

const LandingPage: React.FC<LandingPageProps> = ({ onGetStarted }) => {
  return (
    <div className="landing-page">
      <div className="landing-overlay">
        <header className="landing-nav">
          <div className="logo">Student Wallet</div>
          <button onClick={onGetStarted} className="nav-button">Login</button>
        </header>

        <main className="landing-content">
          <div className="hero-box">
            <span className="tagline">University Digital System</span>
            <h1>Manage your campus finances with ease</h1>
            <p>
              A secure platform designed for students to handle payments, transfers, 
              and budgeting in one central place.
            </p>
            <div className="cta-wrapper">
              <button onClick={onGetStarted} className="primary-cta">
                Access Wallet
              </button>
              <button onClick={onGetStarted} className="secondary-cta">
                Learn More
              </button>
            </div>
          </div>
        </main>

        <footer className="landing-mini-footer">
          <div className="footer-links">
            <span>Security</span>
            <span>Terms</span>
            <span>Support</span>
          </div>
          <div className="copyright">
            &copy; 2024 University Digital Payment Solutions
          </div>
        </footer>
      </div>
    </div>
  );
};

export default LandingPage;