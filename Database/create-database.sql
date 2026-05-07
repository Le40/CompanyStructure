IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE TABLE [Companies] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [LeaderId] int NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [CompanyId] int NOT NULL,
        [Degree] nvarchar(25) NULL,
        [Name] nvarchar(50) NOT NULL,
        [Surname] nvarchar(100) NOT NULL,
        [Email] nvarchar(50) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE TABLE [Divisions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [LeaderId] int NULL,
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_Divisions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Divisions_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Divisions_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE TABLE [Projects] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [LeaderId] int NULL,
        [DivisionId] int NOT NULL,
        CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Projects_Divisions_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Projects_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [LeaderId] int NULL,
        [ProjectId] int NOT NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Departments_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id]),
        CONSTRAINT [FK_Departments_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Companies_LeaderId] ON [Companies] ([LeaderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Departments_LeaderId] ON [Departments] ([LeaderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Departments_ProjectId] ON [Departments] ([ProjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Divisions_CompanyId] ON [Divisions] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Divisions_LeaderId] ON [Divisions] ([LeaderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Projects_DivisionId] ON [Projects] ([DivisionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    CREATE INDEX [IX_Projects_LeaderId] ON [Projects] ([LeaderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    ALTER TABLE [Companies] ADD CONSTRAINT [FK_Companies_Employees_LeaderId] FOREIGN KEY ([LeaderId]) REFERENCES [Employees] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260502150534_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260502150534_Initial', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260503205333_DenormaliseCompanyId'
)
BEGIN
    ALTER TABLE [Projects] ADD [CompanyId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260503205333_DenormaliseCompanyId'
)
BEGIN
    ALTER TABLE [Departments] ADD [CompanyId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260503205333_DenormaliseCompanyId'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260503205333_DenormaliseCompanyId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260503205333_DenormaliseCompanyId', N'10.0.7');
END;

COMMIT;
GO

