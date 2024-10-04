-- EVs table

USE `EV C&S system`;

CREATE TABLE EVs (
    ev_id INT PRIMARY KEY,
    owner_id INT,
    model VARCHAR(50),
    battery_capacity FLOAT,
    soc FLOAT, -- State of charge
    priority_level INT
);

-- ChargingStations table
CREATE TABLE ChargingStations (
    station_id INT PRIMARY KEY,
    location VARCHAR(100),
    total_slots INT,
    available_slots INT
);

-- ChargingSessions table
CREATE TABLE ChargingSessions (
    session_id INT PRIMARY KEY,
    ev_id INT,
    station_id INT,
    start_time DATETIME,
    end_time DATETIME,
    energy_used FLOAT,
    cost FLOAT,
    FOREIGN KEY (ev_id) REFERENCES EVs(ev_id),
    FOREIGN KEY (station_id) REFERENCES ChargingStations(station_id)
);

-- Tariffs table
CREATE TABLE Tariffs (
    tariff_id INT PRIMARY KEY,
    station_id INT,
    time_period VARCHAR(50),
    cost_per_kWh FLOAT,
    FOREIGN KEY (station_id) REFERENCES ChargingStations(station_id)
);

-- Customers table
CREATE TABLE Customers (
    owner_id INT PRIMARY KEY,
    name VARCHAR(50),
    membership_level VARCHAR(20),
    contact_info VARCHAR(100)
);
