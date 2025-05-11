using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using AutoGenerator.Config;
using System;

namespace V1.DyModels.Dto.Build.ResponseFilters
{
    public class AuthorizationSessionServiceResponseFilterBuildDto : ITBuildDto
    {
        /// <summary>
        /// AuthorizationSessionId property for DTO.
        /// </summary>
        public String? AuthorizationSessionId { get; set; }
        public AuthorizationSessionResponseFilterBuildDto? AuthorizationSession { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceResponseFilterBuildDto? Service { get; set; }

        [FilterLGEnabled]
        public string? Lg { get; set; }
    }
}