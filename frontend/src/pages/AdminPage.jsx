import React, { useState, useEffect } from 'react';
import { fetchAllBookings, approveBooking, cancelBooking } from '../services/api';
import BookingCard from '../components/Bookings/BookingCard';
import ErrorAlert from '../components/Common/ErrorAlert';
import './AdminPage.css';

/**
 * AdminPage component for admins to manage bookings and rooms.
 */
export default function AdminPage() {
  const [bookings, setBookings] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('all');

  useEffect(() => {
    const loadBookings = async () => {
      try {
        setLoading(true);
        const data = await fetchAllBookings();
        setBookings(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    loadBookings();
  }, []);

  const handleApprove = async (bookingId) => {
    try {
      await approveBooking(bookingId);
      setBookings(
        bookings.map((b) =>
          b.id === bookingId ? { ...b, status: 'Confirmed' } : b
        )
      );
    } catch (err) {
      setError(err.message);
    }
  };

  const handleCancel = async (bookingId) => {
    try {
      await cancelBooking(bookingId);
      setBookings(
        bookings.map((b) =>
          b.id === bookingId ? { ...b, status: 'Canceled' } : b
        )
      );
    } catch (err) {
      setError(err.message);
    }
  };

  const filteredBookings = bookings.filter((b) => {
    if (filter === 'all') return true;
    return b.status === filter;
  });

  if (loading) return <p className="loading">Loading bookings...</p>;

  return (
    <div className="admin-page">
      <div className="page-container">
        <h1>Admin Panel</h1>

        {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

        <div className="filter-controls">
          <label htmlFor="statusFilter">Filter by Status:</label>
          <select
            id="statusFilter"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
          >
            <option value="all">All Bookings</option>
            <option value="Pending">Pending</option>
            <option value="Confirmed">Confirmed</option>
            <option value="Canceled">Canceled</option>
          </select>
        </div>

        <div className="bookings-list">
          <h2>All Bookings ({filteredBookings.length})</h2>
          <div className="bookings-list__items">
            {filteredBookings.length === 0 ? (
              <p>No bookings found.</p>
            ) : (
              filteredBookings.map((booking) => (
                <BookingCard
                  key={booking.id}
                  booking={booking}
                  onCancel={handleCancel}
                  onApprove={handleApprove}
                  showActions={true}
                />
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
