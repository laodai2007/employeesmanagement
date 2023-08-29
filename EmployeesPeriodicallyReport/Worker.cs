using OfficeOpenXml;
using System.Text.Json;
using System.Text.Json.Serialization;
using webapi;

namespace EmployeesPeriodicallyReport
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;

        public Worker(ILogger<Worker> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Fetch employee data from the API
                    var employeeData = await FetchEmployeeDataAsync();

                    // Generate and save Excel report
                    GenerateExcelReport(employeeData);

                    _logger.LogInformation("Report generated at: {time}", DateTimeOffset.Now);

                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Run every 10 minutes
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while generating the report.");
                }
            }
        }

        private async Task<List<Employee>> FetchEmployeeDataAsync()
        {
            var apiUrl = "https://localhost:7069"; // Replace with your API URL
            var response = await _httpClient.GetStringAsync($"{apiUrl}/api/employees");

            if (!string.IsNullOrEmpty(response))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                var employees = JsonSerializer.Deserialize<List<Employee>>(response, options);

                if (employees != null)
                {
                    foreach (var employee in employees)
                    {
                        employee.HiringDate = employee.HiringDate.ToUniversalTime().Date;
                    }
                    return employees;
                }
            }

            return new List<Employee>();
        }

        private void GenerateExcelReport(IEnumerable<Employee> employees)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Report");

                // Populate the Excel worksheet with employee data
                int row = 1;
                worksheet.Cells[row, 1].Value = "ID";
                worksheet.Cells[row, 2].Value = "Name";
                worksheet.Cells[row, 3].Value = "Position";
                worksheet.Cells[row, 4].Value = "HiringDate";
                worksheet.Cells[row, 5].Value = "Salary";
                row++;

                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.Id;
                    worksheet.Cells[row, 2].Value = employee.Name;
                    worksheet.Cells[row, 3].Value = employee.Position;
                    worksheet.Cells[row, 4].Value = employee.HiringDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = employee.Salary;

                    row++;
                }

                // Save the Excel file to bin
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmployeeReport.xlsx");
                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
        }
    }
}