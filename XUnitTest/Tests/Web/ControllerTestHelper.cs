using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NewLife.Model;

namespace GitCandyXUnitTest.Web;

/// <summary>Web控制器测试辅助。构造带会话的HttpContext，注入当前用户</summary>
internal static class ControllerTestHelper
{
    /// <summary>创建带有模拟会话的HttpContext</summary>
    /// <param name="token">当前登录用户，可为空</param>
    /// <returns></returns>
    public static DefaultHttpContext CreateHttpContext(IManageUser? token)
    {
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();
        ctx.Request.Scheme = "http";
        ctx.Request.Host = new HostString("localhost");
        ctx.Request.Path = "/";

        var session = new Dictionary<String, Object?>();
        session["GitToken"] = token;
        session["GitCandyGitAuthorize"] = null;
        ctx.Items["Session"] = session;

        return ctx;
    }

    /// <summary>准备Candy控制器。设置HttpContext并触发OnActionExecuting完成用户同步</summary>
    /// <param name="controller">控制器</param>
    /// <param name="token">当前登录用户，可为空</param>
    /// <typeparam name="T">控制器类型</typeparam>
    /// <returns></returns>
    public static T Prepare<T>(T controller, IManageUser? token) where T : CandyControllerBase
    {
        var ctx = CreateHttpContext(token);
        controller.ControllerContext = new ControllerContext { HttpContext = ctx, RouteData = new RouteData(), ActionDescriptor = new ControllerActionDescriptor() };

        var context = new ActionExecutingContext(
            controller.ControllerContext,
            [],
            new Dictionary<String, Object?>(),
            controller
        );
        controller.OnActionExecuting(context);

        return controller;
    }

    /// <summary>准备普通控制器。仅设置HttpContext，不执行过滤器</summary>
    /// <param name="controller">控制器</param>
    /// <param name="token">当前登录用户，可为空</param>
    /// <typeparam name="T">控制器类型</typeparam>
    /// <returns></returns>
    public static T PrepareRaw<T>(T controller, IManageUser? token) where T : Controller
    {
        var ctx = CreateHttpContext(token);
        controller.ControllerContext = new ControllerContext { HttpContext = ctx, RouteData = new RouteData(), ActionDescriptor = new ControllerActionDescriptor() };

        return controller;
    }

    /// <summary>以指定查询串执行动作过滤器，用于验证参数回填</summary>
    /// <param name="controller">控制器</param>
    /// <param name="token">当前登录用户，可为空</param>
    /// <param name="query">查询串，如 ?path=master</param>
    /// <param name="arguments">动作参数</param>
    /// <returns>执行后的动作参数</returns>
    public static IDictionary<String, Object?> ExecuteFilters(CandyControllerBase controller, IManageUser? token, String query, IDictionary<String, Object?> arguments)
    {
        var ctx = CreateHttpContext(token);
        ctx.Request.QueryString = new QueryString(query);
        controller.ControllerContext = new ControllerContext { HttpContext = ctx, RouteData = new RouteData(), ActionDescriptor = new ControllerActionDescriptor() };

        var context = new ActionExecutingContext(controller.ControllerContext, [], arguments, controller);
        controller.OnActionExecuting(context);

        return arguments;
    }
}
