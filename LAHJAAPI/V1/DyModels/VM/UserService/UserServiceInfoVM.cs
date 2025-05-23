using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// UserService  property for VM Info.
    /// </summary>
    public class UserServiceInfoVM : ITVM
    {
        public Int32 Id { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? UserId { get; set; }
        public ApplicationUserOutputVM? User { get; set; }
        ///
        public String? ServiceId { get; set; }
        public ServiceOutputVM? Service { get; set; }
    }
}