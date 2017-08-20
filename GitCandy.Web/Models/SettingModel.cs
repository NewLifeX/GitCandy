using GitCandy.Web.App_GlobalResources;
using System.ComponentModel.DataAnnotations;
using System;

namespace GitCandy.Models
{
    public class SettingModel
    {
        [Display(ResourceType = typeof(SR), Name = "Setting_IsPublicServer")]
        public Boolean IsPublicServer { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_ForceSsl")]
        public Boolean ForceSsl { get; set; }

        [Range(1, 65534, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_NumberRange")]
        [Display(ResourceType = typeof(SR), Name = "Setting_SslPort")]
        public Int32 SslPort { get; set; }

        [Range(1, 65534, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_NumberRange")]
        [Display(ResourceType = typeof(SR), Name = "Setting_SshPort")]
        public Int32 SshPort { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_EnableSsh")]
        public Boolean EnableSsh { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_LocalSkipCustomError")]
        public Boolean LocalSkipCustomError { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_AllowRegisterUser")]
        public Boolean AllowRegisterUser { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_AllowRepositoryCreation")]
        public Boolean AllowRepositoryCreation { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_RepositoryPath")]
        public String RepositoryPath { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_CachePath")]
        public String CachePath { get; set; }

        [Display(ResourceType = typeof(SR), Name = "Setting_GitCorePath")]
        public String GitCorePath { get; set; }

        [Range(5, 50, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_NumberRange")]
        [Display(ResourceType = typeof(SR), Name = "Setting_NumberOfCommitsPerPage")]
        public Int32 NumberOfCommitsPerPage { get; set; }

        [Range(5, 50, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_NumberRange")]
        [Display(ResourceType = typeof(SR), Name = "Setting_NumberOfItemsPerList")]
        public Int32 NumberOfItemsPerList { get; set; }

        [Range(10, 100, ErrorMessageResourceType = typeof(SR), ErrorMessageResourceName = "Validation_NumberRange")]
        [Display(ResourceType = typeof(SR), Name = "Setting_NumberOfRepositoryContributors")]
        public Int32 NumberOfRepositoryContributors { get; set; }
    }
}