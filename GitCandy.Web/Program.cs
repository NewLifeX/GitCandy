using System.Reflection;
using GitCandy.Git.Cache;
using GitCandy.Web.Base;
using GitCandy.Web.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using NewLife;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Log;
using XCode;

//!!! 标准Web项目模板，新生命团队强烈推荐

// 启用控制台日志，拦截所有异常
XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// 配置星尘。借助StarAgent，或者读取配置文件 config/star.config 中的服务器地址
var star = services.AddStardust(null);

services.AddSingleton<AccountService>();

// 启用接口响应压缩
services.AddResponseCompression();

services.AddControllersWithViews();

// 引入魔方
services.AddCube();

// 后台服务
//services.AddHostedService<MyHostedService>();

// 解决文件上传Request body too large
services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = Int32.MaxValue;
});
// 接口请求限制
services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = Int32.MaxValue;
});

var app = builder.Build();

// 预热数据层，执行自动建表等操作
// 连接名 Zero 对应连接字符串名字，同时也对应 Zero.Data/Projects/Model.xml 头部的 ConnName
EntityFactory.InitConnection("GitCandy");

// 使用Cube前添加自己的管道
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/CubeHome/Error");

if (Environment.GetEnvironmentVariable("__ASPNETCORE_BROWSER_TOOLS") is null)
    app.UseResponseCompression();

app.UseStaticFiles();

// 独立静态文件设置，魔方自己的静态资源内嵌在程序集里面
var env = app.Environment;
var options = new StaticFileOptions();
{
    var embeddedProvider = new CubeEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "GitCandy.Web.wwwroot");
    if (!env.WebRootPath.IsNullOrEmpty() && Directory.Exists(env.WebRootPath))
        options.FileProvider = new CompositeFileProvider(new PhysicalFileProvider(env.WebRootPath), embeddedProvider);
    else
        options.FileProvider = embeddedProvider;
}
app.UseStaticFiles(options);

// 使用魔方
app.UseCube(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

// 启用星尘注册中心，向注册中心注册服务，服务消费者将自动更新服务端地址列表
app.RegisterService("GitCandy", null, app.Environment.EnvironmentName);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "HomeIndex",
    pattern: "{controller=Repository}/{action=Index}");

#region GitController
app.MapControllerRoute(
    name: "UserGit",
    pattern: "{owner}/{project}/{*verb}",
    defaults: new { controller = "Git", action = "Smart" },
    constraints: new { owner = new UserUrlConstraint(), verb = new GitUrlConstraint() }
);
//app.MapControllerRoute(
//    name: "GitAct",
//    url: "{owner}/{name}/{action}/{branch}/{*path}",
//    defaults: new { controller = "Repository", path = UrlParameter.Optional },
//    constraints: new { owner = new UserUrlConstraint() },
//    namespaces: new[] { typeof(AccountController).Namespace }
//);
app.MapControllerRoute(
    name: "UserGitAct",
    pattern: "{owner}/{name}/{action}/{*path}",
    defaults: new { controller = "Repository" },
    constraints: new { owner = new UserUrlConstraint() }
);
app.MapControllerRoute(
    name: "UserGitWeb",
    pattern: "{owner}/{name}",
    defaults: new { controller = "Repository", action = "Tree" },
    constraints: new { owner = new UserUrlConstraint() }
);
#endregion

#region AccountContorller
// 实现用户名直达用户首页
app.MapControllerRoute(
    name: "UserIndex",
    pattern: "{name}",
    defaults: new { controller = "Account", action = "Detail" },
    constraints: new { name = new UserUrlConstraint { IsTeam = false } }
);
app.MapControllerRoute(
    name: "User",
    pattern: "User/{action}/{name}",
    defaults: new { controller = "Account" }
);
#endregion

#region TeamContorller
// 实现团队名直达团队首页
app.MapControllerRoute(
    name: "TeamIndex",
    pattern: "{name}",
    defaults: new { controller = "Team", action = "Detail" },
    constraints: new { name = new UserUrlConstraint { IsTeam = true } }
);
app.MapControllerRoute(
    name: "Team",
    pattern: "Team/{action}/{name}",
    defaults: new { controller = "Team" }
);
#endregion

#region RepositoryController
app.MapControllerRoute(
    name: "Repository",
    pattern: "{controller=Repository}/{action}/{name}/{*path}",
    defaults: new { controller = "Repository", path = "" }
);
#endregion

GitCacheAccessor.Initialize();

app.Run();