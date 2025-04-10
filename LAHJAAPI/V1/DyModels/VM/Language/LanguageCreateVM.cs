using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using System;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Language  property for VM Create.
    /// </summary>
    public class LanguageCreateVM : ITVM
    {
        ///
        public String? Name { get; set; }
        ///
        public String? Code { get; set; }
    }
}