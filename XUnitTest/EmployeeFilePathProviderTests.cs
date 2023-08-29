using Microsoft.Extensions.Configuration;
using webapi;

namespace UnitTest
{
    public class EmployeeFilePathProviderTests
    {
        [Fact]
        public void GetEmployeeFilePath_Should_Return_Correct_FilePath()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config.GetSection("AppSettings:EmployeeFilePath").Value)
                            .Returns("test.json");

            var filePathProvider = new EmployeeFilePathProvider(configurationMock.Object);

            // Act
            var filePath = filePathProvider.GetEmployeeFilePath();

            // Assert
            Assert.Equal("test.json", filePath);
        }
    }
}
