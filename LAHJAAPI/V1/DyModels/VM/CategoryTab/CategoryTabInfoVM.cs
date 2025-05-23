using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// CategoryTab  property for VM Info.
    /// </summary>
    public class CategoryTabInfoVM : ITVM
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
        ///
        public String? UrlUsed { get; set; }
        ///
        public Int32 CountFalvet { get; set; }
        ///
        public Int32 Rateing { get; set; }
    }
}