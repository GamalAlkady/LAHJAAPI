using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelGateway  property for VM Update.
    /// </summary>
    public class ModelGatewayUpdateVM : ITVM
    {
        [Required]
        public String Name { get; set; }
        ///
        public String? Url { get; set; }
        public Boolean IsDefault { get; set; }
    }
}