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

            // Wrap property names and values with quotation marks while also replacing the "=" sign with ":".
            // Just JsonResult things...
            jsonString = Regex.Replace(jsonString, @"(\w+)\s*=\s*([^,}]+)", @"""$1"": ""$2""");

            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonString);
            Assert.IsTrue((bool)jsonData["success"]);
            Assert.That((string)jsonData["userId"], Is.EqualTo("123456789"));
            Assert.IsNotNull((string)jsonData["password"]);

            mockDbSet.Verify(mock => mock.Add(It.IsAny<ValidPassword>()), Times.Once);
            _mockDbContext.Verify(mock => mock.SaveChanges(), Times.Once);
        }


        [Test]
        public void Reset_WithValidUserId_ShouldRemovePasswordFromDatabaseAndReturnSuccessJson()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "userId", "123456789" }
            });

            var data = new List<ValidPassword>
            {
                new ValidPassword { UserId = "123456789", Password = "passwordrandom" }
            }.AsQueryable();

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
            var result = _passwordController.Reset(form) as JsonResult;
            var jsonString = result.Value.ToString();

            // Wrap property names and values with quotation marks while also replacing the "=" sign with ":".
            // Just JsonResult things...
            jsonString = Regex.Replace(jsonString, @"(\w+)\s*=\s*([^,}]+)", @"""$1"": ""$2""");

            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonString);
            Assert.IsTrue((bool)jsonData["success"]);

            mockDbSet.Verify(mock => mock.Remove(It.IsAny<ValidPassword>()), Times.Once);
            _mockDbContext.Verify(mock => mock.SaveChanges(), Times.Once);
        }

        [Test]
        public void Reset_WithInvalidUserId_ShouldReturnErrorJson()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "userId", "987654321" } // Invalid user ID
            });

            var data = new List<ValidPassword>
            {
                new ValidPassword { UserId = "123456789", Password = "passwordrandom" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<ValidPassword>>();
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockDbContext = new Mock<IAppDbContext>();
            _mockDbContext.Setup(m => m.Passwords).Returns(mockDbSet.Object);

            _passwordController = new PasswordController(_mockDbContext.Object);

            // Act
            var result = _passwordController.Reset(form) as JsonResult;
            var jsonString = result.Value.ToString();

            // Wrap property names and values with quotation marks while also replacing the "=" sign with ":".
            // Just JsonResult things...
            jsonString = Regex.Replace(jsonString, @"(\w+)\s*=\s*([^,}]+)", @"""$1"": ""$2""");

            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonString);
            Assert.IsFalse((bool)jsonData["success"]);
            Assert.That(((string)jsonData["error"]).Trim(), Is.EqualTo("Unexpected Error!"));

            mockDbSet.Verify(mock => mock.Remove(It.IsAny<ValidPassword>()), Times.Never);
            _mockDbContext.Verify(mock => mock.SaveChanges(), Times.Never);
        }

        [Test]
        public void Check_WithExistingPassword_ShouldReturnFoundJson()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "passwordCheck", "passwordrandom" } // Existing password
            });

            var data = new List<ValidPassword>
            {
                new ValidPassword { UserId = "123456789", Password = "passwordrandom" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<ValidPassword>>();
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<ValidPassword>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockDbContext = new Mock<IAppDbContext>();
            _mockDbContext.Setup(m => m.Passwords).Returns(mockDbSet.Object);

            _passwordController = new PasswordController(_mockDbContext.Object);

            // Act
            var result = _passwordController.Check(form) as JsonResult;
            var jsonString = result.Value.ToString();

            // Wrap property names and values with quotation marks while also replacing the "=" sign with ":".
            // Just JsonResult things...
            jsonString = Regex.Replace(jsonString, @"(\w+)\s*=\s*([^,}]+)", @"""$1"": ""$2""");

            var jsonData = JsonConvert.DeserializeObject<JObject>(jsonString);
            Assert.IsTrue((bool)jsonData["success"]);
            Assert.IsTrue((bool)jsonData["found"]);

            _mockDbContext.Verify(mock => mock.Passwords, Times.Once);
        }
    }
}
