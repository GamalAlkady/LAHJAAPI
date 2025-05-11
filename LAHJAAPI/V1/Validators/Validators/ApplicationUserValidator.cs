using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.EntityFrameworkCore;
using Quartz.Util;
using V1.DyModels.Dso.Requests;
using V1.Validators;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum ApplicationUserValidatorStates
    {
        IsActive = 6500,
        IsFull,
        HasCustomerId,
        HasFirstName,
        HasLastName,
        HasDisplayName,
        HasProfileUrl,
        HasImageUrl,
        HasLastLoginIp,
        HasCreatedAt,
        HasUpdatedAt,
        IsNotArchived,
        HasArchivedDate,
        HasLastLoginDate,
        IsHasModelAi,
        IsModelAssigned,
        CanAssignService
    }

    public class ApplicationUserValidator : ValidatorContext<ApplicationUser, ApplicationUserValidatorStates>
    {
        private readonly IConditionChecker _checker;

        public ApplicationUserValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
        }

        protected override void InitializeConditions()
        {
            _provider.Register(
                ApplicationUserValidatorStates.HasLastLoginDate,
                new LambdaCondition<ApplicationUserRequestDso>(
                    nameof(ApplicationUserValidatorStates.HasLastLoginDate),
                    context => context.LastLoginDate.HasValue,
                    "Last Login Date is required"
                )
            );

            _provider.Register(
                ApplicationUserValidatorStates.HasArchivedDate,
                new LambdaCondition<ApplicationUserRequestDso>(
                    nameof(ApplicationUserValidatorStates.HasArchivedDate),
                    context => context.ArchivedDate.HasValue,
                    "Archived Date is required"
                )
            );

            _provider.Register(
                ApplicationUserValidatorStates.HasLastLoginIp,
                new LambdaCondition<ApplicationUserRequestDso>(
                    nameof(ApplicationUserValidatorStates.HasLastLoginIp),
                    context => !string.IsNullOrWhiteSpace(context.LastLoginIp),
                    "Last Login IP is required"
                )
            );

            _provider.Register(
                ApplicationUserValidatorStates.HasImageUrl,
                new LambdaCondition<ApplicationUserRequestDso>(
                    nameof(ApplicationUserValidatorStates.HasImageUrl),
                    context => !string.IsNullOrWhiteSpace(context.Image),
                    "Image URL is required"
                )
            );

            _provider.Register(
                ApplicationUserValidatorStates.IsActive,
                new LambdaCondition<string>(
                    nameof(ApplicationUserValidatorStates.IsActive),
                    context => IsActive(context).Result, // Ensure result is awaited properly
                    "User is archived"
                )
            );

            _provider.Register(
                ApplicationUserValidatorStates.HasCustomerId,
                new LambdaCondition<string>(
                    nameof(ApplicationUserValidatorStates.HasCustomerId),
                    context => !string.IsNullOrWhiteSpace(context),
                    "Customer ID is required"
                )
            );


        }



        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsModelAssigned, "Model AI not belong to user.")]
        async Task<ConditionResult> IsModelAssigned(DataFilter<string> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
            if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

            var result = await _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<UserModelAi>().Include(um => um.ModelAi).FirstOrDefaultAsync(um => um.UserId == data.Id && um.ModelAiId == data.Value)
            );

            return result != null
                ? ConditionResult.ToSuccess(result, "ModelAi is assigned to user.")
                : ConditionResult.ToError($"Model AI with ID: ({data.Value}) not assigned to user.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.CanAssignService)]
        async Task<ConditionResult> CanAssignService(DataFilter<string> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
            if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

            if (await _checker.CheckAndResultAsync(UserServiceValidatorStates.IsServiceAssigned, new DataFilter(data.Id) { Value = data.Value }) is { Success: true } res1)
            {
                return ConditionResult.ToFailure(res1.Result, res1.Message);
            }

            var resultService = await _checker.CheckAndResultAsync(ServiceValidatorStates.IsFound, data.Value);
            if (resultService.Success.GetValueOrDefault())
            {
                var service = (Service)resultService.Result!;
                var res = await IsModelAssigned(new DataFilter<string> { Id = data.Id, Value = service.ModelAiId });
                //if (res.Success.GetValueOrDefault())
                //{
                //    service.ModelAi = ((UserModelAi?)res.Result).ModelAi;
                //    return result.Success.GetValueOrDefault()
                //        ? ConditionResult.ToSuccess(service, "Service Already assigned to user.")
                //        : ConditionResult.ToError($"Service with ID: ({data.Value}) not assigned to user.");
                //}
                return res;
            }
            return ConditionResult.ToError(resultService.Message ?? "Service not found!.");

        }

        private async Task<bool> IsActive(string id)
        {
            var result = await _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.Id == id)
            );

            return !(result?.IsArchived ?? true);
        }
    }
}
