using GitCandy.App_GlobalResources;
using System.ComponentModel.DataAnnotations;
using System;

namespace GitCandy.Models
{
    public class LoginModel
    {
        [Display(ResourceType = typeof(SR), Name = "Account_UsernameOrEmail")]
        public String ID { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Account_Password")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}
