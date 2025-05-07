using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.EntityFrameworkCore;
using Quartz.Util;
using V1.DyModels.Dso.Requests;

namespace LAHJAAPI.V1.Validators
{



    public enum ApplicationUserValidatorStates
    {
        IsActive = 6500,
        IsFull,
        HasCustomerId,
        IsHasService,
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
        CanAssignModel,
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

                       context => isAcive(context),
                       "Space is not active"
                   )
               );

            // First registration for HasCustomerId condition
            _provider.Register(
                ApplicationUserValidatorStates.HasCustomerId,
                new LambdaCondition<string>(
                    nameof(ApplicationUserValidatorStates.HasCustomerId),
                    context => !string.IsNullOrWhiteSpace(context),
                    "Customer ID is required"
                )
            );

            // Next, you have a condition for ApplicationUserRequestDso
            _provider.Register(
                       ApplicationUserValidatorStates.IsFull,
                       new LambdaCondition<ApplicationUserRequestDso>(
                           nameof(ApplicationUserValidatorStates.IsFull),
                           context => IsValidCustomer(context),  //  √ﬂœ √‰ IsValidCustomer  ﬁ»· ‰Ê⁄ ApplicationUserRequestDso
                           "Customer ID is required"
                       )
                   );

            RegisterCondition<string>(
                ApplicationUserValidatorStates.IsHasService,
                IsHasService,
                "Service not belong to user."
            );

            RegisterCondition<string>(
                ApplicationUserValidatorStates.CanAssignService,
                CanAssignService,
                "Service not belong to user."
            );
        }

        //TODO: test bellow two functions



        async Task<ConditionResult> IsHasService(DataFilter<string, ApplicationUser> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");

            var result = await _checker.Injector.Context.Set<UserService>()
                .AnyAsync(x => x.UserId == _injector.UserClaims.UserId && x.ServiceId == data.Id);
            return result
                ? ConditionResult.ToSuccess(null, "Service Already assigned to user.")
                : ConditionResult.ToError("Service not belong to user.");

        }
        async Task<ConditionResult> CanAssignService(DataFilter<string, ApplicationUser> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");

            var resultService = await _checker.CheckAndResultAsync(ServiceValidatorStates.IsFound, data.Id);
            if (resultService.Success == true)
            {
                var service = (Service)resultService.Result!;
                if (await IsHasModelAi(new DataFilter<string, ApplicationUser> { Id = service.ModelAiId }) is { Success: true })
                {
                    if (await IsHasService(data) is { Success: true } res)
                        return ConditionResult.ToError(res.Message!);

                    // assigned service to user
                    var userService = new UserService { UserId = _injector.UserClaims.UserId, ServiceId = service.Id };
                    await _injector.Context.UserServices.AddAsync(userService);
                    await _injector.Context.SaveChangesAsync();
                    return ConditionResult.ToSuccess(userService, "Service assigned successfully.");
                }
                return ConditionResult.ToError($"Service with Id: {service.Id} not belong to UserModelAi");

            }
            return ConditionResult.ToError(resultService.Message ?? "Service not found!.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsHasModelAi, "Model AI not belong to user.")]
        async Task<ConditionResult> IsHasModelAi(DataFilter<string, ApplicationUser> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");

            var result = await _checker.Injector.Context.Set<UserModelAi>()
                .AnyAsync(x => x.UserId == _injector.UserClaims.UserId && x.ModelAiId == data.Id);
            return result
                ? ConditionResult.ToSuccess(null, "ModelAi already assign to user.")
                : ConditionResult.ToError("Model AI not belong to user.");

        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.CanAssignModel, "Model AI not belong to user.")]
        async Task<ConditionResult> CanAssignModel(DataFilter<string, ApplicationUser> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");

            var resultModelAi = await _checker.CheckAndResultAsync(ModelValidatorStates.IsFound, data.Id);
            if (resultModelAi.Success == true)
            {
                if (await IsHasModelAi(data) is { Success: true } res) return ConditionResult.ToError(res.Message!);

                // assigned modelAi to user
                var userModelAi = new UserModelAi { UserId = _injector.UserClaims.UserId, ModelAiId = data.Id };
                await _injector.Context.UserModelAis.AddAsync(userModelAi);
                await _injector.Context.SaveChangesAsync();

                return ConditionResult.ToSuccess(userModelAi, "ModelAi assigned successfully.");

            }
            return ConditionResult.ToError(resultModelAi.Message ?? "ModelAi not found!.");
        }

        bool isAcive(string id)
        {
            var result = _checker.Injector.Context.Set<ApplicationUser>()
                   .FirstOrDefault(x => x.Id == id);

            return result?.IsArchived ?? false;

        }

        bool isCustomerId(string id)
        {
            var result = _checker.Injector.Context.Set<ApplicationUser>()
           .Any(user => user.Id == id);

            return result;

        }


        private bool IsValidCustomer(ApplicationUserRequestDso context)
        {
            var conditions = new List<Func<ApplicationUserRequestDso, bool>>
    {
        c => !string.IsNullOrWhiteSpace(c.CustomerId),
        c => !string.IsNullOrWhiteSpace(c.FirstName),
        c => !string.IsNullOrWhiteSpace(c.LastName),
        c => !string.IsNullOrWhiteSpace(c.DisplayName),
        c => !string.IsNullOrWhiteSpace(c.ProfileUrl),
        c => !string.IsNullOrWhiteSpace(c.Image),
        c => !string.IsNullOrWhiteSpace(c.LastLoginIp),
        c => c.LastLoginDate.HasValue,
        c => c.CreatedAt != default,
        c => c.UpdatedAt != default,
        c => !c.IsArchived || c.ArchivedDate.HasValue,
        };


            return conditions.All(condition => condition(context));
        }






    }
}