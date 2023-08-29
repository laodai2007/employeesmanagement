using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webapi;

namespace UnitTest
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeService _employeeService;
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly Mock<ICache> _mockMemoryCache;
        private readonly Mock<ILogger<EmployeeRepository>> _mockLogger;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _mockMemoryCache = new Mock<ICache>();
            _mockLogger = new Mock<ILogger<EmployeeRepository>>();
            _employeeService = new EmployeeService(_mockRepository.Object, _mockMemoryCache.Object, _mockLogger.Object);
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
            _mockRepository.Setup(repository => repository.GetEmployees()).ReturnsAsync(expectedEmployees);

            // Act
            var employees = await _employeeService.GetEmployees();

            // Assert
            Assert.Equal(expectedEmployees.Count, employees.Count());
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsEmployee()
        {
            // Arrange
            Guid employeeId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            var expectedEmployee = new Employee { Id = employeeId, Name = "John Doe" };
            _mockRepository.Setup(repository => repository.GetEmployeeById(employeeId)).ReturnsAsync(expectedEmployee);

            // Act
            var employee = await _employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.NotNull(employee);
            Assert.Equal(expectedEmployee.Id, employee.Id);
        }

        [Fact]
        public async Task GetEmployeeById_InvalidId_ReturnsNull()
        {
            // Arrange
            Guid invalidId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            _mockRepository.Setup(repository => repository.GetEmployeeById(invalidId)).ReturnsAsync((Employee)null);

            // Act
            var employee = await _employeeService.GetEmployeeById(invalidId);

            // Assert
            Assert.Null(employee);
        }

        [Fact]
        public async Task AddEmployee_ValidEmployee_SuccessfullyAdded()
        {
            // Arrange
            var newEmployee = new Employee { Name = "Alice Johnson" };
            _mockRepository.Setup(repository => repository.AddEmployee(newEmployee));

            // Act
            await _employeeService.AddEmployee(newEmployee);

            // Assert: Verify that the repository's AddEmployee method was called
            _mockRepository.Verify(repository => repository.AddEmployee(newEmployee), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ValidId_SuccessfullyUpdated()
        {
            // Arrange
            Guid employeeId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            var updatedEmployee = new Employee { Id = employeeId, Name = "Updated Name", HiringDate = DateTime.Now, Position = "Test Position", Salary = 9999 };
            _mockRepository.Setup(repository => repository.UpdateEmployee(employeeId, updatedEmployee));

            // Act
            await _employeeService.UpdateEmployee(employeeId, updatedEmployee);

            // Assert: Verify that the repository's UpdateEmployee method was called
            _mockRepository.Verify(repository => repository.UpdateEmployee(employeeId, updatedEmployee), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ValidId_SuccessfullyDeleted()
        {
            // Arrange
            Guid employeeId = new Guid("05e833b8-cd86-472e-a440-08db999a7231");
            _mockRepository.Setup(repository => repository.DeleteEmployee(employeeId));

            // Act
            await _employeeService.DeleteEmployee(employeeId);

            // Assert: Verify that the repository's DeleteEmployee method was called
            _mockRepository.Verify(repository => repository.DeleteEmployee(employeeId), Times.Once);
        }
    }
}
