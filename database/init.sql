-- ============================================
-- 0. SESSION SETTINGS
-- ============================================
SET NOCOUNT ON;
SET XACT_ABORT ON;

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

-- Drop database if exists
IF DB_ID('HRDB') IS NOT NULL
    DROP DATABASE HRDB;
GO

-- Create database
CREATE DATABASE HRDB;
GO

USE HRDB;
GO

-- ============================================
-- 2. TABLES
-- ============================================

-- Departments
CREATE TABLE Departments
(
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Roles
CREATE TABLE Roles
(
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Employees
CREATE TABLE Employees
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    FullName AS (FirstName + ' ' + LastName) PERSISTED,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Phone NVARCHAR(20),
    DepartmentId INT NOT NULL,
    BaseSalary DECIMAL(18,2) NOT NULL CHECK (BaseSalary > 0),
    HireDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    RowVersion ROWVERSION,

    CONSTRAINT FK_Employees_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
);
GO

-- EmployeeRoles (Many-to-Many)
CREATE TABLE EmployeeRoles
(
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),

    PRIMARY KEY (EmployeeId, RoleId),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO

-- Salary History (Audit)
CREATE TABLE SalaryHistory
(
    SalaryHistoryId INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    OldSalary DECIMAL(18,2),
    NewSalary DECIMAL(18,2),
    ChangeDate DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);
GO

-- Attendance
CREATE TABLE Attendance
(
    AttendanceId INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Present','Absent','Remote','Leave')),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);
GO

-- Performance Reviews
CREATE TABLE PerformanceReviews
(
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    ReviewDate DATE NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comments NVARCHAR(500),

    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);
GO

-- ============================================
-- 3. INDEXING (Performance Optimization)
-- ============================================

CREATE INDEX IX_Employees_DepartmentId ON Employees(DepartmentId);
CREATE INDEX IX_Employees_HireDate ON Employees(HireDate);
CREATE INDEX IX_Employees_IsActive ON Employees(IsActive)
INCLUDE (FirstName, LastName, BaseSalary);
GO

-- ============================================
-- 4. FUNCTIONS
-- ============================================

-- Scalar Function: Annual Salary
CREATE FUNCTION fn_GetAnnualSalary(@EmployeeId INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Salary DECIMAL(18,2);

    SELECT @Salary = BaseSalary * 12
    FROM Employees
    WHERE Id = @EmployeeId;

    RETURN @Salary;
END;
GO

-- Table-Valued Function: Active Employees
CREATE FUNCTION fn_GetActiveEmployees()
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM Employees
    WHERE IsActive = 1
);
GO

-- ============================================
-- 5. TRIGGERS
-- ============================================

CREATE TRIGGER trg_AuditSalaryChange
ON Employees
AFTER UPDATE
AS
BEGIN
    IF UPDATE(BaseSalary)
    BEGIN
        INSERT INTO SalaryHistory (EmployeeId, OldSalary, NewSalary)
        SELECT d.Id, d.BaseSalary, i.BaseSalary
        FROM deleted d
        INNER JOIN inserted i ON d.Id = i.Id
        WHERE d.BaseSalary <> i.BaseSalary;
    END
END;
GO

-- ============================================
-- 6. STORED PROCEDURES
-- ============================================

-- Increase Salary with Transaction
CREATE PROCEDURE sp_IncreaseSalary
    @EmployeeId INT,
    @Percentage DECIMAL(5,2)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Employees
        SET BaseSalary = BaseSalary + (BaseSalary * @Percentage / 100)
        WHERE Id = @EmployeeId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Get Employees by Department
CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentId INT
AS
BEGIN
    SELECT e.*
    FROM Employees e
    WHERE e.DepartmentId = @DepartmentId
      AND e.IsActive = 1;
END;
GO

-- ============================================
-- 7. VIEW (For Power BI Reporting)
-- ============================================

CREATE VIEW vw_DepartmentSalarySummary
AS
SELECT 
    d.DepartmentName,
    COUNT(e.Id) AS TotalEmployees,
    AVG(e.BaseSalary) AS AverageSalary,
    SUM(e.BaseSalary) AS TotalPayroll
FROM Departments d
LEFT JOIN Employees e ON d.DepartmentId = e.DepartmentId
GROUP BY d.DepartmentName;
GO

-- Hires by Month
CREATE VIEW vw_HiresByMonth
AS
SELECT
    DATEFROMPARTS(YEAR(e.HireDate), MONTH(e.HireDate), 1) AS MonthStart,
    COUNT(*) AS Hires
FROM Employees e
GROUP BY DATEFROMPARTS(YEAR(e.HireDate), MONTH(e.HireDate), 1);
GO

-- Attendance Summary
CREATE VIEW vw_AttendanceSummary
AS
SELECT
    d.DepartmentName,
    a.Status,
    COUNT(*) AS TotalRecords
FROM Attendance a
INNER JOIN Employees e ON a.EmployeeId = e.Id
INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
GROUP BY d.DepartmentName, a.Status;
GO

-- Salary Changes Over Time
CREATE VIEW vw_SalaryChanges
AS
SELECT
    e.Id AS EmployeeId,
    e.FullName,
    d.DepartmentName,
    sh.OldSalary,
    sh.NewSalary,
    sh.ChangeDate
FROM SalaryHistory sh
INNER JOIN Employees e ON sh.EmployeeId = e.Id
INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId;
GO

-- Salary Buckets (Distribution)
CREATE VIEW vw_SalaryBuckets
AS
SELECT
    CASE
        WHEN e.BaseSalary < 3000 THEN '< 3000'
        WHEN e.BaseSalary BETWEEN 3000 AND 4999 THEN '3000 - 4999'
        WHEN e.BaseSalary BETWEEN 5000 AND 6999 THEN '5000 - 6999'
        WHEN e.BaseSalary BETWEEN 7000 AND 8999 THEN '7000 - 8999'
        ELSE '9000+'
    END AS SalaryRange,
    COUNT(*) AS EmployeeCount
FROM Employees e
GROUP BY
    CASE
        WHEN e.BaseSalary < 3000 THEN '< 3000'
        WHEN e.BaseSalary BETWEEN 3000 AND 4999 THEN '3000 - 4999'
        WHEN e.BaseSalary BETWEEN 5000 AND 6999 THEN '5000 - 6999'
        WHEN e.BaseSalary BETWEEN 7000 AND 8999 THEN '7000 - 8999'
        ELSE '9000+'
    END;
GO

-- Performance Summary
CREATE VIEW vw_PerformanceSummary
AS
SELECT
    d.DepartmentName,
    AVG(CAST(pr.Rating AS DECIMAL(10,2))) AS AverageRating,
    COUNT(*) AS TotalReviews
FROM PerformanceReviews pr
INNER JOIN Employees e ON pr.EmployeeId = e.Id
INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
GROUP BY d.DepartmentName;
GO

-- ============================================
-- 8. SEED DATA
-- ============================================

INSERT INTO Departments (DepartmentName)
VALUES ('IT'), ('HR'), ('Finance'), ('Marketing'), ('Operations');

INSERT INTO Roles (RoleName)
VALUES ('Manager'), ('Developer'), ('Analyst'),
       ('Accountant'), ('HR Specialist');

-- ============================================
-- 9. MASSIVE DATA GENERATION (1000 Employees)
-- ============================================

DECLARE @Counter INT = 1;

WHILE @Counter <= 1000
BEGIN
    INSERT INTO Employees
    (FirstName, LastName, Email, Phone, DepartmentId, BaseSalary, HireDate)
    VALUES
    (
        'Employee' + CAST(@Counter AS NVARCHAR),
        'Last' + CAST(@Counter AS NVARCHAR),
        'employee' + CAST(@Counter AS NVARCHAR) + '@company.com',
        '050000000' + CAST(@Counter % 10 AS NVARCHAR),
        ABS(CHECKSUM(NEWID()) % 5) + 1,
        3000 + ABS(CHECKSUM(NEWID()) % 7000),
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 2000), GETDATE())
    );

    SET @Counter = @Counter + 1;
END;
GO

-- ============================================
-- 9. MASSIVE Attendance GENERATION (1000 Employees)
-- ============================================

DECLARE @Counter INT = 1;

WHILE @Counter <= 1000
BEGIN
    INSERT INTO Attendance (EmployeeId, AttendanceDate, Status)
    VALUES
    (
        @Counter,  -- EmployeeId (from 1 to 1000)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),  -- Random attendance date within the last 30 days
        CASE 
            WHEN @Counter % 4 = 0 THEN 'Present'  -- Example: Every 4th employee is marked as "Present"
            WHEN @Counter % 4 = 1 THEN 'Absent'   -- Every 4th employee is marked as "Absent"
            WHEN @Counter % 4 = 2 THEN 'Remote'   -- Every 4th employee is marked as "Remote"
            ELSE 'Leave'                         -- The rest are on "Leave"
        END
    );

    SET @Counter = @Counter + 1;
END;
GO


-- ============================================
-- 10. MASSIVE PerformanceReviews GENERATION (1000 Employees)
-- ============================================

DECLARE @Counter INT = 1;

WHILE @Counter <= 1000
BEGIN
    INSERT INTO PerformanceReviews (EmployeeId, ReviewDate, Rating, Comments)
    VALUES
    (
        @Counter,  -- EmployeeId (from 1 to 1000)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 365), GETDATE()),  -- Random review date within the last year
        ABS(CHECKSUM(NEWID()) % 5) + 1,  -- Random rating between 1 and 5
        'Automated review comment for Employee ' + CAST(@Counter AS NVARCHAR)  -- Simple review comment
    );

    SET @Counter = @Counter + 1;
END;
GO

-- ============================================
-- 11. Populating the SalaryHistory Table
-- ============================================

DECLARE @Counter INT = 1;

WHILE @Counter <= 1000
BEGIN
    INSERT INTO SalaryHistory (EmployeeId, OldSalary, NewSalary, ChangeDate)
    VALUES
    (
        @Counter,  -- EmployeeId (from 1 to 1000)
        3000 + ABS(CHECKSUM(NEWID()) % 7000),  -- Random old salary between 3000 and 10000
        3000 + ABS(CHECKSUM(NEWID()) % 7000),  -- Random new salary between 3000 and 10000
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 365), GETDATE())  -- Random date within the last year
    );

    SET @Counter = @Counter + 1;
END;
GO

-- Random Roles Assignment
INSERT INTO EmployeeRoles (EmployeeId, RoleId)
SELECT Id, ABS(CHECKSUM(NEWID()) % 5) + 1
FROM Employees;
GO

PRINT 'HRDB Enterprise Database Successfully Created!';