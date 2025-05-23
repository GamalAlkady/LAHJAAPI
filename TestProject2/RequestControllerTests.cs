using APILAHJA.Utilities;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.Extensions.Logging;
using Moq;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;
using V1.Repositories.Share;
using V1.Services.Services;
using WasmAI.ConditionChecker.Base;

public class RequestServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedRequest()
    {
        // Arrange
        var userClaims = new Mock<IUserClaimsHelper>();
        userClaims.Setup(x => x.ServicesIds).Returns(new List<string> { "11111111-1111-1111-1111-111111111111" });
        userClaims.Setup(x => x.SubscriptionId).Returns(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        userClaims.Setup(x => x.SessionId).Returns(Guid.Parse("33333333-3333-3333-3333-333333333333"));
        userClaims.Setup(x => x.UserId).Returns(Guid.Parse("44444444-4444-4444-4444-444444444444"));

        var checker = new Mock<IConditionChecker>();
        var subscriptionFilter = new SubscriptionFilterVM
        {
            Id = Guid.NewGuid(),
            AllowedRequests = 100,
            NumberRequests = 50
        };
        checker.Setup(x => x.CheckAndResultAsync(
            SubscriptionValidatorStates.IsAllowedRequestsForCreate,
            It.IsAny<DataFilter>()))
            .ReturnsAsync(new ConditionResult
            {
                Success = true,
                Result = subscriptionFilter
            });

        var service = new Service
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            AbsolutePath = "ai/endpoint",
            ModelAi = new ModelAi
            {
                AbsolutePath = "ai/model",
                ModelGateway = new ModelGateway
                {
                    Url = "https://model.gateway"
                }
            }
        };

        var serviceService = new Mock<IUseServiceService>();
        serviceService.Setup(x => x.GetOneByAsync(
            It.IsAny<List<FilterCondition>>(),
            It.IsAny<ParamOptions>()))
            .ReturnsAsync(service);

        var mapper = new Mock<IMapper>();
        var repo = new Mock<IRequestShareRepository>();
        var eventRequestService = new Mock<IUseEventRequestService>();
        var loggerFactory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<RequestService>>();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        var requestService = new RequestService(
            mapper.Object,
            loggerFactory.Object,
            repo.Object,
            eventRequestService.Object,
            checker.Object,
            serviceService.Object,
            userClaims.Object
        );

        var entity = new RequestRequestDso
        {
            Id = Guid.NewGuid(),
            ServiceId = service.Id,
            SpaceId = Guid.NewGuid(),
            Events = new List<EventRequestRequestDso>()
        };

        // إعداد الدالة الأساسية CreateAsync التي ترجع استجابة
        repo.Setup(r => r.CreateAsync(It.IsAny<RequestRequestShareDto>()))
            .ReturnsAsync(new RequestResponseShareDto { Id = entity.Id });

        mapper.Setup(m => m.Map<RequestResponseDso>(It.IsAny<RequestResponseShareDto>()))
            .Returns((RequestResponseShareDto dto) => new RequestResponseDso { Id = dto.Id });

        mapper.Setup(m => m.Map<RequestRequestShareDto>(It.IsAny<RequestRequestDso>()))
            .Returns(new RequestRequestShareDto());

        // Act
        var result = await requestService.CreateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal("https://model.gateway", entity.ModelGateway);
        Assert.Equal("ai/model", entity.ModelAi);
        Assert.Single(entity.Events);
        Assert.Equal("Created", entity.Events[0].Status);
    }
}
