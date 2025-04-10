using AutoGenerator.Conditions;
using V1.DyModels.Dso.ResponseFilters;
using V1.Validators;

namespace AutoBuilderApiCore.V1.Validators
{

    public enum SpaceValidatorStates
    {
        IsActive,
        IsFull,
        IsValid,
        IsAllowed

    }


    public class SpaceValidator : BaseValidator<SpaceResponseFilterDso, SpaceValidatorStates>, ITValidator
    {
        public SpaceValidator(IConditionChecker checker) : base(checker)
        {


        }
        protected override void InitializeConditions()
        {
            _provider.Register(
                SpaceValidatorStates.IsActive,
                new LambdaCondition<SpaceResponseFilterDso>(
                    nameof(SpaceValidatorStates.IsActive),

                    context => IsActive(context),
                    "Space is not active"
                )
            );








        }



        private bool IsActive(SpaceResponseFilterDso context)
        {
            if (context.IsGlobal == true &&
                _checker.Check(SubscriptionValidatorStates.IsActive, context.Subscription))
            {
                return true;
            }
            return false;
        }

        private bool IsFull(SpaceResponseFilterDso context)
        {

            if (IsActive(context))
            {
                return false;
            }
            return true;
        }
    }
}