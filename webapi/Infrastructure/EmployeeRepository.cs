using System.Text.Json;

namespace webapi;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IEmployeeFilePathProvider _filePath;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(IEmployeeFilePathProvider filePathProvider, ILogger<EmployeeRepository> logger)
    {
        _filePath = filePathProvider;
        _logger = logger;
    }

    public async Task<List<Employee>> ReadEmployeesFromFileAsync()
    {
        _logger.LogInformation("Reading employees from file.");

        try
        {
            using (var fileStream = File.OpenRead(_filePath.GetEmployeeFilePath()))
            {
                return await JsonSerializer.DeserializeAsync<List<Employee>>(fileStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while reading employees from file.");
            throw;
        }
    }

    public async Task WriteEmployeesToFileAsync(List<Employee> employees)
    {
        _logger.LogInformation("Writing employees to file.");

        try
        {
            using (var fileStream = File.Create(_filePath.GetEmployeeFilePath()))
            {
                await JsonSerializer.SerializeAsync(fileStream, employees);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while writing employees to file.");
            throw;
        }
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        _logger.LogInformation("Getting employees from repository.");

        try
        {
            var employees = await ReadEmployeesFromFileAsync();
            return employees;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting employees from repository.");
            throw;
        }
    }

    public async Task<Employee> GetEmployeeById(Guid id)
    {
        _logger.LogInformation($"Getting employee by ID {id} from repository.");

        try
        {
            var employees = await ReadEmployeesFromFileAsync();
            return employees.FirstOrDefault(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while getting employee by ID {id} from repository.");
            throw;
        }
    }

    public async Task AddEmployee(Employee employee)
    {
        _logger.LogInformation("Adding employee to repository.");

        try
        {
            var employees = await ReadEmployeesFromFileAsync();
            employee.Id = Guid.NewGuid(); // Assign a new ID
            employees.Add(employee);
            await WriteEmployeesToFileAsync(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding employee to repository.");
            throw;
        }
    }

    public async Task UpdateEmployee(Guid id, Employee updatedEmployee)
    {
        _logger.LogInformation($"Updating employee with ID {id} in repository.");

        try
        {
            var employees = await ReadEmployeesFromFileAsync();
            var existingEmployee = employees.FirstOrDefault(e => e.Id == id);
            if (existingEmployee != null)
            {
                existingEmployee.Name = updatedEmployee.Name;
                existingEmployee.Position = updatedEmployee.Position;
                existingEmployee.HiringDate = updatedEmployee.HiringDate;
                existingEmployee.Salary = updatedEmployee.Salary;
                await WriteEmployeesToFileAsync(employees);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while updating employee with ID {id} in repository.");
            throw;
        }
    }

    public async Task DeleteEmployee(Guid id)
    {
        _logger.LogInformation($"Deleting employee with ID {id} from repository.");

        try
        {
            var employees = await ReadEmployeesFromFileAsync();
            var employeeToRemove = employees.FirstOrDefault(e => e.Id == id);
            if (employeeToRemove != null)
            {
                employees.Remove(employeeToRemove);
                await WriteEmployeesToFileAsync(employees);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting employee with ID {id} from repository.");
            throw;
        }
    }
}
