using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webapi;
using webapi.Controllers;

namespace UnitTest
{
    public class EmployeesControllerTests
    {
        private readonly EmployeesController _controller;
        private readonly Mock<IEmployeeService> _mockEmployeeService;
        private readonly Mock<ILogger<EmployeesController>> _mockLogger;

        public EmployeesControllerTests()
        {
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockLogger = new Mock<ILogger<EmployeesController>>();
            _controller = new EmployeesController(_mockEmployeeService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetEmployees_ReturnsListOfEmployees()
        {
            // Arrange
            var expectedEmployees = new List<Employee>
            {
                new Employee { Id = new Guid("05e833b8-cd86-472e-a440-08db999a7231"), Name = "John Doe" },
                new Employee { Id = new Guid("05e833b8-cd86-472e-a440-08db999a7232"), Name = "Jane Smith" }
            };
            _mockEmployeeService.Setup(service => service.GetEmployees()).ReturnsAsync(expectedEmployees);

            // Act
            var result = await _controller.GetEmployees();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okObjectResult.Value);
            Assert.Equal(expectedEmployees.Count, employees.Count());
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsEmployee()
        {
            // Arrange
            var expectedEmployee = new Employee { Id = new Guid("05e833b8-cd86-472e-a440-08db999a7231"), Name = "John Doe" };
            _mockEmployeeService.Setup(service => service.GetEmployeeById(expectedEmployee.Id)).ReturnsAsync(expectedEmployee);

            // Act
            var result = await _controller.GetEmployee(expectedEmployee.Id);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var employee = Assert.IsType<Employee>(okObjectResult.Value);
            Assert.Equal(expectedEmployee.Id, employee.Id);
        }

        [Fact]
        public async Task GetEmployeeById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid invalidId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            _mockEmployeeService.Setup(service => service.GetEmployeeById(invalidId)).ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.GetEmployee(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddEmployee_ValidEmployee_ReturnsCreatedResult()
        {
            // Arrange
            var newEmployee = new Employee { Id = new Guid("05e833b8-cd86-472e-a440-08db999a7231"), Name = "Alice Johnson" };
            _mockEmployeeService.Setup(service => service.AddEmployee(newEmployee));

            // Act
            var result = await _controller.AddEmployee(newEmployee);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(EmployeesController.GetEmployee), createdResult.ActionName);
            Assert.Equal(newEmployee.Id, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task UpdateEmployee_ValidId_ReturnsNotFound()
        {
            // Arrange
            Guid employeeId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            var updatedEmployee = new Employee { Id = employeeId, Name = "Updated Name" };
            _mockEmployeeService.Setup(service => service.UpdateEmployee(employeeId, updatedEmployee));

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updatedEmployee);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateEmployee_InvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid invalidId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            var updatedEmployee = new Employee { Id = invalidId, Name = "Updated Name" };
            _mockEmployeeService.Setup(service => service.UpdateEmployee(invalidId, updatedEmployee))
                                .ThrowsAsync(new InvalidOperationException("Employee not found."));

            // Act
            var result = await _controller.UpdateEmployee(invalidId, updatedEmployee);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_InvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid invalidId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            _mockEmployeeService.Setup(service => service.DeleteEmployee(invalidId))
                                .ThrowsAsync(new InvalidOperationException("Employee not found."));

            // Act
            var result = await _controller.DeleteEmployee(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_Returns_NotFound_If_Employee_Not_Found()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<EmployeesController>>();
            var mockEmployeeService = new Mock<IEmployeeService>();
            mockEmployeeService.Setup(service => service.GetEmployeeById(It.IsAny<Guid>()))
                              .ReturnsAsync((Employee)null);
            var controller = new EmployeesController(mockEmployeeService.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteEmployee(Guid.Empty);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_ValidId_ReturnsNoContent()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<EmployeesController>>();
            var mockEmployeeService = new Mock<IEmployeeService>();
            mockEmployeeService.Setup(service => service.GetEmployeeById(It.IsAny<Guid>()))
                              .ReturnsAsync(new Employee { Id = new Guid("05e833b8-cd86-472e-a440-08db999a7231"), Name = "John" });
            var controller = new EmployeesController(mockEmployeeService.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteEmployee(Guid.Empty);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_Returns_InternalServerError_If_Exception_Occurs()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<EmployeesController>>();
            var mockEmployeeService = new Mock<IEmployeeService>();
            mockEmployeeService.Setup(service => service.GetEmployeeById(It.IsAny<Guid>()))
                              .ThrowsAsync(new Exception("Some error"));
            var controller = new EmployeesController(mockEmployeeService.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteEmployee(Guid.Empty);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}