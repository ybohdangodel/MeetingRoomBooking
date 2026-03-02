import React, { useContext } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { UserContext } from './context/UserContext';
import Header from './components/Common/Header';
import LoginPage from './pages/LoginPage';
import EmployeePage from './pages/EmployeePage';
import AdminPage from './pages/AdminPage';
import './App.css';

/**
 * Main App component with routing.
 */
export default function App() {
  const { user } = useContext(UserContext);

  return (
    <Router>
      {user && <Header />}
      <Routes>
        <Route path="/" element={user ? <Navigate to={user.role === 'Admin' ? '/admin' : '/employee'} /> : <LoginPage />} />
        <Route
          path="/employee"
          element={user && user.role === 'Employee' ? <EmployeePage /> : <Navigate to="/" />}
        />
        <Route
          path="/admin"
          element={user && user.role === 'Admin' ? <AdminPage /> : <Navigate to="/" />}
        />
      </Routes>
    </Router>
  );
}
