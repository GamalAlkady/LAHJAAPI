using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;


namespace V1.Validators
{
    public enum RequestValidatorStates
    {
        HasId,
        HasStatus,
        HasQuestion,
        HasAnswer,
        HasModelGateway,
        HasModelAi,
        HasCreatedAt,
        HasUpdatedAt,
        HasUserId,
        HasUser,
        HasSubscriptionId,
        HasSubscription,
        HasServiceId,
        HasService,
        HasSpaceId,
        HasSpace,
        HasEvents,
        IsValid,
        IsFull
    }

    public class RequestValidatorContext : ValidatorContext<Request, RequestValidatorStates>, ITValidator
    {
        private Request? _request;

        public RequestValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasId, "Request ID is required.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, Request> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Request ID is required.");
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasStatus, "Status is required.")]
        private Task<ConditionResult> ValidateStatus(DataFilter<string, Request> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Status);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.Status) : ConditionResult.ToFailureAsync("Status is required.");
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasQuestion, "Question is required.")]
        private Task<ConditionResult> ValidateQuestion(DataFilter<string, Request> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Question);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.Question) : ConditionResult.ToFailureAsync("Question is required.");
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasAnswer, "Answer cannot be empty if provided.")]
        private Task<ConditionResult> ValidateAnswer(DataFilter<string?, Request> f)
        {
            var answer = f.Share?.Answer;
            bool valid = answer == null || !string.IsNullOrWhiteSpace(answer);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(answer) : ConditionResult.ToFailure("Answer cannot be empty if provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasModelGateway, "Model Gateway URL format is invalid.")]
        private Task<ConditionResult> ValidateModelGateway(DataFilter<string?, Request> f)
        {
            var url = f.Share?.ModelGateway;
            bool valid = url == null || Uri.TryCreate(url, UriKind.Absolute, out _);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(url) : ConditionResult.ToFailure("Model Gateway URL format is invalid."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasModelAi, "Model AI ID cannot be empty if provided.")]
        private Task<ConditionResult> ValidateModelAi(DataFilter<string?, Request> f)
        {
            var modelAiId = f.Share?.ModelAi;
            bool valid = modelAiId == null || !string.IsNullOrWhiteSpace(modelAiId);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(modelAiId) : ConditionResult.ToFailure("Model AI ID cannot be empty if provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasCreatedAt, "Created At Date is required.")]
        private Task<ConditionResult> ValidateCreatedAt(DataFilter<DateTime, Request> f)
        {
            bool valid = f.Share?.CreatedAt != default(DateTime);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.CreatedAt) : ConditionResult.ToFailure("Created At Date is required."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasUpdatedAt, "Updated At Date is required.")]
        private Task<ConditionResult> ValidateUpdatedAt(DataFilter<DateTime, Request> f)
        {
            bool valid = f.Share?.UpdatedAt != default(DateTime);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.UpdatedAt) : ConditionResult.ToFailure("Updated At Date is required."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasUserId, "User ID is required.")]
        private Task<ConditionResult> ValidateUserId(DataFilter<string, Request> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.UserId);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.UserId) : ConditionResult.ToFailureAsync("User ID is required.");
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasUser, "User object is required.")]
        private Task<ConditionResult> ValidateUser(DataFilter<ApplicationUser?, Request> f)
        {
            bool valid = f.Share?.User != null;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.User) : ConditionResult.ToFailure("User object is required."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasSubscriptionId, "Subscription ID cannot be empty if provided.")]
        private Task<ConditionResult> ValidateSubscriptionId(DataFilter<string?, Request> f)
        {
            var subscriptionId = f.Share?.SubscriptionId;
            bool valid = subscriptionId == null || !string.IsNullOrWhiteSpace(subscriptionId);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(subscriptionId) : ConditionResult.ToFailure("Subscription ID cannot be empty if provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasSubscription, "Subscription object cannot be null if Subscription ID is provided.")]
        private Task<ConditionResult> ValidateSubscription(DataFilter<Subscription?, Request> f)
        {
            bool valid = f.Share?.SubscriptionId == null || f.Share?.Subscription != null;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Subscription) : ConditionResult.ToFailure("Subscription object cannot be null if Subscription ID is provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasServiceId, "Service ID cannot be empty if provided.")]
        private Task<ConditionResult> ValidateServiceId(DataFilter<string?, Request> f)
        {
            var serviceId = f.Share?.ServiceId;
            bool valid = serviceId == null || !string.IsNullOrWhiteSpace(serviceId);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(serviceId) : ConditionResult.ToFailure("Service ID cannot be empty if provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasService, "Service object cannot be null if Service ID is provided.")]
        private Task<ConditionResult> ValidateService(DataFilter<Service?, Request> f)
        {
            bool valid = f.Share?.ServiceId == null || f.Share?.Service != null;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Service) : ConditionResult.ToFailure("Service object cannot be null if Service ID is provided."));
        }


        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasSpaceId, "Space ID cannot be empty if provided.")]
        private Task<ConditionResult> ValidateSpaceId(DataFilter<string?, Request> f)
        {
            var spaceId = f.Share?.SpaceId;
            bool valid = spaceId == null || !string.IsNullOrWhiteSpace(spaceId);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(spaceId) : ConditionResult.ToFailure("Space ID cannot be empty if provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasSpace, "Space object cannot be null if Space ID is provided.")]
        private Task<ConditionResult> ValidateSpace(DataFilter<Space?, Request> f)
        {
            bool valid = f.Share?.SpaceId == null || f.Share?.Space != null;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Space) : ConditionResult.ToFailure("Space object cannot be null if Space ID is provided."));
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.HasEvents, "Events collection cannot be empty.")]
        private Task<ConditionResult> ValidateEvents(DataFilter<ICollection<EventRequest>, Request> f)
        {
            bool valid = f.Share?.Events != null; // Allow empty collection, just check if not null
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Events) : ConditionResult.ToFailure("Events collection cannot be null."));
        }


        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.IsValid, "Request data is invalid.")]
        private async Task<ConditionResult> CheckIsValid(DataFilter<object, Request> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "Request is null.");

            var idValid = await ValidateId(new DataFilter<string, Request> { Share = f.Share });
            if (idValid.Success == false) return idValid;

            var statusValid = await ValidateStatus(new DataFilter<string, Request> { Share = f.Share });
            if (statusValid.Success == false) return statusValid;

            var questionValid = await ValidateQuestion(new DataFilter<string, Request> { Share = f.Share });
            if (questionValid.Success == false) return questionValid;

            var createdAtValid = await ValidateCreatedAt(new DataFilter<DateTime, Request> { Share = f.Share });
            if (createdAtValid.Success == false) return createdAtValid;

            var updatedAtValid = await ValidateUpdatedAt(new DataFilter<DateTime, Request> { Share = f.Share });
            if (updatedAtValid.Success == false) return updatedAtValid;

            var userIdValid = await ValidateUserId(new DataFilter<string, Request> { Share = f.Share });
            if (userIdValid.Success == false) return userIdValid;

            var userValid = await ValidateUser(new DataFilter<ApplicationUser?, Request> { Share = f.Share });
            if (userValid.Success == false) return userValid;


            return ConditionResult.ToSuccess(f.Share, "Request data is valid.");
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.IsFull, "Request data is incomplete.")]
        private async Task<ConditionResult> CheckIsFull(DataFilter<object, Request> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "Request is null.");

            var isValid = await CheckIsValid(new DataFilter<object, Request> { Share = f.Share });
            if (isValid.Success == false) return isValid;

            // Check nullable properties if they are considered required for "fullness"
            var answerValid = await ValidateAnswer(new DataFilter<string?, Request> { Share = f.Share });
            if (answerValid.Success == false) return answerValid;

            var modelGatewayValid = await ValidateModelGateway(new DataFilter<string?, Request> { Share = f.Share });
            if (modelGatewayValid.Success == false) return modelGatewayValid;

            var modelAiValid = await ValidateModelAi(new DataFilter<string?, Request> { Share = f.Share });
            if (modelAiValid.Success == false) return modelAiValid;

            var subscriptionIdValid = await ValidateSubscriptionId(new DataFilter<string?, Request> { Share = f.Share });
            if (subscriptionIdValid.Success == false) return subscriptionIdValid;

            var subscriptionValid = await ValidateSubscription(new DataFilter<Subscription?, Request> { Share = f.Share });
            if (subscriptionValid.Success == false) return subscriptionValid;

            var serviceIdValid = await ValidateServiceId(new DataFilter<string?, Request> { Share = f.Share });
            if (serviceIdValid.Success == false) return serviceIdValid;

            var serviceValid = await ValidateService(new DataFilter<Service?, Request> { Share = f.Share });
            if (serviceValid.Success == false) return serviceValid;

            var spaceIdValid = await ValidateSpaceId(new DataFilter<string?, Request> { Share = f.Share });
            if (spaceIdValid.Success == false) return spaceIdValid;

            var spaceValid = await ValidateSpace(new DataFilter<Space?, Request> { Share = f.Share });
            if (spaceValid.Success == false) return spaceValid;

            var eventsValid = await ValidateEvents(new DataFilter<ICollection<EventRequest>, Request> { Share = f.Share });
            if (eventsValid.Success == false) return eventsValid;


            return ConditionResult.ToSuccess(f.Share, "Request data is complete.");
        }


        protected override async Task<Request?> GetModel(string? id)
        {
            if (_request != null && _request.Id == id)
                return _request;
            _request = await base.GetModel(id);
            return _request;
        }
    }
}