using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Setting  property for VM Create.
    /// </summary>
    public class SettingCreateVM : ITVM
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Value { get; set; }
    }
}