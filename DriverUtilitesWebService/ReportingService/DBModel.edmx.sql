
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/08/2011 01:15:34
-- Generated from EDMX file: C:\Users\Mina\Documents\Visual Studio 2010\Projects\Web Service\ReportingService\DBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [freemiumdata];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[NormalReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NormalReports];
GO
IF OBJECT_ID(N'[dbo].[Statistics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Statistics];
GO
IF OBJECT_ID(N'[dbo].[BugReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BugReports];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'NormalReports'
CREATE TABLE [dbo].[NormalReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] nvarchar(max)  NOT NULL,
    [Action] nvarchar(25)  NOT NULL,
    [Date] datetime  NOT NULL,
    [MAC] nchar(12)  NOT NULL,
    [IP] nvarchar(max)  NOT NULL,
    [OS] nvarchar(max)  NOT NULL,
    [HostName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Statistics'
CREATE TABLE [dbo].[Statistics] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] nvarchar(max)  NOT NULL,
    [Type] nvarchar(25)  NOT NULL,
    [Count] int  NOT NULL
);
GO

-- Creating table 'BugReports'
CREATE TABLE [dbo].[BugReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] nvarchar(max)  NOT NULL,
    [Date] datetime  NOT NULL,
    [MAC] nchar(12)  NOT NULL,
    [IP] nvarchar(max)  NOT NULL,
    [OS] nvarchar(max)  NOT NULL,
    [HostName] nvarchar(max)  NOT NULL,
    [BugStackTrace] nvarchar(max)  NOT NULL,
    [BugType] nvarchar(max)  NOT NULL,
    [BugMessage] nvarchar(max)  NOT NULL,
    [BugUserInput] nvarchar(max)  NOT NULL,
    [BugTargetSite] nvarchar(max)  NOT NULL,
    [BugSource] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Versions'
CREATE TABLE [dbo].[Versions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Number] nvarchar(max)  NOT NULL,
    [DownloadLink] nvarchar(max)  NOT NULL,
    [Date] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'NormalReports'
ALTER TABLE [dbo].[NormalReports]
ADD CONSTRAINT [PK_NormalReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Statistics'
ALTER TABLE [dbo].[Statistics]
ADD CONSTRAINT [PK_Statistics]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BugReports'
ALTER TABLE [dbo].[BugReports]
ADD CONSTRAINT [PK_BugReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Versions'
ALTER TABLE [dbo].[Versions]
ADD CONSTRAINT [PK_Versions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------