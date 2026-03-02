import React, { createContext, useState } from 'react';
import PropTypes from 'prop-types';

export const UserContext = createContext();

/**
 * UserProvider component that wraps the app and provides user state.
 * Manages login/logout state for hardcoded test users and persists to localStorage.
 */
export function UserProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem('user');
    return stored ? JSON.parse(stored) : null;
  });

  const login = (userId, userName, userRole) => {
    const userData = { id: userId, name: userName, role: userRole };
    setUser(userData);
    localStorage.setItem('userId', userId);
    localStorage.setItem('user', JSON.stringify(userData));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('userId');
    localStorage.removeItem('user');
  };

  return (
    <UserContext.Provider value={{ user, login, logout }}>
      {children}
    </UserContext.Provider>
  );
}

UserProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
