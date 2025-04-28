using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// TypeModel  property for VM Update.
    /// </summary>
    public class TypeModelUpdateVM : ITVM
    {
        public TranslationData? Name { get; set; }
        //
        public TranslationData? Description { get; set; }
        ///
        public Boolean Active { get; set; }
        ///
        public String? Image { get; set; }
    }
}