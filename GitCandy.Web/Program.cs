using GitCandy.Base;
using GitCandy.Git.Cache;
using GitCandy.Web.Base;
using GitCandy.Web.Controllers;
using GitCandy.Web.Services;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Log;
using XCode;

//!!! 标准Web项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust(null);

// 启用星尘配置中心。分布式部署或容器化部署推荐使用，单机部署不推荐使用
var config = star.Config;

// 默认内存缓存，如有配置可使用Redis缓存
var cache = new MemoryCache();
services.AddSingleton<ICache>(cache);

services.AddSingleton<AccountService>();

// 启用接口响应压缩
services.AddResponseCompression();
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Bearer";
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
    options.DefaultSignInScheme = "Bearer";
});
services.AddControllersWithViews();

// 引入魔方
services.AddCube();

// 后台服务
//services.AddHostedService<MyHostedService>();

var app = builder.Build();

// 预热数据层，执行自动建表等操作
// 连接名 Zero 对应连接字符串名字，同时也对应 Zero.Data/Projects/Model.xml 头部的 ConnName
EntityFactory.InitConnection("GitCandy");

// 使用Cube前添加自己的管道
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/CubeHome/Error");

app.UseResponseCompression();
app.UseStaticFiles();

// 使用魔方
app.UseCube(app.Environment);

app.UseAuthorization();

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
app.RegisterService("GitCandy", null, app.Environment.EnvironmentName);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "HomeIndex",
        pattern: "{controller=Repository}/{action=Index}");

    #region GitController
    endpoints.MapControllerRoute(
        name: "UserGit",
        pattern: "{owner}/{project}/{*verb}",
        defaults: new { controller = "Git", action = "Smart" },
        constraints: new { owner = new UserUrlConstraint(), verb = new GitUrlConstraint() }
    );
    //endpoints.MapControllerRoute(
    //    name: "GitAct",
    //    url: "{owner}/{name}/{action}/{branch}/{*path}",
    //    defaults: new { controller = "Repository", path = UrlParameter.Optional },
    //    constraints: new { owner = new UserUrlConstraint() },
    //    namespaces: new[] { typeof(AccountController).Namespace }
    //);
    endpoints.MapControllerRoute(
        name: "UserGitAct",
        pattern: "{owner}/{name}/{action}/{*path}",
        defaults: new { controller = "Repository" },
        constraints: new { owner = new UserUrlConstraint() }
    );
    endpoints.MapControllerRoute(
        name: "UserGitWeb",
        pattern: "{owner}/{name}",
        defaults: new { controller = "Repository", action = "Tree" },
        constraints: new { owner = new UserUrlConstraint() }
    );
    #endregion

    #region AccountContorller
    // 实现用户名直达用户首页
    endpoints.MapControllerRoute(
        name: "UserIndex",
        pattern: "{name}",
        defaults: new { controller = "Account", action = "Detail" },
        constraints: new { name = new UserUrlConstraint { IsTeam = false } }
    );
    endpoints.MapControllerRoute(
        name: "User",
        pattern: "User/{action}/{name}",
        defaults: new { controller = "Account" }
    );
    #endregion

    #region TeamContorller
    // 实现团队名直达团队首页
    endpoints.MapControllerRoute(
        name: "TeamIndex",
        pattern: "{name}",
        defaults: new { controller = "Team", action = "Detail" },
        constraints: new { name = new UserUrlConstraint { IsTeam = true } }
    );
    endpoints.MapControllerRoute(
        name: "Team",
        pattern: "Team/{action}/{name}",
        defaults: new { controller = "Team" }
    );
    #endregion

    #region RepositoryController
    endpoints.MapControllerRoute(
        name: "Repository",
        pattern: "{controller=Repository}/{action}/{name}/{*path}",
        defaults: new { controller = "Repository", path = "" }
    );
    #endregion
});

GitCacheAccessor.Initialize();

app.Run();