using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Service  property for VM Create.
    /// </summary>
    public class ServiceCreateVM : ITVM
    {

        [Required]
        public String Name { get; set; }

        [Required]
        public String AbsolutePath { get; set; }

        [Required]
        public String Token { get; set; }

        [Required]
        public String ModelAiId { get; set; }
        //public ModelAiCreateVM? ModelAi { get; set; }
        //
        //public List<ServiceMethodCreateVM>? ServiceMethods { get; set; }
        //
        //public List<UserServiceCreateVM>? UserServices { get; set; }
        //
        //public List<RequestCreateVM>? Requests { get; set; }
    }
}