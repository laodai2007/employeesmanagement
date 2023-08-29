using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetEmployees()
    {
        try
        {
            _logger.LogInformation("Getting employees from controller.");

            var employees = await _employeeService.GetEmployees();

            _logger.LogInformation($"Retrieved {employees.Count()} employees.");
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching employees.");
            return StatusCode(500, "Internal server error");
        }

    }

    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        _logger.LogInformation("Fetching employee by ID from controller.");

        try
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                _logger.LogInformation($"Employee with ID {id} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Retrieved employee with ID {id} from controller.");
            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching employee by ID.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
    {
        _logger.LogInformation("Adding new employee from controller.");

        try
        {
            await _employeeService.AddEmployee(employee);
            _logger.LogInformation($"Added new employee with ID {employee.Id} from controller.");

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding employee.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    //[Authorize]
    public async Task<IActionResult> UpdateEmployee(Guid id, Employee employee)
    {
        _logger.LogInformation("Updating employee from controller.");

        try
        {
            var existingEmployee = await _employeeService.GetEmployeeById(id);
            if (existingEmployee == null)
            {
                _logger.LogInformation($"Employee with ID {id} not found.");
                return NotFound();
            }

            await _employeeService.UpdateEmployee(id, employee);
            _logger.LogInformation($"Updated employee with ID {id} from controller.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating employee.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        _logger.LogInformation("Deleting employee from controller.");

        try
        {
            var existingEmployee = await _employeeService.GetEmployeeById(id);
            if (existingEmployee == null)
            {
                _logger.LogInformation($"Employee with ID {id} not found.");
                return NotFound();
            }

            await _employeeService.DeleteEmployee(id);
            _logger.LogInformation($"Deleted employee with ID {id} from controller.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting employee.");
            return StatusCode(500, "Internal server error");
        }
    }
}
