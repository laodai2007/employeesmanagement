namespace webapi
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployees();
        Task<Employee> GetEmployeeById(Guid id);
        Task AddEmployee(Employee employee);
        Task UpdateEmployee(Guid id, Employee updatedEmployee);
        Task DeleteEmployee(Guid id);
    }
}
