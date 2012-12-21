CREATE TABLE [dbo].[Logs] (
    [Id]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [EntityId] INT           NOT NULL,
    [Date]     DATETIME      NOT NULL,
    [User]     VARCHAR (100) NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Logs_Entities] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entities] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Logs_Entity]
    ON [dbo].[Logs]([EntityId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Logs_User]
    ON [dbo].[Logs]([User] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Logs_Date]
    ON [dbo].[Logs]([Date] ASC);

