using System;
using System.ComponentModel.DataAnnotations;
using GitCandy.Web.App_GlobalResources;

namespace GitCandy.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(100, MinimumLength =5, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(SR), Name = "Account_OldPassword")]
        public String OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(SR), Name = "Account_NewPassword")]
        public String NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Compare")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(SR), Name = "Account_ConformPassword")]
        public String ConformPassword { get; set; }
    }
}
