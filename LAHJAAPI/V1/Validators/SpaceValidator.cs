using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Responses;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{



    public enum SpaceValidatorStates
    {
        IsFound = 6100,
        IsValid,
        IsValidForSession,
        IsActive,
        IsFull,
        HasName,
        HasRam,
        HasCpu,
        HasDisk,
        HasBandwidth,
        HasToken,
        HasSubscriptionId,
        IsGpuEnabled,
        IsGlobalEnabled,
        IsCountSpces,
        IsAvailable,
    }


    public class SpaceValidator : ValidatorContext<Space, SpaceValidatorStates>
    {

        private readonly IConditionChecker _checker;
        public SpaceValidator(IConditionChecker checker) : base(checker)
        {

            _checker = checker;
        }
        protected override void InitializeConditions()
        {


        }

        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsFound)]
        private async Task<ConditionResult> IsFound(DataFilter<string, Space> data)
        {
            if (data.Share == null) return ConditionResult.ToError("Space is not found.");
            return ConditionResult.ToSuccess(data.Share);
        }



        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.HasSubscriptionId)]
        private async Task<ConditionResult> HasSubscriptionId(DataFilter<string, Space> data)
        {
            if (data.Share == null) return ConditionResult.ToError("Space is not found.");
            if (data.Value == null) return ConditionResult.ToError("SubscriptionId is not found as value.");
            if (data.Value == data.Share.SubscriptionId) return ConditionResult.ToSuccess(data.Share);
            return ConditionResult.ToError("This space is not included in your subscription.");
        }


        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsAvailable, "Spaces are not avaliable")]
        async Task<ConditionResult> IsSpacesAvailableAsync(DataFilter<int, Space> data)
        {
            try
            {
                if (data?.Items?.ContainsKey("subscriptionId") != true)
                    return ConditionResult.ToError("Item must contains subscriptionId");


                if (data?.Items?.ContainsKey("planId") != true)
                    return ConditionResult.ToError("Item must contains planId");

                var countSpaces = await QueryCountAsync<Space>(x => x.SubscriptionId == data.Items["subscriptionId"].ToString());

                if (await _checker.CheckAsync(PlanValidatorStates.HasAllowedSpaces, new DataFilter
                {
                    Id = data.Items["planId"].ToString(),
                    Value = countSpaces
                }))
                {
                    return ConditionResult.ToSuccess(data.Share);
                }
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Coudn't create space",
                    Detail = "You have exhausted all allowed subscription spaces.",
                    Status = SubscriptionValidatorStates.IsAvailableSpaces.ToInt()
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsValid, "Space is not valid")]
        async Task<ConditionResult> IsValidAsync(DataFilter<string, Space> data)
        {
            if (data.Items == null || data.Items.TryGetValue("servicesIds", out object? servicesIds))
                return ConditionResult.ToError("servicesIds is null.");

            //if (_checker.Check(TokenValidatorStates.IsServiceIdsEmpty))
            //{
            //    return ConditionResult.ToFailure(new ProblemDetails
            //    {
            //        Title = "Coudn't create space",
            //        Detail = "You cannot add a space because this session does not belong to service create space.",
            //        Status = TokenValidatorStates.IsServiceIdsEmpty.ToInt()
            //    });
            //}

            if ((await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces)) is { Success: !true } result2)
            {
                return result2;
            }

            //var service = _checker.Injector.Context.Services.FirstOrDefault(s => s.AbsolutePath == GeneralServices.CreateSpace.ToString());
            var serviceId = (servicesIds as List<string>)[0];
            if (serviceId is null || !await _checker.CheckAsync(ServiceValidatorStates.IsFound, serviceId))
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Coudn't create space",
                    Detail = "You coudn't create space with this session. You need to create service CreateSpace.",
                    Status = GeneralServices.CreateSpace.ToInt()
                }, "You coudn't create space with this session. You need to create service CreateSpace.");
            }
            return ConditionResult.ToSuccess(_injector.Mapper.Map<SpaceResponseDso>(data.Share));
        }


        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsValidForSession, "Space is not valid")]
        async Task<ConditionResult> IsValidForSessionAsync(DataFilter<string, Space> data)
        {
            if (string.IsNullOrEmpty(data.Id)) return ConditionResult.ToError("You must encrypt space id with token.");

            if (data.Share == null)
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Coudn't create session for service 'createspace'",
                    Detail = "This space is not included in your subscription.",
                    Status = SpaceValidatorStates.IsValid.ToInt()
                });
            }

            if ((await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces)) is { Success: false } result2)
            {
                return result2;
            }

            return ConditionResult.ToSuccess(_injector.Mapper.Map<SpaceResponseDso>(data.Share));
        }


    }
}