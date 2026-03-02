import React from 'react';
import PropTypes from 'prop-types';
import './RoomCard.css';

/**
 * RoomCard component displays room details and book button.
 */
export default function RoomCard({ room, onSelect }) {
  return (
    <div className="room-card">
      <h3 className="room-card__title">{room.name}</h3>
      <p className="room-card__detail">
        <strong>Building:</strong> {room.building}
      </p>
      <p className="room-card__detail">
        <strong>Capacity:</strong> {room.capacity} people
      </p>
      <p className="room-card__detail">
        <strong>Description:</strong> {room.description}
      </p>
      <button className="button button--primary" onClick={() => onSelect(room)}>
        Book Room
      </button>
    </div>
  );
}

RoomCard.propTypes = {
  room: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    building: PropTypes.string.isRequired,
    capacity: PropTypes.number.isRequired,
    description: PropTypes.string,
  }).isRequired,
  onSelect: PropTypes.func.isRequired,
};
