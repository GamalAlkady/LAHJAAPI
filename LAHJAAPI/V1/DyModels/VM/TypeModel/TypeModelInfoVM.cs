using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// TypeModel  property for VM Info.
    /// </summary>
    public class TypeModelInfoVM : ITVM
    {
        public String? Id { get; set; }
        //
        public string? Name { get; set; }
        //
        public string? Description { get; set; }
        ///
        public Boolean Active { get; set; }
        ///
        public String? Image { get; set; }
    }
}