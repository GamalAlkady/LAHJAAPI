using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using AutoGenerator.Config;
using System;

namespace V1.DyModels.Dto.Build.Requests
{
    public class AuthorizationSessionServiceRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// AuthorizationSessionId property for DTO.
        /// </summary>
        public String? AuthorizationSessionId { get; set; }
        public AuthorizationSessionRequestBuildDto? AuthorizationSession { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceRequestBuildDto? Service { get; set; }
    }
}