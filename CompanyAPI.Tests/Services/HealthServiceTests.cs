using System;
using System.Collections.Generic;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Interface;
using CompanyAPI.Services;
using CompanyAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;
using Xunit;

namespace CompanyAPI.Tests.Services
{
    public class HealthServiceTests
    {
        private readonly IDbHealthCheck _dbHealthCheck;
        private readonly IHealthService _healthService;
        private readonly ILogger<HealthService> _logger;
        private readonly IAppConfig _config;

        public HealthServiceTests()
        {
            _config = Substitute.For<IAppConfig>();
            _dbHealthCheck = Substitute.For<IDbHealthCheck>();
            _logger = NullLogger<HealthService>.Instance;
            _healthService = new HealthService(_config, _logger, _dbHealthCheck);
        }

        #region PerformHealthCheck

        [Fact]
        public async void PerformHealthCheck_ReturnsTrue()
        {
            //Arrange
            bool result;
            bool expected = true;
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(true, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.PerformHealthCheck();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void PerformHealthCheck_ReturnsFalseDueBadConnection()
        {
            //Arrange
            bool result;
            bool expected = false;
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(false, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.PerformHealthCheck();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void PerformHealthCheck_ReturnsFalseDueException()
        {
            //Arrange
            bool result;
            bool expected = false;
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Throws(new Exception());

            //Act
            result = _healthService.PerformHealthCheck();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        #endregion

        #region VerifyDependencies

        [Fact]
        public async void VerifyDependencies_ReturnsOk()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.OK },
            };
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(true, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailable_DueMySql()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.Unavailable },
            };
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(false, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailable_DueImageApi()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.OK },
            };
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(true, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailable_DueBoth()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.Unavailable },
            };
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(false, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailable_DueExceptionMySql()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.Unavailable },
            };
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Throws(new Exception());

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailable_DueExceptionImageApi()
        {
            //Arrange
            Dictionary<string, string> result;
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.OK },
            };
            DbHealthCheckResponse mockedReturn = new DbHealthCheckResponse(true, "");
            _dbHealthCheck.MySqlConnectionTest(Arg.Any<string>()).Returns(mockedReturn);

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(1).MySqlConnectionTest(Arg.Any<string>());
        }

        [Fact]
        public async void VerifyDependencies_ReturnsUnavailableDueExceptionBoth()
        {
            //Arrange
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.Unavailable },
            };
            _dbHealthCheck.WhenForAnyArgs(x => x.ReceivedCalls()).Do(x => { throw new Exception(); });

            //Act
            result = _healthService.VerifyDependencies();

            //Assert
            Assert.Equal(expected, result);
            _dbHealthCheck.Received(0).MySqlConnectionTest(Arg.Any<string>());
        }

        #endregion

        #region CheckDependencies

        [Fact]
        public void CheckDependenciesResult_ReturnsTrue()
        {
            //Arrange
            Dictionary<string, string> verifyResult = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.OK },
            };

            //Act
            var result = _healthService.CheckDependenciesResult(verifyResult);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckDependenciesResult_ReturnsFalse()
        {
            //Arrange
            Dictionary<string, string> verifyResult = new Dictionary<string, string>
            {
                { DependenciesTypes.MySql, HealthResults.Unavailable },
                { HealthResults.OK, HealthResults.OK },
            };

            //Act
            var result = _healthService.CheckDependenciesResult(verifyResult);

            //Assert
            Assert.False(result);
        }

        #endregion
    }
}