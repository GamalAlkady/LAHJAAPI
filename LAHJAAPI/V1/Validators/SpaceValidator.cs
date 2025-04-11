using AutoGenerator.Conditions;

using AutoMapper;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.ResponseFilters;

namespace LAHJAAPI.V1.Validators
{



    public enum SpaceValidatorStates
    {
        IsActive,
        IsSpaceId,
        IsFull,
        IsValid,
        HasName,
        HasRam,
        HasCpu,
        HasDisk,
        HasBandwidth,
        HasToken,
        HasSubscriptionId,
        IsGpuEnabled,
        IsGlobalEnabled,
        IsCountSpces


    }


    public class SpaceValidator : BaseValidator<SpaceResponseFilterDso, SpaceValidatorStates>, ITValidator
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


            _provider.Register(
                SpaceValidatorStates.IsFull,
                new LambdaCondition<SpaceRequestDso>(
                    nameof(SpaceValidatorStates.IsFull),
                    context => IsFull(context),
                    "Space is full"
                )
            );


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



            _provider.Register(SpaceValidatorStates.IsSpaceId,
                new LambdaCondition<string>(
                    nameof(SpaceValidatorStates.IsSpaceId),
                    context => IsValidSpaceId(context),
                    "Space is not valid"
                )
            );




        }


        private bool IsValidSpaceId(string spaceId)
        {
            if (spaceId != "")
            {
                var result = _checker.Injector.Context.Set<Space>()
                    .Any(x => x.Id == spaceId);

                return result;
            }
            else
            {
                return false;

            }



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
        c => _checker.Check(SubscriptionValidatorStates.IsActive, c.SubscriptionId) ,  // ÊÍÞÞ ãä Ãä ÇáÇÔÊÑÇß äÔØ
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


        //private bool IsFull(SpaceResponseFilterDso context)
        //{

        //        return false;
        //    }
        //    return true;
        //}
    }
}