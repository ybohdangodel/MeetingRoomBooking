import React from 'react';
import PropTypes from 'prop-types';
import './ErrorAlert.css';

/**
 * ErrorAlert component displays error messages with dismiss button.
 */
export default function ErrorAlert({ message, onDismiss }) {
  return (
    <div className="error-alert">
      <span className="error-message">{message}</span>
      <button className="dismiss-btn" onClick={onDismiss} aria-label="Dismiss error">
        ✕
      </button>
    </div>
  );
}

ErrorAlert.propTypes = {
  message: PropTypes.string.isRequired,
  onDismiss: PropTypes.func.isRequired,
};
