
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/14/2016 17:08:36
-- Generated from EDMX file: C:\Users\Jay\Documents\Visual Studio 2015\Projects\Final Project\Final-Project\FinalProjectAlpha\FinalProjectAlpha\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [wayback];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Archives_AspNetUsers1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Archives] DROP CONSTRAINT [FK_Archives_AspNetUsers1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Archives]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Archives];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'Archives'
CREATE TABLE [dbo].[Archives] (
    [Link] nvarchar(50)  NOT NULL,
    [ArchiveLink] nvarchar(100)  NOT NULL,
    [RepoLink] nvarchar(100)  NULL,
    [ShortDesc] nchar(200)  NOT NULL,
    [LongDesc] nvarchar(500)  NULL,
    [SnapShot] varbinary(max)  NULL,
    [UserID] nvarchar(128)  NOT NULL,
    [ProjectName] nvarchar(50)  NOT NULL,
    [ArchiveDate] datetime  NOT NULL,
    [TeamName] nvarchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Link] in table 'Archives'
ALTER TABLE [dbo].[Archives]
ADD CONSTRAINT [PK_Archives]
    PRIMARY KEY CLUSTERED ([Link] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserID] in table 'Archives'
ALTER TABLE [dbo].[Archives]
ADD CONSTRAINT [FK_Archives_AspNetUsers1]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Archives_AspNetUsers1'
CREATE INDEX [IX_FK_Archives_AspNetUsers1]
ON [dbo].[Archives]
    ([UserID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------