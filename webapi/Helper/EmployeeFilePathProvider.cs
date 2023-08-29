namespace webapi
{
    public class EmployeeFilePathProvider : IEmployeeFilePathProvider
    {
        private readonly IConfiguration _configuration;

        public EmployeeFilePathProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetEmployeeFilePath()
        {
            return _configuration.GetSection("AppSettings:EmployeeFilePath").Value;
        }
    }
}
