using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

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
            _provider.Register(
                SpaceValidatorStates.IsActive,
                new LambdaCondition<SpaceRequestDso>(
                    nameof(SpaceValidatorStates.IsActive),

                    context => IsActive(context),
                    "Space is not active"
                )
            );


            //_provider.Register(
            //    SpaceValidatorStates.IsValid,
            //    new LambdaCondition<SpaceFilterVM>(
            //        nameof(SpaceValidatorStates.IsValid),
            //        context => IsValid(context),
            //        "Space is not valid"
            //    )
            //);


            _provider.Register(SpaceValidatorStates.IsCountSpces,
                new LambdaCondition<string>(
                    nameof(SpaceValidatorStates.IsCountSpces),
                    context => IsCountSpces(context),
                    "Space is not count"
                )
            );




            _provider.Register(SpaceValidatorStates.HasToken,
                new LambdaCondition<SpaceRequestDso>(
                    nameof(SpaceValidatorStates.HasToken),
                    context => Istoken(context),
                    "Space is not global"
                )
            );


            //RegisterCondition<string>(
            //    SpaceValidatorStates.IsFound,
            //    IsFound, "Space is not found");

            //_provider.Register(SpaceValidatorStates.IsFound,
            //    new LambdaCondition<SpaceFilterVM>(
            //        nameof(SpaceValidatorStates.IsFound),
            //        context => IsFound(context.Id),
            //        "Space is not found"
            //    )
            //);

            //_provider.Register(SpaceValidatorStates.IsAvailable,
            //    new LambdaCondition<SpaceFilterVM>(
            //        nameof(SpaceValidatorStates.IsAvailable),
            //        context => IsSpacesAvailable(context),
            //        "Spaces not available"
            //    )
            //);


            //_provider.Register(SpaceValidatorStates.IsAvailableForCreate,
            //    new LambdaCondition<SpaceFilterVM>(
            //        nameof(SpaceValidatorStates.IsAvailableForCreate),
            //        context => IsAvailableForCreate(context),
            //        "Spaces not available"
            //    )
            //);
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
            if (data.Value == null) return ConditionResult.ToError("SubscriptionId is not found.");
            if (data.Value == data.Share.SubscriptionId) return ConditionResult.ToSuccess(data.Share);
            return ConditionResult.ToError("This space is not included in your subscription.");
        }


        private bool IsCountSpces(string subId)
        {
            var spaces = _checker.Injector.Context.Set<Space>()
                .Where(x => x.SubscriptionId == subId)
                .ToList();


            var count = spaces.Count;
            // return _checker.Check(SubscriptionValidatorStates.IsIsAllowedSpaces,count, subId);

            return count >= 3;



        }

        private bool Istoken(SpaceRequestDso context)
        {
            if (context.IsGlobal == true)
            {
                return true;
            }
            return false;
        }


        private bool IsActive(SpaceRequestDso context)
        {
            var result = _checker.Check(SubscriptionValidatorStates.IsActive, context.SubscriptionId);

            if (context.IsGlobal == true && result)

            {
                return true;
            }
            return false;
        }


        private bool IsFull(SpaceRequestDso context)
        {

            var conditions = new List<Func<SpaceRequestDso, bool>>
    {
        c => c.IsGlobal == true,
        c => _checker.Check(SubscriptionValidatorStates.IsActive, c.SubscriptionId) ,  // ���� �� �� �������� ���
        c => !string.IsNullOrWhiteSpace(c.Name),
        c => c.Ram.HasValue && c.Ram > 0,
        c => c.CpuCores.HasValue && c.CpuCores > 0,
        c => c.DiskSpace.HasValue && c.DiskSpace > 0,
        c => c.Bandwidth.HasValue && c.Bandwidth > 0,
        c => !string.IsNullOrWhiteSpace(c.Token),
        c => _checker.Check(SubscriptionValidatorStates.IsSubscriptionId, c.SubscriptionId),
        c => c.IsGpu == true,
        c =>  !IsCountSpces(c.SubscriptionId)
        };

            return conditions.All(condition => condition(context));
        }



        private (bool isFull, List<string> failedConditions) IsFullerror(SpaceRequestDso context)
        {
            var failedConditions = new List<string>();

            var conditions = new List<(Func<SpaceRequestDso, bool> condition, string errorMessage)>
    {
        (c => c.IsGlobal == true, "Space must be global"),
       // (c => _checker.Check(SubscriptionValidatorStates.IsActive, c.Subscription), "Subscription must be active"),
        (c => !string.IsNullOrWhiteSpace(c.Name), "Name is required"),
        (c => c.Ram.HasValue && c.Ram > 0, "RAM must be greater than 0"),
        (c => c.CpuCores.HasValue && c.CpuCores > 0, "CPU cores must be greater than 0"),
        (c => c.DiskSpace.HasValue && c.DiskSpace > 0, "Disk space must be greater than 0"),
        (c => c.Bandwidth.HasValue && c.Bandwidth > 0, "Bandwidth must be greater than 0"),
        (c => !string.IsNullOrWhiteSpace(c.Token), "Token is required"),
        (c => !string.IsNullOrWhiteSpace(c.SubscriptionId), "Subscription ID is required"),
        (c => c.IsGpu == true, "GPU must be enabled")
    };



            foreach (var (condition, errorMessage) in conditions)
            {
                if (!condition(context))
                {
                    failedConditions.Add(errorMessage);
                }
            }

            return (failedConditions.Count == 0, failedConditions);
        }



        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsAvailable, "Spaces are not avaliable")]
        async Task<ConditionResult> IsSpacesAvailableAsync(DataFilter<int, Space> data)
        {
            if (data?.Items?.ContainsKey("subscriptionId") != true)
                return ConditionResult.ToError("Item must contains subscriptionId");


            if (data?.Items?.ContainsKey("planId") != true)
                return ConditionResult.ToError("Item must contains planId");

            var countSpaces = await _injector.Context.Spaces.CountAsync(x => x.SubscriptionId == data.Items["subscriptionId"].ToString());

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

        [RegisterConditionValidator(typeof(SpaceValidatorStates), SpaceValidatorStates.IsValid, "Space is not valid")]
        async Task<ConditionResult> IsValidAsync(DataFilter<string, Space> data)
        {
            if (_checker.Check(TokenValidatorStates.IsServiceIdsEmpty))
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Coudn't create space",
                    Detail = "You cannot add a space because this session does not belong to service create space.",
                    Status = TokenValidatorStates.IsServiceIdsEmpty.ToInt()
                });
            }

            if ((await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces)) is { Success: !true } result2)
            {
                return result2;
            }

            //var service = _checker.Injector.Context.Services.FirstOrDefault(s => s.AbsolutePath == GeneralServices.CreateSpace.ToString());
            var serviceId = _checker.Injector.UserClaims.ServicesIds?.FirstOrDefault();
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

            if ((await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces)) is { Success: !true } result2)
            {
                return result2;
            }

            return ConditionResult.ToSuccess(_injector.Mapper.Map<SpaceResponseDso>(data.Share));
        }


    }
}