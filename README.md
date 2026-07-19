## GitCandy

GitCandy© 是一个基于 .NET 的 [Git](http://git-scm.com/documentation) 分布式版本控制平台（Git Hosting Service），支持公有和私有代码库，可不受限制地创建代码库，随时随地与团队进行协作。

本分支基于 ASP.NET Core MVC + [NewLife.Cube 魔方](http://git.newlifex.com/NewLife/X/Tree/master/NewLife.Cube) 重构，主要特点：免部署、团队与个人下属源码库两级管理。

演示：[http://git.newlifex.com/](http://git.newlifex.com/)

源码：http://git.NewLifeX.com/NewLife/GitCandy  
海外：https://github.com/NewLifeX/GitCandy  

GitCandy官方群：200319579    新生命群：1600800

### 📚 文档索引

| 文档 | 说明 |
|------|------|
| [需求文档](Doc/需求文档.md) | 愿景、核心目标与功能需求 |
| [功能清单](Doc/功能清单.md) | 功能点实现/测试/注释三维追踪 |
| [架构设计](Doc/架构设计.md) | 分层架构、核心组件与关键流程 |

---
### 系统要求
* [.NET 10.0](https://dotnet.microsoft.com/)（或更高版本）
* [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
* [Git](http://git-for-windows.github.io/)（git-core 用于传输协议）
* [SQLite](https://www.sqlite.org/) 或 [SqlServer](https://www.microsoft.com/sqlserver/)（通过 XCode 支持多数据库）

---
### 安装
* 下载最新[发布](https://github.com/NewLifeX/GitCandy/releases)版本或编译最新 [master](http://git.newlifex.com/NewLife/GitCandy) 分支源码
* 发布后直接运行 `GitCandy.exe`（自宿主 Kestrel），或部署到 IIS
* 如果用了 Visual Studio 的发布功能，还需复制 `runtimes/` 目录到站点目录
* 打开站点，默认管理员用户名 `admin`，密码 `admin`
* 转到魔方后台「糖果配置」页面设置 `代码库`、`缓存` 和 `git-core` 路径
* 推荐生产环境设置 `ASPNETCORE_ENVIRONMENT=Production`

##### *注*
* `代码库`和`缓存`路径示例：`x:\Repos`，`x:\Cache`
* `git-core`路径示例：`x:\PortableGit\libexec\git-core`，`x:\PortableGit\mingw64\libexec\git-core`

---
### 鸣谢 (按字母序)
* [ASP.NET MVC](http://aspnetwebstack.codeplex.com/) @ [Apache License 2.0](http://aspnetwebstack.codeplex.com/license)
* [Bootstrap](http://github.com/twbs/bootstrap) @ [MIT License](http://github.com/twbs/bootstrap/blob/master/LICENSE)
* [Bootstrap-switch](http://github.com/nostalgiaz/bootstrap-switch) @ [Apache License 2.0](http://github.com/nostalgiaz/bootstrap-switch/blob/master/LICENSE)
* [Highlight.js](http://github.com/isagalaev/highlight.js) @ [New BSD License](http://github.com/isagalaev/highlight.js/blob/master/LICENSE)
* [jQuery](http://github.com/jquery/jquery) @ [MIT License](http://github.com/jquery/jquery/blob/master/MIT-LICENSE.txt)
* [LibGit2Sharp](http://github.com/libgit2/libgit2sharp) @ [MIT License](http://github.com/libgit2/libgit2sharp/blob/master/LICENSE.md)
* [marked](http://github.com/chjj/marked) @ [MIT License](http://github.com/chjj/marked/blob/master/LICENSE)

---
### 协议
MIT 协议
