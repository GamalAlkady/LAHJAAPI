using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelGateway  property for VM Output.
    /// </summary>
    public class ModelGatewayOutputVM : ITVM
    {
        ///
        public String? Id { get; set; }
        ///
        public String? Name { get; set; }
        ///
        public String? Url { get; set; }
        ///
        public String? Token { get; set; }
        ///
        public Boolean IsDefault { get; set; }
    }
}