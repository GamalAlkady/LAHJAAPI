using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using System;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// AuthorizationSessionService  property for VM Update.
    /// </summary>
    public class AuthorizationSessionServiceUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public AuthorizationSessionServiceCreateVM? Body { get; set; }
    }
}