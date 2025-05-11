using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using System;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// AuthorizationSessionService  property for VM Create.
    /// </summary>
    public class AuthorizationSessionServiceCreateVM : ITVM
    {
        ///
        public String? AuthorizationSessionId { get; set; }
        public AuthorizationSessionCreateVM? AuthorizationSession { get; set; }
        ///
        public String? ServiceId { get; set; }
        public ServiceCreateVM? Service { get; set; }
    }
}