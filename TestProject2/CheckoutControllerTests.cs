using AutoMapper;
using FluentAssertions;
using LAHJAAPI.V1.Controllers.Api;
using LAHJAAPI.V1.DyModels.VM.Stripe.Checkout;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StripeGateway;
using V1.DyModels.Dso.Responses;
using V1.Services.Services;

namespace TestProject2
{
    public class CheckoutControllerTests
    {
        private readonly Mock<IUseApplicationUserService> _userServiceMock = new();
        private readonly Mock<IStripeSubscription> _stripeSubscriptionMock = new();
        private readonly Mock<IStripeCheckout> _stripeCheckoutMock = new();
        private readonly Mock<IStripeCustomer> _stripeCustomerMock = new();
        private readonly Mock<IUsePlanService> _planServiceMock = new();
        private readonly Mock<ILogger<CheckoutController>> _loggerMock = new();
        private readonly Mock<IConditionChecker> _checkerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private CheckoutController CreateController()
        {
            return new CheckoutController(
                _userServiceMock.Object,
                _stripeSubscriptionMock.Object,
                _stripeCheckoutMock.Object,
                _stripeCustomerMock.Object,
                _planServiceMock.Object,
                _loggerMock.Object,
                _checkerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateCheckout_ReturnsConflict_WhenUserAlreadySubscribed()
        {
            // Arrange
            var controller = CreateController();
            var checkoutOptions = new CheckoutOptions { PlanId = "plan_123" };

            _userServiceMock.Setup(x => x.GetUserWithSubscription())
                .ReturnsAsync(new ApplicationUserResponseDso());

            _checkerMock.Setup(x => x.Check(SubscriptionValidatorStates.IsSubscribe, It.IsAny<object>()))
                .Returns(true);

            // Act
            var result = await controller.CreateCheckout(checkoutOptions);

            // Assert
            result.Result.Should().BeOfType<ConflictObjectResult>();
            var conflictResult = result.Result as ConflictObjectResult;
            conflictResult!.StatusCode.Should().Be(409);
        }


        [Fact]
        public async Task CreateCheckout_ReturnsNotFound_WhenPlanDoesNotExist()
        {
            // Arrange
            var controller = CreateController();
            var checkoutOptions = new CheckoutOptions { PlanId = "nonexistent_plan" };

            _userServiceMock.Setup(x => x.GetUserWithSubscription())
                .ReturnsAsync(new ApplicationUserResponseDso());

            _checkerMock.Setup(x => x.Check(SubscriptionValidatorStates.IsSubscribe, It.IsAny<object>()))
                .Returns(false);

            _planServiceMock.Setup(x => x.GetByIdAsync("nonexistent_plan"))
                .ReturnsAsync((PlanResponseDso?)null);

            // Act
            var result = await controller.CreateCheckout(checkoutOptions);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}