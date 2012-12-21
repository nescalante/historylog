CREATE TABLE [dbo].[EntityColumns] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Name]     VARCHAR (100) NOT NULL,
    [EntityId] INT           NOT NULL,
    CONSTRAINT [PK_EntityColumns] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EntityColumns_Entities] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Entities] ([Id]),
    CONSTRAINT [IX_EntityColumns] UNIQUE NONCLUSTERED ([EntityId] ASC, [Name] ASC)
);

