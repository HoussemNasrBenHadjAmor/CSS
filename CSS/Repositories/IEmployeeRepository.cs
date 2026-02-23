using HRManagement.Models;
using System.Collections.Generic;
using HRManagement.Repositories;


namespace HRManagement.Repositories
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAll();
        Employee GetById(int id);
        void IncreaseSalary(int id, decimal percentage);
        void Save();
    }
}