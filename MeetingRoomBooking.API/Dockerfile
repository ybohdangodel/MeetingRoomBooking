# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MeetingRoomBooking.API/MeetingRoomBooking.API.csproj", "MeetingRoomBooking.API/"]
COPY ["MeetingRoomBooking.Tests/MeetingRoomBooking.Tests.csproj", "MeetingRoomBooking.Tests/"]

RUN dotnet restore "MeetingRoomBooking.API/MeetingRoomBooking.API.csproj"

COPY . .
RUN dotnet build "MeetingRoomBooking.API/MeetingRoomBooking.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MeetingRoomBooking.API/MeetingRoomBooking.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "MeetingRoomBooking.API.dll"]
