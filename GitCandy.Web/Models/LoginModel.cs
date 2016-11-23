using System;
using System.ComponentModel.DataAnnotations;
using GitCandy.Web.App_GlobalResources;

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
