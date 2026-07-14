/*
    Entities:
      Firmware  - versioned firmware that can be installed on a Device
      Group     - a group of devices, a Group can belong to a parent Group 
      Device    - a physical device, has exactly one current Firmware, optionally belongs to one Group
*/

CREATE DATABASE IoT_Device_Tracking;

USE IoT_Device_Tracking;

IF OBJECT_ID('dbo.Device', 'U') IS NOT NULL DROP TABLE dbo.Device;
IF OBJECT_ID('dbo.[Group]', 'U') IS NOT NULL DROP TABLE dbo.[Group];
IF OBJECT_ID('dbo.Firmwarel', 'U') IS NOT NULL DROP TABLE dbo.Firmware;
GO

CREATE TABLE dbo.Firmware (
    FirmwareID     INT IDENTITY(1,1) PRIMARY KEY,
    FirmwareName   NVARCHAR(100) NOT NULL,
    Version        NVARCHAR(20)  NOT NULL,
    ReleaseDate    DATE          NOT NULL,
    CONSTRAINT UQ_Firmware_Name_Version UNIQUE (FirmwareName, Version)
);
GO

CREATE TABLE dbo.[Group] (
    GroupID        INT IDENTITY(1,1) PRIMARY KEY,
    GroupName      NVARCHAR(100) NOT NULL,
    ParentGroupID  INT NULL,
    CONSTRAINT FK_Group_ParentGroup FOREIGN KEY (ParentGroupID)
        REFERENCES dbo.[Group](GroupID)
);
GO

CREATE TABLE dbo.Device (
    DeviceID       INT IDENTITY(1,1) PRIMARY KEY,
    SerialNumber   NVARCHAR(50)  NOT NULL,
    DeviceName     NVARCHAR(100) NOT NULL,
    FirmwareID     INT NOT NULL,
    GroupID        INT NULL,
    CONSTRAINT UQ_Device_SerialNumber UNIQUE (SerialNumber),
    CONSTRAINT FK_Device_Firmware FOREIGN KEY (FirmwareID)
        REFERENCES dbo.Firmware(FirmwareID),
    CONSTRAINT FK_Device_Group FOREIGN KEY (GroupID)
        REFERENCES dbo.[Group](GroupID)
);
GO

CREATE INDEX IX_Device_GroupID ON dbo.Device(GroupID);
CREATE INDEX IX_Device_FirmwareID ON dbo.Device(FirmwareID);
CREATE INDEX IX_Group_ParentGroupID ON dbo.[Group](ParentGroupID);
GO
