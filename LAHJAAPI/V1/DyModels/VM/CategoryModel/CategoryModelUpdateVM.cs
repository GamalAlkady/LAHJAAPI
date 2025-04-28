using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// CategoryModel  property for VM Update.
    /// </summary>
    public class CategoryModelUpdateVM : ITVM
    {
        public TranslationData? Name { get; set; }
        //
        public TranslationData? Description { get; set; }
    }
}