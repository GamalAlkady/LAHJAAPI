using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using System;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// UserService  property for VM Update.
    /// </summary>
    public class UserServiceUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public UserServiceCreateVM? Body { get; set; }
    }
}