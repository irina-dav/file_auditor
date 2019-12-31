CREATE TABLE [dbo].[RuleAudit] (
    [RuleId]            UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [Folder]            NVARCHAR (MAX)   NULL,
    [IncludeSubfolders] BIT              NULL,
    [Email]             NVARCHAR (MAX)   NULL,
    [Notify]            BIT              NULL,
    [MasksInclude]      NVARCHAR (MAX)   NULL,
    [MasksExclude]      NVARCHAR (MAX)   NULL,
    [FileEvents]        INT              NULL,
    [State]             INT              NULL,
    [ClientInfoId]      UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([RuleId] ASC)
);

