import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { fetchRooms } from '../../services/api';
import RoomCard from './RoomCard';
import ErrorAlert from '../Common/ErrorAlert';
import './RoomList.css';

/**
 * RoomList component displays all available rooms.
 */
export default function RoomList({ onRoomSelect }) {
  const [rooms, setRooms] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadRooms = async () => {
      try {
        setLoading(true);
        const data = await fetchRooms();
        setRooms(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    loadRooms();
  }, []);

  if (loading) return <p className="loading">Loading rooms...</p>;
  if (error) return <ErrorAlert message={error} onDismiss={() => setError(null)} />;

  return (
    <div className="room-list">
      <h2>Available Rooms</h2>
      <div className="room-list__grid">
        {rooms.length === 0 ? (
          <p>No rooms available.</p>
        ) : (
          rooms.map((room) => (
            <RoomCard key={room.id} room={room} onSelect={onRoomSelect} />
          ))
        )}
      </div>
    </div>
  );
}

RoomList.propTypes = {
  onRoomSelect: PropTypes.func.isRequired,
};
