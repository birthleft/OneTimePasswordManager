using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneTimePasswordManager.Controllers;
using OneTimePasswordManager.Data;
using OneTimePasswordManager.Models;
using System;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Tests
{
    [TestFixture]
    public class PasswordControllerTests
    {
        private PasswordController _passwordController;
        private Mock<IAppDbContext> _mockDbContext;

        [Test]
        public void Submit_WithValidData_ShouldAddPasswordToDatabaseAndReturnSuccessJson()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "userId", "123456789" },
                { "date", "2023-07-03" },
                { "time", "12:00" },
                { "currentDateTime", "on" }
            });

            var data = new List<ValidPassword>().AsQueryable();
            var mockDbSet = new Mock<DbSet<ValidPassword>>();
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockDbContext = new Mock<IAppDbContext>();
            _mockDbContext.Setup(m => m.Passwords).Returns(mockDbSet.Object);
            _mockDbContext.Setup(m => m.SaveChanges()).Returns(1);

            _passwordController = new PasswordController(_mockDbContext.Object);

            // Act
            var result = _passwordController.Submit(form) as JsonResult;
            var jsonString = result.Value.ToString();

            // Wrap property names with quotation marks
            jsonString = Regex.Replace(jsonString, @"(?<![""])({|,)\s*([a-zA-Z0-9_]+)\s*=", "$1\"$2\":");
            // Wrap property values with quotation marks
            jsonString = Regex.Replace(jsonString, @":\s*([^\s,}]+)", ": \"$1\"");

            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonString);
            Assert.IsTrue((bool)jsonData["success"]);
            Assert.That((string)jsonData["userId"], Is.EqualTo("123456789"));
            Assert.IsNotNull((string)jsonData["password"]);

            mockDbSet.Verify(mock => mock.Add(It.IsAny<ValidPassword>()), Times.Once);
            _mockDbContext.Verify(mock => mock.SaveChanges(), Times.Once);
        }
    }
}
