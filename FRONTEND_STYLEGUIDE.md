# Frontend Style Guide (React + Vite)

## Overview
This guide establishes standards for the React frontend. Follow these practices to ensure clean, maintainable, and testable components.

---

## Project Structure

```
src/
├── components/                      # Reusable UI components
│   ├── Common/
│   │   ├── Header.jsx
│   │   ├── Navigation.jsx
│   │   └── ErrorAlert.jsx
│   ├── Rooms/
│   │   ├── RoomCard.jsx
│   │   ├── RoomList.jsx
│   │   └── RoomForm.jsx
│   └── Bookings/
│       ├── BookingForm.jsx
│       ├── BookingsList.jsx
│       └── BookingCard.jsx
├── pages/                          # Page-level components
│   ├── EmployeePage.jsx
│   ├── AdminPage.jsx
│   └── LoginPage.jsx
├── services/                       # API & utilities
│   ├── api.js                      # Axios instance & HTTP calls
│   └── auth.js                     # Authentication helpers
├── context/                        # Global state (Context API)
│   ├── UserContext.jsx
│   └── BookingContext.jsx
├── App.jsx                         # Root component
├── main.jsx                        # Entry point
└── index.css                       # Global styles
```

---

## Naming Conventions

### Components
- **PascalCase** for all component files and names.
- **Example**: `RoomCard.jsx`, `BookingForm.jsx`, `EmployeePage.jsx`.

### Functions, Variables, State
- **camelCase** for non-component functions, variables, and state names.
- **Example**: `const fetchRooms = ...; const [isLoading, setIsLoading] = ...;`

### Files
- By default, use `.jsx` extension for files containing JSX.
- Utilities can use `.js`.

### Props & Event Handlers
- Prefix event handlers with `on`: `onClick`, `onChange`, `onSubmit`.
- Prefix handler functions with `handle`: `handleSubmit`, `handleClick`.

```jsx
// ✅ Good
<button onClick={handleBookingSubmit}>Book Room</button>
<input onChange={handleRoomSelect} />

// ❌ Bad
<button onClick={bookingSubmit}>Book Room</button>
<input onChange={roomSelect} />
```

---

## Component Structure

### Functional Components Only
Use React functional components with hooks. No class components.

```jsx
// ✅ Good
import PropTypes from 'prop-types';

export default function RoomCard({ room, onBook }) {
  return (
    <div className="room-card">
      <h3>{room.name}</h3>
      <p>Capacity: {room.capacity}</p>
      <button onClick={() => onBook(room.id)}>Book</button>
    </div>
  );
}

RoomCard.propTypes = {
  room: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    capacity: PropTypes.number.isRequired,
  }).isRequired,
  onBook: PropTypes.func.isRequired,
};

RoomCard.defaultProps = {};
```

### One Component Per File
- File name matches component name.
- Keep components focused and single-responsibility.

---

## Hooks & State Management

### State with `useState`
- Use `useState` for local component state.
- Keep state at the lowest level that needs it.

```jsx
export default function BookingForm() {
  const [roomId, setRoomId] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await createBooking({ roomId, startTime, endTime });
      // Success feedback
    } catch (error) {
      // Error feedback
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Form fields */}
    </form>
  );
}
```

### Global State with Context API
Use Context to share auth state (current user, role) across the app.

```jsx
// UserContext.jsx
import { createContext, useState } from 'react';

export const UserContext = createContext();

export function UserProvider({ children }) {
  const [user, setUser] = useState(null);

  const login = (userId, userRole) => {
    setUser({ id: userId, role: userRole });
  };

  const logout = () => {
    setUser(null);
  };

  return (
    <UserContext.Provider value={{ user, login, logout }}>
      {children}
    </UserContext.Provider>
  );
}

// In a component:
import { useContext } from 'react';
import { UserContext } from './context/UserContext';

export default function Header() {
  const { user, logout } = useContext(UserContext);

  return (
    <header>
      {user && <span>Welcome, {user.id}</span>}
      <button onClick={logout}>Logout</button>
    </header>
  );
}
```

### Effects with `useEffect`
- Fetch data in `useEffect`, not in render.
- Include proper dependency arrays.

```jsx
import { useState, useEffect } from 'react';
import { fetchRooms } from '../services/api';

export default function RoomList() {
  const [rooms, setRooms] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadRooms = async () => {
      try {
        const data = await fetchRooms();
        setRooms(data);
      } catch (err) {
        setError(err.message);
      }
    };

    loadRooms();
  }, []);

  if (error) return <ErrorAlert message={error} />;
  return <div>{rooms.map(room => <RoomCard key={room.id} room={room} />)}</div>;
}
```

---

## API Integration

### Centralized API Service
Create a single axios instance for all HTTP calls:

```jsx
// services/api.js
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
});

// Add user ID to every request (hardcoded auth)
apiClient.interceptors.request.use((config) => {
  const userId = localStorage.getItem('userId');
  if (userId) {
    config.headers['X-User-Id'] = userId;
  }
  return config;
});

// Handle errors globally
apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const message = error.response?.data?.error || 'An error occurred';
    return Promise.reject(new Error(message));
  }
);

export const fetchRooms = async () => apiClient.get('/rooms');
export const createBooking = async (payload) => apiClient.post('/bookings', payload);
export const cancelBooking = async (bookingId) => apiClient.put(`/bookings/${bookingId}/cancel`);
export const fetchMyBookings = async (userId) => apiClient.get(`/bookings?userId=${userId}`);
export const updateRoom = async (roomId, payload) => apiClient.put(`/rooms/${roomId}`, payload);

export default apiClient;
```

### Usage in Components
```jsx
import { fetchRooms, createBooking } from '../services/api';

export default function RoomList() {
  useEffect(() => {
    fetchRooms()
      .then(setRooms)
      .catch(err => setError(err.message));
  }, []);

  const handleBook = async (roomId) => {
    try {
      await createBooking({ roomId, startTime, endTime });
      // Refresh list or show success
    } catch (err) {
      setError(err.message);
    }
  };
}
```

---

## Error Handling & User Feedback

### Error Alerts
Create a reusable error component:

```jsx
// components/Common/ErrorAlert.jsx
export default function ErrorAlert({ message, onDismiss }) {
  return (
    <div className="alert alert-error">
      <span>{message}</span>
      <button onClick={onDismiss}>✕</button>
    </div>
  );
}
```

### Usage
```jsx
const [error, setError] = useState(null);

return (
  <>
    {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}
    {/* Rest of component */}
  </>
);
```

---

## Styling

### CSS Approach
- Use plain CSS modules or global stylesheet.
- Class names: BEM-like naming (`component__element--modifier`).

```css
/* index.css */
.room-card {
  border: 1px solid #ddd;
  padding: 1rem;
  border-radius: 8px;
}

.room-card__title {
  font-size: 1.25rem;
  font-weight: bold;
}

.button--primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  cursor: pointer;
}

.button--primary:hover {
  background-color: #0056b3;
}
```

### Avoiding Inline Styles
Prefer CSS classes over inline `style` props.

```jsx
// ✅ Good
<div className="button--primary">Book</div>

// ❌ Bad
<div style={{ backgroundColor: '#007bff', color: 'white' }}>Book</div>
```

---

## Props & PropTypes

### Define PropTypes
Always define PropTypes for component props:

```jsx
import PropTypes from 'prop-types';

export default function BookingCard({ booking, onCancel }) {
  return <div>Booking: {booking.roomId}</div>;
}

BookingCard.propTypes = {
  booking: PropTypes.shape({
    id: PropTypes.number.isRequired,
    roomId: PropTypes.number.isRequired,
    startTime: PropTypes.string.isRequired,
    endTime: PropTypes.string.isRequired,
    status: PropTypes.oneOf(['Pending', 'Confirmed', 'Canceled']).isRequired,
  }).isRequired,
  onCancel: PropTypes.func.isRequired,
};

BookingCard.defaultProps = {
  // If any props are optional, provide defaults here
};
```

### Avoid Prop Drilling
If props pass through many levels, use Context API instead:

```jsx
// ❌ Prop drilling (avoid)
<GrandParent user={user}>
  <Parent user={user}>
    <Child user={user} />
  </Parent>
</GrandParent>

// ✅ Use Context
<UserProvider>
  <GrandParent>
    <Parent>
      <Child />  {/* useContext(UserContext) here */}
    </Parent>
  </GrandParent>
</UserProvider>
```

---

## Routing

Use React Router for page navigation:

```jsx
// App.jsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { UserProvider } from './context/UserContext';
import LoginPage from './pages/LoginPage';
import EmployeePage from './pages/EmployeePage';
import AdminPage from './pages/AdminPage';

export default function App() {
  return (
    <UserProvider>
      <Router>
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/employee" element={<EmployeePage />} />
          <Route path="/admin" element={<AdminPage />} />
        </Routes>
      </Router>
    </UserProvider>
  );
}
```

---

## Comments & Code Quality

### Meaningful Comments
Comment *why* decisions were made, not *what* the code does.

```jsx
// ✅ Good
// Fetch rooms only once on component mount
useEffect(() => {
  fetchRooms().then(setRooms);
}, []);

// ❌ Bad
// Set rooms from fetchRooms
const rooms = fetchRooms();
```

### Avoid Console Logs
Remove `console.log` before committing. Use proper error handling instead.

---

## Testing

### Component Tests with Jest
```jsx
// RoomCard.test.jsx
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import RoomCard from './RoomCard';

describe('RoomCard', () => {
  it('renders room name and capacity', () => {
    const room = { id: 1, name: 'Conference A', capacity: 10 };
    render(<RoomCard room={room} onBook={() => {}} />);

    expect(screen.getByText('Conference A')).toBeInTheDocument();
    expect(screen.getByText(/Capacity: 10/)).toBeInTheDocument();
  });

  it('calls onBook callback when button clicked', async () => {
    const room = { id: 1, name: 'Conference A', capacity: 10 };
    const mockOnBook = jest.fn();
    const user = userEvent.setup();

    render(<RoomCard room={room} onBook={mockOnBook} />);
    await user.click(screen.getByText('Book'));

    expect(mockOnBook).toHaveBeenCalledWith(1);
  });
});
```

---

## Environment Variables

### .env & .env.local
```
VITE_API_BASE_URL=http://localhost:5000/api
```

Access in code:
```jsx
const apiUrl = import.meta.env.VITE_API_BASE_URL;
```

---

## ESLint & Prettier (Optional but Recommended)

### ESLint Config
```json
{
  "extends": ["eslint:recommended", "plugin:react/recommended"],
  "rules": {
    "react/react-in-jsx-scope": "off",
    "react/prop-types": "warn"
  }
}
```

### Prettier Config
```json
{
  "semi": true,
  "singleQuote": true,
  "trailingComma": "es5"
}
```

---

## Summary Checklist
- ✅ PascalCase for components, camelCase for functions/state.
- ✅ One component per file.
- ✅ PropTypes defined for all components.
- ✅ Use hooks (useState, useEffect, useContext); no class components.
- ✅ Centralized API service.
- ✅ Global state in Context API.
- ✅ Error handling with user feedback.
- ✅ CSS classes over inline styles.
- ✅ Meaningful comments only.
- ✅ Tests cover core functionality.
