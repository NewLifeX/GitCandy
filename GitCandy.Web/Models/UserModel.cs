using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GitCandy.Web.App_GlobalResources;
using GitCandy.Base;

namespace GitCandy.Models
{
    public class UserModel
    {
        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(20, MinimumLength = 2, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [RegularExpression(RegularExpression.Username, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Name")]
        [Display(ResourceType = typeof(SR), Name = "Account_Username")]
        public String Name { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLength")]
        [Display(ResourceType = typeof(SR), Name = "Account_Nickname")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String Nickname { get; set; }

        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLength")]
        [RegularExpression(RegularExpression.Email, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Email")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(SR), Name = "Account_Email")]
        public String Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(20, MinimumLength = 5, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(SR), Name = "Account_Password")]
        public String Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Required")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLengthRange")]
        [Compare("Password", ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_Compare")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(SR), Name = "Account_ConformPassword")]
        public String ConformPassword { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_StringLength")]
        [Display(ResourceType = typeof(SR), Name = "Account_Description")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String Description { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Account_IsSystemAdministrator")]
        [UIHint("YesNo")]
        public bool IsAdmin { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Account_Teams")]
        [UIHint("Maps")]
        [System.Web.Mvc.AdditionalMetadata("Controller", "Team")]
        public IDictionary<String, String> Teams { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Account_Repositories")]
        [UIHint("Members")]
        [System.Web.Mvc.AdditionalMetadata("Controller", "Repository")]
        public String[] Respositories { get; set; }
    }
}
