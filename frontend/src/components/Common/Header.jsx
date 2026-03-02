import React, { useContext } from 'react';
import PropTypes from 'prop-types';
import { UserContext } from '../../context/UserContext';
import './Header.css';

/**
 * Header component displays user info and logout button.
 */
export default function Header() {
  const { user, logout } = useContext(UserContext);

  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  return (
    <header className="header">
      <div className="header-container">
        <h1>Meeting Room Booking</h1>
        {user && (
          <div className="header-user">
            <span className="user-info">
              {user.name} ({user.role})
            </span>
            <button className="logout-btn" onClick={handleLogout}>
              Logout
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
