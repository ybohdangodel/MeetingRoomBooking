# Meeting Room Booking System

An internal platform for booking meeting rooms and devices with separate interfaces for employees and administrators.

**Tech Stack:**
- Backend: .NET 10 Web API
- Frontend: React 18 + Vite
- Database: PostgreSQL
- Authentication: Hardcoded test users (for demo purposes)

---

## Quick Start

### Option 1: Docker Compose (Recommended)

**Prerequisites:** Docker Desktop installed

```bash
cd c:\work\MeetingRoomBooking
docker-compose up
```

This will start:
- PostgreSQL on `localhost:5432`
- Backend API on `http://localhost:5000`

Then start the frontend (in another terminal):
```bash
cd frontend
npm run dev
```

Frontend will be available on `http://localhost:3000`

---

### Option 2: Local Setup

#### Backend Setup

**Prerequisites:**
- .NET 10 SDK
- PostgreSQL 16+ (or another compatible version)

1. **Create the database:**
```bash
createdb -U postgres meetingroom
```

2. **Update connection string** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=meetingroom;Username=postgres;Password=postgres"
  }
}
```

3. **Run migrations and start API:**
```bash
cd MeetingRoomBooking.API
dotnet ef database update
dotnet run
```

API will be available on `https://localhost:5001` and `http://localhost:5000`

#### Frontend Setup

**Prerequisites:**
- Node.js 18+ (tested with v24.14.0)

```bash
cd frontend
npm install
npm run dev
```

Frontend will be available on `http://localhost:3000`

---

## Project Structure

```
MeetingRoomBooking/
├── MeetingRoomBooking.API/              # .NET backend
│   ├── Controllers/                     # HTTP endpoints
│   ├── Services/                        # Business logic
│   ├── Models/                          # Entities and DTOs
│   ├── Data/                            # DbContext and migrations
│   └── Middleware/                      # Auth, error handling
├── MeetingRoomBooking.Tests/            # .NET unit tests
├── frontend/                            # React frontend
│   ├── src/
│   │   ├── pages/                       # Employee & Admin pages
│   │   ├── components/                  # Reusable UI components
│   │   ├── services/                    # API client
│   │   └── context/                     # Global state (UserContext)
│   └── package.json
├── BACKEND_STYLEGUIDE.md                # Backend code standards
├── FRONTEND_STYLEGUIDE.md               # Frontend code standards
└── docker-compose.yml                   # Docker Compose configuration
```

---

## Features

### Employee Portal
- View available rooms
- Book rooms for specific time slots
- Manage personal bookings (view, cancel)
- Conflict detection (no overlapping bookings)

### Admin Panel
- View all bookings across the organization
- Approve/reject pending bookings
- Manage room details (capacity, location)
- Filter bookings by status

### Test Users

Login with one of these hardcoded test users:

| User         | Role     | ID |
|--------------|----------|-----|
| John Employee | Employee | 1   |
| Jane Admin    | Admin    | 2   |
| Bob Employee  | Employee | 3   |

---

## API Endpoints

### Rooms
- `GET /api/rooms` – List all rooms
- `GET /api/rooms/{id}` – Get room details
- `PUT /api/rooms/{id}` – Update room (admin only)

### Bookings
- `GET /api/bookings` – List all bookings (admin only)
- `GET /api/bookings/{id}` – Get booking details
- `GET /api/bookings/user/{userId}` – Get user's bookings
- `POST /api/bookings` – Create booking
- `PUT /api/bookings/{id}/cancel` – Cancel booking
- `PUT /api/bookings/{id}/approve` – Approve booking (admin only)

### Users
- `GET /api/users/me` – Get current user
- `GET /api/users/{id}` – Get user details
- `GET /api/users` – List all users (admin only)

---

## Development

### Code Style

Follow the style guides before contributing:
- [Backend Style Guide](BACKEND_STYLEGUIDE.md)
- [Frontend Style Guide](FRONTEND_STYLEGUIDE.md)

### Testing

**Backend:**
```bash
cd MeetingRoomBooking.API
dotnet test
```

**Frontend:**
```bash
cd frontend
npm test
```

### Build

**Backend:**
```bash
dotnet build
```

**Frontend:**
```bash
cd frontend
npm run build
```

---

## Notes

- This is a **student project** with simplified authentication (hardcoded users).
- In production, implement proper authentication (SSO, OAuth, JWT, etc.).
- Database credentials in `appsettings.json` are for local development only.
- All timestamps are stored in UTC.

---

## Troubleshooting

**PostgreSQL Connection Error:**
- Ensure PostgreSQL is running on `localhost:5432`
- Or use Docker Compose: `docker-compose up postgres`

**Frontend Won't Connect to API:**
- Check API is running on `http://localhost:5000`
- Check `VITE_API_BASE_URL` in `.env` or `frontend/.env.local`

**npm install fails:**
- Clear npm cache: `npm cache clean --force`
- Delete `node_modules` and `package-lock.json`, reinstall