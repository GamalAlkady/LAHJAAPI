using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Setting  property for VM Update.
    /// </summary>
    public class SettingUpdateVM : ITVM
    {
        [Required]
        public String Value { get; set; }
    }
}