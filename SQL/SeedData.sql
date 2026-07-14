/*
    Sample data
*/

INSERT INTO dbo.Firmware (FirmwareName, Version, ReleaseDate) VALUES
    ('Oyster Firmware', '1.0.0', '2024-01-15'),
    ('Oyster Firmware', '1.2.0', '2024-06-01'),
    ('Yabby Firmware',  '2.0.0', '2024-03-10'),
    ('Hawk Firmware',   '1.5.2', '2024-08-20');
GO


INSERT INTO dbo.[Group] (GroupName, ParentGroupID) VALUES ('Global Fleet', NULL);        
INSERT INTO dbo.[Group] (GroupName, ParentGroupID) VALUES ('South Africa', 1);           
INSERT INTO dbo.[Group] (GroupName, ParentGroupID) VALUES ('Australia', 1);               
INSERT INTO dbo.[Group] (GroupName, ParentGroupID) VALUES ('Johannesburg Depot', 2);      
INSERT INTO dbo.[Group] (GroupName, ParentGroupID) VALUES ('Cape Town Depot', 2);         
GO

INSERT INTO dbo.Device (SerialNumber, DeviceName, FirmwareID, GroupID) VALUES
    ('SN-0001', 'Oyster Tracker A', 2, 4),
    ('SN-0002', 'Oyster Tracker B', 1, 4),
    ('SN-0003', 'Yabby Tracker C',  3, 5),
    ('SN-0004', 'Hawk Logger D',    4, 3),
    ('SN-0005', 'Oyster Tracker E', 2, NULL); 
GO

-- Sanity check queries
SELECT * FROM dbo.Firmware;
SELECT * FROM dbo.[Group];
SELECT d.DeviceID, d.SerialNumber, d.DeviceName, f.FirmwareName, f.Version, g.GroupName
FROM dbo.Device d
JOIN dbo.Firmware f ON d.FirmwareID = f.FirmwareID
LEFT JOIN dbo.[Group] g ON d.GroupID = g.GroupID;
