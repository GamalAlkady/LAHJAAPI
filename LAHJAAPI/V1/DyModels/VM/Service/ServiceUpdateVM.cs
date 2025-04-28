using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Service  property for VM Update.
    /// </summary>
    public class ServiceUpdateVM : ITVM
    {
        [Required]
        public String AbsolutePath { get; set; }

        [Required]
        public String Token { get; set; }
    }
}