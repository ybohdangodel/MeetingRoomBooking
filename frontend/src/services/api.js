import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5077/api';

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
    const message = error.response?.data?.error || error.message || 'An error occurred';
    return Promise.reject(new Error(message));
  }
);

// Rooms API
export const fetchRooms = async () => apiClient.get('/rooms');
export const fetchRoom = async (roomId) => apiClient.get(`/rooms/${roomId}`);
export const updateRoom = async (roomId, payload) => apiClient.put(`/rooms/${roomId}`, payload);

// Bookings API
export const fetchAllBookings = async () => apiClient.get('/bookings');
export const fetchBooking = async (bookingId) => apiClient.get(`/bookings/${bookingId}`);
export const fetchUserBookings = async (userId) => apiClient.get(`/bookings/user/${userId}`);
export const createBooking = async (payload) => apiClient.post('/bookings', payload);
export const cancelBooking = async (bookingId) => apiClient.put(`/bookings/${bookingId}/cancel`);
export const approveBooking = async (bookingId) => apiClient.put(`/bookings/${bookingId}/approve`);

// Users API
export const getCurrentUser = async () => apiClient.get('/users/me');
export const getUser = async (userId) => apiClient.get(`/users/${userId}`);
export const fetchAllUsers = async () => apiClient.get('/users');

export default apiClient;
