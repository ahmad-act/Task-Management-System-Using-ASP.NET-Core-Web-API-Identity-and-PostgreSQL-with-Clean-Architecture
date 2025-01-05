using AutoMapper;
using TaskManagement.Application.DTOs.EntityPrototype;
using TaskManagement.Application.ServiceInterfaces;
using TaskManagement.Application.Services;
using TaskManagement.Application.Utilities.Pagination;
using TaskManagement.Domain.Common.HATEOAS;
using TaskManagement.Domain.Common.JWT;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using TaskManagement.Domain.Common;
using TaskManagement.Application.Utilities.HATEOAS;

namespace TaskManagement.Test
{
    public class EntityPrototypeServiceTest
    {
        //// Mocks for dependencies
        //private readonly Mock<IActivityLog> _activityLogMock;
        //private readonly Mock<IEntityPrototypeRepository> _entityPrototypeRepositoryMock;
        //private readonly Mock<IMapper> _mapperMock;
        //private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        //private readonly IEntityLinkGenerator _entityLinkGenerator;
        //private readonly IEntityPrototypeService _entityPrototypeService;
        //private readonly Mock<IUserService> _userServiceMock;

        //public EntityPrototypeServiceTest()
        //{
        //    // Initialize mocks
        //    _activityLogMock = new Mock<IActivityLog>();
        //    _entityPrototypeRepositoryMock = new Mock<IEntityPrototypeRepository>();
        //    _mapperMock = new Mock<IMapper>();
        //    _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        //    _userServiceMock = new Mock<IUserService>();

        //    // Create an instance of EntityLinkGenerator with mocked HttpContextAccessor
        //    _entityLinkGenerator = new EntityLinkGenerator(_httpContextAccessorMock.Object);

        //    // Setup JWT settings for testing
        //    var jwtSettings = new JwtSettings
        //    {
        //        SecretKey = "abcdefghijklmnopqrstuvwxyzzxcvbnm", // Exactly 32 characters = 256 bits
        //        Issuer = "test_issuer",
        //        Audience = "test_audience",
        //        TokenExpirationInMinutes = 60
        //    };

        //    var jwtHelper = new JwtHelper();

        //    // Create an instance of EntityPrototypeService with mocked dependencies
        //    _entityPrototypeService = new EntityPrototypeService(
        //        _activityLogMock.Object,
        //        _entityPrototypeRepositoryMock.Object,
        //        _mapperMock.Object,
        //        jwtSettings,
        //        jwtHelper,
        //        _entityLinkGenerator,
        //        _userServiceMock.Object
        //    );
        //}

        //#region GetEntityPrototypes Tests

        //[Theory]
        //[InlineData(true, "searchTerm", 1, 10, "EntityPrototypename", "asc", true)]  // EntityPrototype has access, entityPrototypes exist
        //[InlineData(true, null, 1, 10, null, null, false)]               // EntityPrototype has access, no entityPrototypes exist
        //[InlineData(false, "searchTerm", 1, 10, "EntityPrototypename", "asc", false)] // EntityPrototype does not have access
        //public async Task GetEntityPrototypes_ShouldReturnPaginatedEntityPrototypesWithHateoasLinksOrNull(bool hasAccess, string searchTerm, int? page, int? pageSize, string sortColumn, string sortOrder, bool entityPrototypesExist)
        //{
        //    // Arrange
        //    // Mock service's GetUser method
        //    var jwtData = new JwtData { id = "01JBTRXTK6ZH3609W97FS0GEDV", role = "1" };
        //    _userServiceMock.Setup(repo => repo.HasAccess(It.IsAny<JwtData>(), It.IsAny<int>())).ReturnsAsync(hasAccess);

        //    // Set up paginated entityPrototype data and map to EntityPrototypeReadDto if entityPrototypes should exist
        //    IPaginatedList<EntityPrototype> entityPrototypes = entityPrototypesExist ? new PaginatedList<EntityPrototype>(
        //        new List<EntityPrototype> {
        //            new EntityPrototype { Id = Guid.NewUlid(), FieldName = "TestEntityPrototype1", RoleSerial = 10 },
        //            new EntityPrototype { Id = Guid.NewUlid(), FieldDescription = "TestEntityPrototype2", RoleSerial = 10 }
        //        },
        //        page ?? 1, pageSize ?? 10, 20) : null;  // assuming total count of 20 for this example

        //    _entityPrototypeRepositoryMock.Setup(repo => repo.List(searchTerm, page, pageSize, sortColumn, sortOrder)).ReturnsAsync(entityPrototypes);

        //    var entityPrototypeDtos = entityPrototypesExist ? entityPrototypes.Items.Select(entityPrototype => new EntityPrototypeReadDto
        //    {
        //        Id = entityPrototype.Id,
        //        FieldName = entityPrototype.FieldName,
        //    }).ToList() : null;

        //    _mapperMock.Setup(m => m.Map<EntityPrototypeReadDto>(It.IsAny<EntityPrototype>())).Returns((EntityPrototype entityPrototype) => entityPrototypeDtos?.FirstOrDefault(dto => dto.Id == entityPrototype.Id));

        //    // Set up HttpContextAccessor for HATEOAS links
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.Request.Scheme = "http";
        //    httpContext.Request.Host = new HostString("localhost:5000");
        //    httpContext.Request.Path = "/api/entityPrototypes";
        //    _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        //    // Act
        //    var result = await _entityPrototypeService.List(jwtData, searchTerm, page, pageSize, sortColumn, sortOrder);

        //    // Assert
        //    if (!hasAccess)
        //    {
        //        Assert.Null(result); // Ensure result is null if the entityPrototype does not have access
        //    }
        //    else if (!entityPrototypesExist)
        //    {
        //        Assert.Null(result); // Ensure result is null if no entityPrototypes are found
        //    }
        //    else
        //    {
        //        // Ensure result is not null and contains paginated list of EntityPrototypeReadDto with HATEOAS links
        //        Assert.NotNull(result);
        //        Assert.Equal(entityPrototypes.Page, result.Page);
        //        Assert.Equal(entityPrototypes.PageSize, result.PageSize);
        //        Assert.Equal(entityPrototypes.TotalCount, result.TotalCount);

        //        // Validate HATEOAS links for each entityPrototype
        //        foreach (var entityPrototypeDto in result.Items)
        //        {
        //            Assert.Collection(entityPrototypeDto.Links,
        //                link => Assert.Equal(LinkMethod.GET.ToString(), link.Method),
        //                link => Assert.Equal(LinkMethod.POST.ToString(), link.Method),
        //                link => Assert.Equal(LinkMethod.PUT.ToString(), link.Method),
        //                link => Assert.Equal(LinkMethod.DELETE.ToString(), link.Method)
        //            );
        //        }

        //        // Validate pagination HATEOAS links if applicable
        //        if (entityPrototypes.Page > 1)
        //        {
        //            Assert.Contains(result.Links, link => link.Operation == LinkOperation.First.ToString());
        //            Assert.Contains(result.Links, link => link.Operation == LinkOperation.Previous.ToString());
        //        }

        //        if (entityPrototypes.Page < entityPrototypes.TotalPages)
        //        {
        //            Assert.Contains(result.Links, link => link.Operation == LinkOperation.Next.ToString());
        //            Assert.Contains(result.Links, link => link.Operation == LinkOperation.Last.ToString());
        //        }
        //    }
        //}

        //#endregion

        //#region GetEntityPrototype Tests

        //[Theory]
        //[InlineData("01JBTRXTK6ZH3609W97FS0GEDV", true)] // Test with existing entityPrototype ID
        //[InlineData("non-existing-id", false)]      // Test with non-existing entityPrototype ID
        //public async Task GetEntityPrototype_ShouldReturnCorrectEntityPrototypeDtoWithHateoasLinksOrNull(string entityPrototypeId, bool entityPrototypeExists)
        //{
        //    // Arrange
        //    // Mock service's GetUser method
        //    var jwtData = new JwtData { id = "01JBTRXTK6ZH3609W97FS0GEDV", role = "1" };
        //    _userServiceMock.Setup(repo => repo.HasAccess(It.IsAny<JwtData>(), It.IsAny<int>())).ReturnsAsync(true);

        //    var entityPrototype = entityPrototypeExists ? new EntityPrototype { Id = Guid.Parse(entityPrototypeId), FieldName = "TestEntityPrototype" } : null;
        //    var entityPrototypeReadDto = entityPrototypeExists ? new EntityPrototypeReadDto { Id = Guid.Parse(entityPrototypeId), FieldName = "TestEntityPrototype" } : null;

        //    // Mock repository and mapper
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Get(entityPrototypeId)).ReturnsAsync(entityPrototype);
        //    _mapperMock.Setup(m => m.Map<EntityPrototypeReadDto>(It.IsAny<EntityPrototype>())).Returns(entityPrototypeReadDto);

        //    // Set HttpContextAccessor to provide context for HATEOAS links
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.Request.Scheme = "http";
        //    httpContext.Request.Host = new HostString("localhost:5000");
        //    httpContext.Request.Path = "/api/entityPrototypes";
        //    _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        //    // Act
        //    var result = await _entityPrototypeService.Get(jwtData, entityPrototypeId);

        //    // Assert
        //    if (entityPrototypeExists)
        //    {
        //        Assert.NotNull(result); // Ensure result is not null if the entityPrototype exists
        //        Assert.Equal(entityPrototypeId, result.Id.ToString());
        //        // Check that the expected HATEOAS links are present
        //        Assert.Collection(result.Links,
        //            link => Assert.Equal(LinkMethod.GET.ToString(), link.Method),
        //            link => Assert.Equal(LinkMethod.POST.ToString(), link.Method),
        //            link => Assert.Equal(LinkMethod.PUT.ToString(), link.Method),
        //            link => Assert.Equal(LinkMethod.DELETE.ToString(), link.Method)
        //        );
        //    }
        //    else
        //    {
        //        Assert.Null(result); // Ensure result is null if the entityPrototype does not exist
        //    }
        //}

        //#endregion

        //#region CreateEntityPrototype Tests

        //[Theory]
        //[InlineData("newEntityPrototype", "password", false, true)] // New entityPrototype should be created successfully
        //[InlineData("existingEntityPrototype", "password", true, false)] // Existing entityPrototype should not be created
        //public async Task CreateEntityPrototype_ShouldReturnUlidOrNull(string fieldName, string fieldDescription, bool entityPrototypeExists, bool expectedResult)
        //{
        //    // Arrange
        //    // Mock service's GetUser method
        //    var jwtData = new JwtData { id = "01JBTRXTK6ZH3609W97FS0GEDV", role = "1" };
        //    _userServiceMock.Setup(repo => repo.HasAccess(It.IsAny<JwtData>(), It.IsAny<int>())).ReturnsAsync(true);

        //    var entityPrototypeCreateDto = new EntityPrototypeCreateDto { FieldName = fieldName, FieldDescription = fieldDescription };

        //    // Create a mock entityPrototype object
        //    var entityPrototype = new EntityPrototype { Id = Guid.Parse("01JBTRXTK6ZH3609W97FS0GEDV"), FieldName = fieldName, FieldDescription = fieldDescription };

        //    // Setup the EntityPrototypeExists mock
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.EntityPrototypeExists(fieldName)).ReturnsAsync(entityPrototypeExists);

        //    // Automapper setup
        //    _mapperMock.Setup(m => m.Map<EntityPrototype>(It.IsAny<EntityPrototypeCreateDto>())).Returns(entityPrototype);

        //    // Mock the entityPrototype repository's CreateEntityPrototype method
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Create(entityPrototype)).ReturnsAsync(expectedResult ? 1 : 0);

        //    // Act
        //    var result = await _entityPrototypeService.Create(jwtData, entityPrototypeCreateDto);

        //    // Assert
        //    if (expectedResult)
        //    {
        //        Assert.NotNull(result); // Valid result expected when entityPrototype is created
        //    }
        //    else
        //    {
        //        Assert.Equal("00000000000000000000000000", result.ToString()); // Result is "0000000000" when entity already exists 
        //    }
        //}

        //#endregion

        //#region UpdateEntityPrototype Tests

        //[Theory]
        //[InlineData("01JBTRXTK6ZH3609W97FS0GEDV", "updatedEntityPrototype", "newPassword", "1")] // Existing entityPrototype update
        //[InlineData("non-existing-id", "updatedEntityPrototype", "newPassword", "404")] // Non-existing entityPrototype update
        //public async Task UpdateEntityPrototype_ShouldReturnExpectedResult(string Id, string fieldName, string fieldDescription, string expected)
        //{
        //    // Arrange
        //    // Mock service's GetUser method
        //    var jwtData = new JwtData { id = "01JBTRXTK6ZH3609W97FS0GEDV", role = "1" };
        //    _userServiceMock.Setup(repo => repo.HasAccess(It.IsAny<JwtData>(), It.IsAny<int>())).ReturnsAsync(true);

        //    EntityPrototype? existingEntityPrototype = expected == "404" ? null : new EntityPrototype { Id = Guid.Parse(Id), FieldName = fieldName };

        //    // Mock repository's GetEntityPrototype method
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Get(Id)).ReturnsAsync(existingEntityPrototype);

        //    // Mocking the UpdateEntityPrototype method
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Update(It.IsAny<EntityPrototype>()))
        //        .ReturnsAsync(existingEntityPrototype != null ? 1 : 0); // return 1 if entityPrototype exists, 0 if not

        //    // Create DTO for entityPrototype update
        //    var updateEntityPrototypeDto = new EntityPrototypeUpdateDto
        //    {
        //        FieldName = fieldName,
        //        FieldDescription = fieldDescription,
        //        RoleSerial = 1
        //    };

        //    // Act
        //    var result = await _entityPrototypeService.Update(jwtData, Id, updateEntityPrototypeDto);

        //    // Assert
        //    Assert.Equal(expected, result); // Assert that the result matches the expected value
        //}

        //#endregion

        //#region DeleteEntityPrototype Tests

        //[Theory]
        //[InlineData("01JBTRXTK6ZH3609W97FS0GEDV", "1")] // Successful deletion
        //[InlineData("non-existing-id", null)] // Unsuccessful deletion
        //public async Task DeleteEntityPrototype_ShouldReturnExpectedResult(string entityPrototypeId, string? expectedResult)
        //{
        //    // Arrange
        //    // Mock service's GetUser method
        //    var jwtData = new JwtData { id = "01JBTRXTK6ZH3609W97FS0GEDV", role = "1" };
        //    _userServiceMock.Setup(repo => repo.HasAccess(It.IsAny<JwtData>(), It.IsAny<int>())).ReturnsAsync(true);

        //    var entityPrototype = expectedResult == null ? null : new EntityPrototype { Id = Guid.NewUlid(), FieldName = "TestEntityPrototype" };
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Get(entityPrototypeId)).ReturnsAsync(entityPrototype);
        //    _entityPrototypeRepositoryMock.Setup(repo => repo.Delete(It.IsAny<EntityPrototype>())).ReturnsAsync(1);

        //    // Act
        //    var result = await _entityPrototypeService.Delete(jwtData, entityPrototypeId);

        //    // Assert
        //    Assert.Equal(expectedResult, result); // Check if the result matches the expected deletion outcome
        //}

        //#endregion
    }
}
