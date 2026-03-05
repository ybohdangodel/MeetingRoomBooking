import React from 'react';
import PropTypes from 'prop-types';
import './BookingCard.css';

/**
 * BookingCard component displays booking details.
 */
export default function BookingCard({ booking, onCancel, onApprove, showActions }) {
  const formatDateTime = (dateTime) => {
    return new Date(dateTime).toLocaleString();
  };

  const statusClass = `status status--${booking.status.toLowerCase()}`;

  return (
    <div className="booking-card">
      <div className="booking-card__content">
        <h4 className="booking-card__room">{booking.roomName}</h4>
        <p className="booking-card__detail">
          <strong>Start:</strong> {formatDateTime(booking.startTime)}
        </p>
        <p className="booking-card__detail">
          <strong>End:</strong> {formatDateTime(booking.endTime)}
        </p>
        <p className={statusClass}>
          <strong>Status:</strong> {booking.status}
        </p>
        {booking.notes && (
          <p className="booking-card__notes">
            <strong>Notes:</strong> {booking.notes}
          </p>
        )}
      </div>

      {showActions && (
        <div className="booking-card__actions">
          {booking.status === 'Pending' && (
            <>
              <button className="button button--danger" onClick={() => onCancel(booking.id)}>
                Cancel
              </button>
              {onApprove && (
                <button className="button button--success" onClick={() => onApprove(booking.id)}>
                  Approve
                </button>
              )}
            </>
          )}
          {booking.status === 'Confirmed' && (
            <button className="button button--danger" onClick={() => onCancel(booking.id)}>
              Cancel
            </button>
          )}
        </div>
      )}
    </div>
  );
}

BookingCard.propTypes = {
  booking: PropTypes.shape({
    id: PropTypes.number.isRequired,
    roomName: PropTypes.string.isRequired,
    startTime: PropTypes.string.isRequired,
    endTime: PropTypes.string.isRequired,
    status: PropTypes.string.isRequired,
    notes: PropTypes.string,
  }).isRequired,
  onCancel: PropTypes.func.isRequired,
  onApprove: PropTypes.func,
  showActions: PropTypes.bool,
};

BookingCard.defaultProps = {
  onApprove: null,
  showActions: false,
};
