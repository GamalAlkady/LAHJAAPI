using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelGateway  property for VM Create.
    /// </summary>
    public class ModelGatewayCreateVM : ITVM
    {
        [Required]
        public String Name { get; set; }
        ///
        public String? Url { get; set; }
        public Boolean IsDefault { get; set; }
    }
}