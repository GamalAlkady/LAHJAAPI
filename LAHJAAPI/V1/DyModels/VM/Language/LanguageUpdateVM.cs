using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Language  property for VM Update.
    /// </summary>
    public class LanguageUpdateVM : ITVM
    {

        public TranslationData? Name { get; set; }
        ///
        public String? Code { get; set; }
    }
}