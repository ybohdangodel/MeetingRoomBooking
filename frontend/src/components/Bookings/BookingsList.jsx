import React, { useState, useEffect, useContext } from 'react';
import PropTypes from 'prop-types';
import { fetchUserBookings, cancelBooking } from '../../services/api';
import { UserContext } from '../../context/UserContext';
import BookingCard from './BookingCard';
import ErrorAlert from '../Common/ErrorAlert';
import './BookingsList.css';

/**
 * BookingsList component displays user's bookings.
 */
export default function BookingsList({ onRefresh }) {
  const { user } = useContext(UserContext);
  const [bookings, setBookings] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadBookings = async () => {
      try {
        setLoading(true);
        if (user) {
          const data = await fetchUserBookings(user.id);
          setBookings(data);
        }
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    loadBookings();
  }, [user, onRefresh]);

  const handleCancel = async (bookingId) => {
    try {
      await cancelBooking(bookingId);
      setBookings(bookings.filter((b) => b.id !== bookingId));
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) return <p className="loading">Loading bookings...</p>;
  if (error) return <ErrorAlert message={error} onDismiss={() => setError(null)} />;

  // Sort bookings by start time (most recent first)
  const sortedBookings = [...bookings].sort(
    (a, b) => new Date(b.startTime) - new Date(a.startTime)
  );

  return (
    <div className="bookings-list">
      <h2>My Bookings</h2>
      <div className="bookings-list__items">
        {sortedBookings.length === 0 ? (
          <p>You have no bookings yet.</p>
        ) : (
          sortedBookings.map((booking) => (
            <BookingCard
              key={booking.id}
              booking={booking}
              onCancel={handleCancel}
              showActions={true}
            />
          ))
        )}
      </div>
    </div>
  );
}

BookingsList.propTypes = {
  onRefresh: PropTypes.number,
};

BookingsList.defaultProps = {
  onRefresh: 0,
};
