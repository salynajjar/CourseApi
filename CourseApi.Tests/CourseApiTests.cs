using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CourseApi.Enums;
using CourseApi.Models;
using CourseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CourseApi.Tests;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_IncludesRoleClaim()
    {
        var configuration = BuildConfiguration();
        var jwtService = new JwtService(configuration);
        var user = new AppUser
        {
            Id = 1,
            Username = "admin",
            Email = "admin@courseapi.com",
            Role = Role.Admin
        };

        var token = jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        Assert.NotNull(roleClaim);
        Assert.Equal("Admin", roleClaim.Value);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsMyVerySecretKeyForCourseApi123456",
                ["Jwt:Issuer"] = "CourseApi",
                ["Jwt:Audience"] = "CourseApiUsers",
                ["Jwt:DurationInMinutes"] = "60"
            })
            .Build();
    }
}

public class EnumValidationTests
{
    [Theory]
    [InlineData(EnrollmentStatus.NotStarted, true)]
    [InlineData(EnrollmentStatus.Completed, true)]
    [InlineData((EnrollmentStatus)99, false)]
    public void EnrollmentStatus_IsDefined_ReturnsExpectedResult(
        EnrollmentStatus status,
        bool expected)
    {
        var isDefined = Enum.IsDefined(typeof(EnrollmentStatus), status);
        Assert.Equal(expected, isDefined);
    }

    [Theory]
    [InlineData(PassStatus.Pending, true)]
    [InlineData(PassStatus.Passed, true)]
    [InlineData((PassStatus)99, false)]
    public void PassStatus_IsDefined_ReturnsExpectedResult(
        PassStatus status,
        bool expected)
    {
        var isDefined = Enum.IsDefined(typeof(PassStatus), status);
        Assert.Equal(expected, isDefined);
    }
}

public class AuthorizationAttributeTests
{
    [Fact]
    public void DeleteEndpoints_RequireAdminRole()
    {
        var deleteMethods = new[]
        {
            typeof(CourseApi.Controllers.CoursesController).GetMethod(nameof(CourseApi.Controllers.CoursesController.DeleteCourse)),
            typeof(CourseApi.Controllers.StudentsController).GetMethod(nameof(CourseApi.Controllers.StudentsController.DeleteStudent)),
            typeof(CourseApi.Controllers.TeachersController).GetMethod(nameof(CourseApi.Controllers.TeachersController.DeleteTeacher))
        };

        Assert.All(deleteMethods, method =>
        {
            Assert.NotNull(method);
            var authorize = method!
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Contains("Admin", authorize!.Roles);
        });
    }

    [Fact]
    public void MutatingCourseEndpoints_RequireAdminOrTeacherRole()
    {
        var methods = new[]
        {
            typeof(CourseApi.Controllers.CoursesController).GetMethod(nameof(CourseApi.Controllers.CoursesController.CreateCourse)),
            typeof(CourseApi.Controllers.CoursesController).GetMethod(nameof(CourseApi.Controllers.CoursesController.UpdateCourse))
        };

        Assert.All(methods, method =>
        {
            Assert.NotNull(method);
            var authorize = method!
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>()
                .FirstOrDefault();

            Assert.NotNull(authorize);
            Assert.Contains("Admin", authorize!.Roles);
            Assert.Contains("Teacher", authorize.Roles);
        });
    }
}

public class ProtectedEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProtectedEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCourses_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/Courses");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
}
