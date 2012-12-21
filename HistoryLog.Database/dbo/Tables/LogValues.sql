CREATE TABLE [dbo].[LogValues] (
    [LogId]          BIGINT         NOT NULL,
    [EntityColumnId] INT            NOT NULL,
    [Value]          NVARCHAR (500) NULL,
    CONSTRAINT [PK_LogValues] PRIMARY KEY CLUSTERED ([LogId] ASC, [EntityColumnId] ASC),
    CONSTRAINT [FK_LogValues_EntityColumns] FOREIGN KEY ([EntityColumnId]) REFERENCES [dbo].[EntityColumns] ([Id]),
    CONSTRAINT [FK_LogValues_Logs] FOREIGN KEY ([LogId]) REFERENCES [dbo].[Logs] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_LogValues]
    ON [dbo].[LogValues]([EntityColumnId] ASC, [LogId] ASC);

