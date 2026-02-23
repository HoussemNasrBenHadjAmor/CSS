CREATE DATABASE HRDB;
GO

USE HRDB;
GO

CREATE TABLE Employees
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100),
    Salary DECIMAL(18,2),
    HireDate DATETIME
);
GO

INSERT INTO Employees (FirstName, LastName, Department, Salary, HireDate)
VALUES
('John', 'Smith', 'IT', 5000, '2023-01-10'),
('Sarah', 'Johnson', 'HR', 4500, '2023-02-15'),
('Mike', 'Brown', 'IT', 6000, '2023-04-20'),
('Anna', 'Taylor', 'Finance', 7000, '2023-05-01');
GO

CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentName NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Employees
    WHERE Department = @DepartmentName
END
GO

CREATE PROCEDURE sp_IncreaseSalary
    @EmployeeId INT,
    @Percentage DECIMAL(5,2)
AS
BEGIN
    UPDATE Employees
    SET Salary = Salary + (Salary * @Percentage / 100)
    WHERE Id = @EmployeeId
END
GO

CREATE VIEW vw_DepartmentSalarySummary
AS
SELECT
    Department,
    COUNT(*) AS TotalEmployees,
    AVG(Salary) AS AverageSalary,
    SUM(Salary) AS TotalSalary
FROM Employees
GROUP BY Department;
GO