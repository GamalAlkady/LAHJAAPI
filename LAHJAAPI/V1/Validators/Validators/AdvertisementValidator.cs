using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;


namespace V1.Validators
{
    public enum AdvertisementValidatorStates
    {
        HasId,
        HasTitle,
        HasDescription,
        HasImage,
        IsActive,
        HasUrl,
        HasAdvertisementTabs,
        IsValid,
        IsFull
    }

    public class AdvertisementValidatorContext : ValidatorContext<Advertisement, AdvertisementValidatorStates>, ITValidator
    {
        private Advertisement? _entityCache;

        public AdvertisementValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasId, "Advertisement ID is required.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, Advertisement> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Advertisement ID is required.");
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasTitle, "Title is required.")]
        private Task<ConditionResult> ValidateTitle(DataFilter<string, Advertisement> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Title);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.Title) : ConditionResult.ToFailureAsync("Title is required.");
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasDescription, "Description is required.")]
        private Task<ConditionResult> ValidateDescription(DataFilter<string, Advertisement> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Description);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.Description) : ConditionResult.ToFailureAsync("Description is required.");
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasImage, "Image URL is required.")]
        private Task<ConditionResult> ValidateImage(DataFilter<string?, Advertisement> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Image);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Image) : ConditionResult.ToFailure("Image URL is required."));
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.IsActive, "Advertisement is not active.")]
        private Task<ConditionResult> ValidateActive(DataFilter<bool, Advertisement> f)
        {
            bool valid = f.Share?.Active == true;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("Advertisement is not active."));
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasUrl, "URL format is invalid.")]
        private Task<ConditionResult> ValidateUrl(DataFilter<string?, Advertisement> f)
        {
            var url = f.Share?.Url;
            bool valid = url == null || Uri.TryCreate(url, UriKind.Absolute, out _);
            return Task.FromResult(valid ? ConditionResult.ToSuccess(url) : ConditionResult.ToFailure("URL format is invalid."));
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.HasAdvertisementTabs, "Advertisement must have at least one tab.")]
        private Task<ConditionResult> ValidateAdvertisementTabs(DataFilter<ICollection<AdvertisementTab>?, Advertisement> f)
        {
            bool valid = f.Share?.AdvertisementTabs != null && f.Share.AdvertisementTabs.Any();
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.AdvertisementTabs) : ConditionResult.ToFailure("Advertisement must have at least one tab."));
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.IsValid, "Advertisement is invalid.")]
        private async Task<ConditionResult> CheckIsValid(DataFilter<object, Advertisement> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "Advertisement is null.");

            // Check core required fields
            var idValid = await ValidateId(new DataFilter<string, Advertisement> { Share = f.Share });
            if (idValid.Success == false) return idValid;

            var titleValid = await ValidateTitle(new DataFilter<string, Advertisement> { Share = f.Share });
            if (titleValid.Success == false) return titleValid;

            var descriptionValid = await ValidateDescription(new DataFilter<string, Advertisement> { Share = f.Share });
            if (descriptionValid.Success == false) return descriptionValid;

            return ConditionResult.ToSuccess(f.Share, "Advertisement is valid.");
        }

        [RegisterConditionValidator(typeof(AdvertisementValidatorStates), AdvertisementValidatorStates.IsFull, "Advertisement data is incomplete.")]
        private async Task<ConditionResult> CheckIsFull(DataFilter<object, Advertisement> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "Advertisement is null.");

            // First, check basic validity
            var isValid = await CheckIsValid(new DataFilter<object, Advertisement> { Share = f.Share });
            if (isValid.Success == false) return isValid;


            // Then check other "fullness" criteria
            var isActive = await ValidateActive(new DataFilter<bool, Advertisement> { Share = f.Share });
            if (isActive.Success == false) return isActive;

            var hasTabs = await ValidateAdvertisementTabs(new DataFilter<ICollection<AdvertisementTab>?, Advertisement> { Share = f.Share });
            if (hasTabs.Success == false) return hasTabs;

            // Add checks for Image and Url if they are considered part of "fullness"
            // var hasImage = await ValidateImage(new DataFilter<string, Advertisement> { Share = f.Share });
            // if (hasImage.Success == false) return hasImage;

            // var hasUrl = await ValidateUrl(new DataFilter<string, Advertisement> { Share = f.Share });
            // if (hasUrl.Success == false) return hasUrl;


            return ConditionResult.ToSuccess(f.Share, "Advertisement data is complete.");
        }


        protected override async Task<Advertisement?> GetModel(string? id)
        {
            if (_entityCache != null && _entityCache.Id == id)
                return _entityCache;
            _entityCache = await base.GetModel(id);
            return _entityCache;
        }
    }
}