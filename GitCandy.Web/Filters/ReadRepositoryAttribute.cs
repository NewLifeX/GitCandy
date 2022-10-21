using System;
using System.Web.Mvc;
using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GitCandy.Web.Filters;

/// <summary>读写代码库</summary>
public class ReadRepositoryAttribute : SmartAuthorizeAttribute
{
    private Boolean requireWrite;

    public ReadRepositoryAttribute(Boolean requireWrite = false)
    {
        this.requireWrite = requireWrite;
    }

    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        base.OnAuthorization(filterContext);

        var controller = filterContext.Controller as CandyControllerBase;

        var repoController = controller as RepositoryController;
        if (repoController != null)
        {
            var username = controller.Token?.Name;
            var owner = controller.ValueProvider.GetValue("owner");
            var field = controller.ValueProvider.GetValue("name");
            var canRead = owner != null && field != null && (requireWrite
                   ? repoController.RepositoryService.CanWriteRepository(owner.AttemptedValue, field.AttemptedValue, username)
                   : repoController.RepositoryService.CanReadRepository(owner.AttemptedValue, field.AttemptedValue, username));
            if (canRead) return;
        }

        HandleUnauthorizedRequest(filterContext);
    }
}