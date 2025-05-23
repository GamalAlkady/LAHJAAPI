using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ServiceMethod  property for VM Info.
    /// </summary>
    public class ServiceMethodInfoVM : ITVM
    {
        public String? Id { get; set; }
        ///
        public String? Method { get; set; }
        ///
        public String? InputParameters { get; set; }
        ///
        public String? OutputParameters { get; set; }
        ///
        public String? ServiceId { get; set; }
        public ServiceOutputVM? Service { get; set; }
    }
}