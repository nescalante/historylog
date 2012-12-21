CREATE TABLE [dbo].[Entities] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (100) NOT NULL,
    [ApplicationId] INT           NOT NULL,
    CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Entities_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Entities_Name]
    ON [dbo].[Entities]([ApplicationId] ASC, [Name] ASC);

