import React, { useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { UserContext } from '../context/UserContext';
import './LoginPage.css';

// Test users matching backend seed data
const TEST_USERS = [
  { id: 1, name: 'John Employee', role: 'Employee' },
  { id: 2, name: 'Jane Admin', role: 'Admin' },
  { id: 3, name: 'Bob Employee', role: 'Employee' },
];

/**
 * LoginPage component for selecting test user.
 * In production, this would be replaced with real authentication.
 */
export default function LoginPage() {
  const { login } = useContext(UserContext);
  const navigate = useNavigate();

  const handleLogin = (user) => {
    login(user.id, user.name, user.role);
    navigate(user.role === 'Admin' ? '/admin' : '/employee');
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <h1>Meeting Room Booking</h1>
        <p className="intro-text">Select a test user to continue:</p>

        <div className="user-list">
          {TEST_USERS.map((user) => (
            <button
              key={user.id}
              className="user-button"
              onClick={() => handleLogin(user)}
            >
              <strong>{user.name}</strong>
              <span className="role-badge">{user.role}</span>
            </button>
          ))}
        </div>

        <p className="note">
          This is a demo environment. In production, use real authentication (SSO, OAuth, etc.)
        </p>
      </div>
    </div>
  );
}
