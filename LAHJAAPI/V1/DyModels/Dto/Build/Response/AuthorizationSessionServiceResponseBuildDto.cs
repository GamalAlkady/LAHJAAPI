using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using AutoGenerator.Config;
using System;

namespace V1.DyModels.Dto.Build.Responses
{
    public class AuthorizationSessionServiceResponseBuildDto : ITBuildDto
    {
        /// <summary>
        /// AuthorizationSessionId property for DTO.
        /// </summary>
        public String? AuthorizationSessionId { get; set; }
        public AuthorizationSessionResponseBuildDto? AuthorizationSession { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceResponseBuildDto? Service { get; set; }
    }
}