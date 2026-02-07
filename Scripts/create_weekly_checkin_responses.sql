-- Create weekly_checkin_responses table for Check-in Submission (WO-3)
-- Run this against each tenant/org database

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'weekly_checkin_responses')
BEGIN
    CREATE TABLE [dbo].[weekly_checkin_responses] (
        [Id]                  BIGINT          IDENTITY(1,1) NOT NULL,
        [EmployeeId]          BIGINT          NOT NULL,
        [WeekNumber]          INT             NOT NULL,
        [Year]                INT             NOT NULL,
        [PulseScore]          INT             NULL,
        [TaskCompletionRate]  INT             NULL,
        [Wins]                NVARCHAR(4000)  NULL,
        [Blockers]            NVARCHAR(4000)  NULL,
        [ManagerNote]         NVARCHAR(2000)  NULL,
        [Status]              NVARCHAR(20)    NOT NULL DEFAULT 'DRAFT',
        [SubmittedAt]         DATETIME2       NULL,
        [ReviewedAt]          DATETIME2       NULL,
        [ReviewedBy]          BIGINT          NULL,
        [ManagerComment]      NVARCHAR(1000)  NULL,
        [IsActive]            BIT             NOT NULL DEFAULT 1,
        [CreatedBy]           BIGINT          NOT NULL,
        [CreatedOn]           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy]           BIGINT          NULL,
        [UpdatedOn]           DATETIME2       NULL,

        CONSTRAINT [PK_weekly_checkin_responses] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Unique constraint: one check-in per employee per week per year
    CREATE UNIQUE INDEX [IX_WeeklyCheckinResponses_Employee_Week_Year]
        ON [dbo].[weekly_checkin_responses] ([EmployeeId], [WeekNumber], [Year]);

    PRINT 'Table [weekly_checkin_responses] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [weekly_checkin_responses] already exists. Skipping.';
END
GO
