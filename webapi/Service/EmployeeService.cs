using Microsoft.Extensions.Caching.Memory;

namespace webapi;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ICache _cache;
    private readonly ILogger<EmployeeRepository> _logger;
    //private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public EmployeeService(IEmployeeRepository repository, ICache cache, ILogger<EmployeeRepository> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        _logger.LogInformation("Getting employees from service.");

        if (_cache.TryGetValue("employees", out IEnumerable<Employee> cachedEmployees))
        {
            _logger.LogInformation("Retrieved employees from cache.");
            return cachedEmployees;
        }

        var employees = await _repository.GetEmployees();
        _cache.Set("employees", employees);
        _logger.LogInformation("Retrieved employees from repository and cached.");

        return employees;
    }

    public async Task<Employee> GetEmployeeById(Guid id)
    {
        _logger.LogInformation($"Getting employee by ID {id} from service.");
        return await _repository.GetEmployeeById(id);
    }

    public async Task AddEmployee(Employee employee)
    {
        _logger.LogInformation("Adding employee from service.");
        await _repository.AddEmployee(employee);

        var employees = await _repository.GetEmployees();
        _cache.Set("employees", employees);
        _logger.LogInformation("Retrieved employees from repository and cached.");
    }

    public async Task UpdateEmployee(Guid id, Employee updatedEmployee)
    {
        _logger.LogInformation($"Updating employee with ID {id} from service.");
        await _repository.UpdateEmployee(id, updatedEmployee);

        var employees = await _repository.GetEmployees();
        _cache.Set("employees", employees);
        _logger.LogInformation("Retrieved employees from repository and cached.");
    }

    public async Task DeleteEmployee(Guid id)
    {
        _logger.LogInformation($"Deleting employee with ID {id} from service.");
        await _repository.DeleteEmployee(id);

        var employees = await _repository.GetEmployees();
        _cache.Set("employees", employees);
        _logger.LogInformation("Retrieved employees from repository and cached.");
    }
}
