## GitCandy
GitCandy© 是一个基于 ASP.NET MVC 的 [Git](http://git-scm.com/documentation) 版本控制服务端，支持公有和私有代码库，可不受限制的创建代码代码库，随时随地的与团队进行协作。  

GitCandy© 由团队成员[Aimeast](https://github.com/Aimeast/GitCandy)创建，本分支引入[魔方](http://git.newlifex.com/NewLife/X/Tree/master/NewLife.Cube)并进行功能调整，主要改进为免部署，以及支持团队个人下属源码库两级管理。  

演示：[http://git.newlifex.com/](http://git.newlifex.com/)

源码： http://git.NewLifeX.com/NewLife/GitCandy  
海外： https://github.com/NewLifeX/GitCandy  

GitCandy官方群：200319579    新生命群：1600800

---
### 系统要求
* [IIS 7.0](http://www.iis.net/learn)
* [.NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
* [ASP.NET MVC 5](http://www.asp.net/mvc/tutorials/mvc-5)
* [Git](http://git-for-windows.github.io/)
* [Sqlite](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki) 或 [Sql Server](http://www.microsoft.com/en-us/sqlserver/get-sql-server/try-it.aspx)

---
### 安装
* 下载最新[发布](https://github.com/NewLifeX/GitCandy/releases)的版本或自己编译最新的[master](http://git.newlifex.com/NewLife/GitCandy)分支源码
* 在IIS创建一个站点，并把二进制文件和资源文件复制到站点目录
* 如果用了 Visual Studio 的发布功能，还要复制`GitCandy\bin\[NativeBinaries & x86 & x64]`文件夹到站点目录
* 打开新建的站点，默认登录用户名是`admin`，密码是`gitcandy`
* 转到`设置`页面，分别设置`代码库`，`缓存`和`git-core`的路径
* 推荐在`Web.config`设置`<compilation debug="false" />`

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
