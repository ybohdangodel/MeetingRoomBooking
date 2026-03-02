import React, { useState } from 'react';
import RoomList from '../components/Rooms/RoomList';
import BookingForm from '../components/Bookings/BookingForm';
import BookingsList from '../components/Bookings/BookingsList';
import './EmployeePage.css';

/**
 * EmployeePage component for employees to book rooms.
 */
export default function EmployeePage() {
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleRoomSelect = (room) => {
    setSelectedRoom(room);
  };

  const handleBookingSuccess = () => {
    setSelectedRoom(null);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <div className="employee-page">
      <div className="page-container">
        <h1>Employee Portal</h1>

        {selectedRoom ? (
          <BookingForm
            room={selectedRoom}
            onSuccess={handleBookingSuccess}
            onCancel={() => setSelectedRoom(null)}
          />
        ) : (
          <>
            <RoomList onRoomSelect={handleRoomSelect} />
            <BookingsList key={refreshKey} onRefresh={refreshKey} />
          </>
        )}
      </div>
    </div>
  );
}
