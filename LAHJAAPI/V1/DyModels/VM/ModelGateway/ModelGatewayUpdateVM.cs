using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelGateway  property for VM Update.
    /// </summary>
    public class ModelGatewayUpdateVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public String? Name { get; set; }
        ///
        public String? Url { get; set; }
        public Boolean IsDefault { get; set; }
    }
}