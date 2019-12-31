CREATE TABLE [dbo].[Client] (
    [ClientInfoId]         UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [ComputerName]         NVARCHAR (MAX)   NULL,
    [IpAddress]            NVARCHAR (MAX)   NULL,
    [LastEventDateTimeUtc] DATETIME         NULL,
    [State]                INT              NULL,
    PRIMARY KEY CLUSTERED ([ClientInfoId] ASC)
);

