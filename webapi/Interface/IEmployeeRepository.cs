namespace webapi;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployees();
    Task<Employee> GetEmployeeById(Guid id);
    Task AddEmployee(Employee employee);
    Task UpdateEmployee(Guid id, Employee employee);
    Task DeleteEmployee(Guid id);
}
