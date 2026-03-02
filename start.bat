@echo off
REM Quick Start Script for Meeting Room Booking System

echo.
echo ========================================
echo  Meeting Room Booking - Quick Start
echo ========================================
echo.

REM Check if Docker is running
echo Checking for Docker...
docker --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Docker found. Starting PostgreSQL container...
    docker-compose up -d postgres
    echo Waiting for PostgreSQL to be ready...
    timeout /t 5 /nobreak
) else (
    echo Docker not found. Please ensure PostgreSQL is running locally on localhost:5432
)

echo.
echo Starting .NET API on http://localhost:5000...
start cmd /k "cd MeetingRoomBooking.API && dotnet run"

echo Waiting for API to start...
timeout /t 5 /nobreak

echo.
echo Starting React frontend on http://localhost:3000...
start cmd /k "cd frontend && npm run dev"

echo.
echo ========================================
echo  System is loading...
echo  Backend: http://localhost:5000/openapi
echo  Frontend: http://localhost:3000
echo  
echo  Test users:
echo    - John Employee (ID: 1, Role: Employee)
echo    - Jane Admin (ID: 2, Role: Admin)
echo    - Bob Employee (ID: 3, Role: Employee)
echo ========================================
echo.

pause
