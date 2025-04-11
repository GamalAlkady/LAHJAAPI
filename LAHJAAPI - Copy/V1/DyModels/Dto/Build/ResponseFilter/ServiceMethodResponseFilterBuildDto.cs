using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using AutoGenerator.Config;
using System;

namespace V1.DyModels.Dto.Build.ResponseFilters
{
    public class ServiceMethodResponseFilterBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; }
        /// <summary>
        /// Method property for DTO.
        /// </summary>
        public String? Method { get; set; }
        /// <summary>
        /// InputParameters property for DTO.
        /// </summary>
        public String? InputParameters { get; set; }
        /// <summary>
        /// OutputParameters property for DTO.
        /// </summary>
        public String? OutputParameters { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceResponseFilterBuildDto? Service { get; set; }

        [FilterLGEnabled]
        public string? Lg { get; set; }
    }
}