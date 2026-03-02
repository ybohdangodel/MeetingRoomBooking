import React, { useState, useContext } from 'react';
import PropTypes from 'prop-types';
import { createBooking } from '../../services/api';
import { UserContext } from '../../context/UserContext';
import ErrorAlert from '../Common/ErrorAlert';
import './BookingForm.css';

/**
 * BookingForm component for creating a new booking.
 */
export default function BookingForm({ room, onSuccess, onCancel }) {
  const { user } = useContext(UserContext);
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [notes, setNotes] = useState('');
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!startTime || !endTime) {
      setError('Please select both start and end times.');
      return;
    }

    if (new Date(startTime) >= new Date(endTime)) {
      setError('End time must be after start time.');
      return;
    }

    setIsSubmitting(true);
    try {
      await createBooking({
        userId: user.id,
        roomId: room.id,
        startTime,
        endTime,
        notes,
      });
      onSuccess();
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="booking-form">
      <h2>Book Room: {room.name}</h2>
      {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="startTime">Start Time:</label>
          <input
            id="startTime"
            type="datetime-local"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="endTime">End Time:</label>
          <input
            id="endTime"
            type="datetime-local"
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="notes">Notes (optional):</label>
          <textarea
            id="notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            placeholder="Add any notes or requirements..."
            rows="3"
          />
        </div>

        <div className="form-actions">
          <button type="submit" className="button button--primary" disabled={isSubmitting}>
            {isSubmitting ? 'Booking...' : 'Book Room'}
          </button>
          <button type="button" className="button button--secondary" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

BookingForm.propTypes = {
  room: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
  }).isRequired,
  onSuccess: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
};
