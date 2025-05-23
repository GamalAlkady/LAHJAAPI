using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.Models;
using System;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// FAQItem  property for VM Output.
    /// </summary>
    public class FAQItemOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        //
        public string? Question { get; set; }
        //
        public string? Answer { get; set; }
        //
        public string? Tag { get; set; }
    }
}