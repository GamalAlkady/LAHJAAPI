using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// UserModelAi  property for VM Output.
    /// </summary>
    public class UserModelAiOutputVM : ITVM
    {
        ///
        public Int32 Id { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public String? UserId { get; set; }
        //public ApplicationUserOutputVM? User { get; set; }
        ///
        public String? ModelAiId { get; set; }
        //public ModelAiOutputVM? ModelAi { get; set; }
    }
}