using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ServiceMethod  property for VM Update.
    /// </summary>
    public class ServiceMethodUpdateVM : ITVM
    {
        public String? Method { get; set; }
        ///
        public String? InputParameters { get; set; }
        ///
        public String? OutputParameters { get; set; }
    }
}