CREATE TABLE [dbo].[EventInfo] (
    [EventInfoId]    UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [EventId]        INT              NULL,
    [EventRecordId]  BIGINT           NULL,
    [TimeCreatedUtc] DATETIME         NULL,
    [Computer]       NVARCHAR (MAX)   NULL,
    [UserName]       NVARCHAR (MAX)   NULL,
    [DomainName]     NVARCHAR (MAX)   NULL,
    [ObjectName]     NVARCHAR (MAX)   NULL,
    [AccessMask]     INT              NULL,
    [HandleId]       NVARCHAR (MAX)   NULL,
    [ClientInfoId]   UNIQUEIDENTIFIER NULL,
    [FileEvent]      INT              NULL,
    PRIMARY KEY CLUSTERED ([EventInfoId] ASC),
    CONSTRAINT [fk_EventInfo_Client] FOREIGN KEY ([ClientInfoId]) REFERENCES [dbo].[Client] ([ClientInfoId])
);

