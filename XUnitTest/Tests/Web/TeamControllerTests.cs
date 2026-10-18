using System.ComponentModel;
using GitCandy.Models;
using GitCandy.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Web;

/// <summary>团队控制器测试。覆盖团队列表、增删改和成员管理</summary>
[Collection("GitCandyTests")]
public class TeamControllerTests
{
    private readonly User _admin;
    private readonly User _member;

    public TeamControllerTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];

        _admin = User.Create("teamadmin_" + suffix, "团队管理员", "pwd", "teamadmin_" + suffix + "@test.com", null);
        _admin.IsAdmin = true;
        _admin.Update();

        _member = User.Create("teammember_" + suffix, "团队成员", "pwd", "teammember_" + suffix + "@test.com", null);
    }

    private static TeamController NewController(User token) => ControllerTestHelper.Prepare(new TeamController(), token);

    [Fact]
    [DisplayName("Index_管理员_返回团队列表")]
    public void Index_Admin_ReturnsTeamList()
    {
        var team = User.CreateTeam("listteam" + Guid.NewGuid().ToString("N")[..6], "列表团队", null);
        var controller = NewController(_admin);

        var result = controller.Index(team.Name, 1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeamListModel>(view.Model);
        Assert.Contains(model.Teams, e => e.Name == team.Name);
    }

    [Fact]
    [DisplayName("Index_非管理员_拒绝访问")]
    public void Index_NonAdmin_Forbids()
    {
        var result = NewController(_member).Index(null, 1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    [DisplayName("Create_提交有效模型_创建团队并跳转")]
    public void Create_ValidModel_CreatesTeamAndRedirects()
    {
        var name = "newteam_" + Guid.NewGuid().ToString("N")[..6];
        var controller = NewController(_admin);

        var result = controller.Create(new TeamModel { Name = name, Nickname = "新团队", Description = "测试创建" });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Detail", redirect.ActionName);
        Assert.Equal("Team", redirect.ControllerName);

        var team = User.FindByName(name);
        Assert.NotNull(team);
        Assert.True(team.IsTeam);
        Assert.Equal("新团队", team.NickName);
    }

    [Fact]
    [DisplayName("Detail_存在团队_返回团队详情")]
    public void Detail_ExistingTeam_ReturnsDetail()
    {
        var team = User.CreateTeam("detailteam_" + Guid.NewGuid().ToString("N")[..6], "详情团队", "描述");
        var controller = NewController(_admin);

        var result = controller.Detail(team.Name);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeamModel>(view.Model);
        Assert.Equal(team.Name, model.Name);
    }

    [Fact]
    [DisplayName("Edit_提交有效模型_更新团队信息")]
    public void Edit_ValidModel_UpdatesTeam()
    {
        var team = User.CreateTeam("editteam_" + Guid.NewGuid().ToString("N")[..6], "编辑团队", null);
        var controller = NewController(_admin);

        var result = controller.Edit(team.Name, new TeamModel { Name = team.Name, Nickname = "更新团队" });

        Assert.IsType<ViewResult>(result);
        var updated = User.FindByName(team.Name);
        Assert.Equal("更新团队", updated.NickName);
    }

    [Fact]
    [DisplayName("ChooseUser_添加成员_成功")]
    public void ChooseUser_AddMember_Succeeds()
    {
        var team = User.CreateTeam("addteam_" + Guid.NewGuid().ToString("N")[..6], "添加成员团队", null);
        var controller = NewController(_admin);

        var result = controller.ChooseUser(team.Name, _member.Name, "add");

        var json = Assert.IsType<JsonResult>(result);
        Assert.Equal("success", json.Value);
        Assert.True(UserTeam.FindByUserIDAndTeamID(_member.ID, team.ID) != null);
    }

    [Fact]
    [DisplayName("ChooseUser_设置管理员和普通成员_正确切换")]
    public void ChooseUser_SetAdministratorAndMember_TogglesCorrectly()
    {
        var team = User.CreateTeam("roleteam_" + Guid.NewGuid().ToString("N")[..6], "角色团队", null);
        var controller = NewController(_admin);
        controller.ChooseUser(team.Name, _member.Name, "add");

        var admin = controller.ChooseUser(team.Name, _member.Name, "admin");
        Assert.Equal("success", Assert.IsType<JsonResult>(admin).Value);
        Assert.True(UserTeam.FindByUserIDAndTeamID(_member.ID, team.ID).IsAdmin);

        var member = controller.ChooseUser(team.Name, _member.Name, "member");
        Assert.Equal("success", Assert.IsType<JsonResult>(member).Value);
        Assert.False(UserTeam.FindByUserIDAndTeamID(_member.ID, team.ID).IsAdmin);
    }

    [Fact]
    [DisplayName("ChooseUser_移除成员_成功")]
    public void ChooseUser_RemoveMember_Succeeds()
    {
        var team = User.CreateTeam("delteam_" + Guid.NewGuid().ToString("N")[..6], "移除成员团队", null);
        var controller = NewController(_admin);
        controller.ChooseUser(team.Name, _member.Name, "add");

        var result = controller.ChooseUser(team.Name, _member.Name, "del");

        Assert.Equal("success", Assert.IsType<JsonResult>(result).Value);
        Assert.Null(UserTeam.FindByUserIDAndTeamID(_member.ID, team.ID));
    }

    [Fact]
    [DisplayName("Delete_确认删除_移除团队")]
    public void Delete_Confirmed_RemovesTeam()
    {
        var team = User.CreateTeam("delteam2_" + Guid.NewGuid().ToString("N")[..6], "待删除团队", null);
        var controller = NewController(_admin);

        var result = controller.Delete(team.Name, "yes");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Null(User.FindByName(team.Name));
    }

    [Fact]
    [DisplayName("Search_关键词_返回匹配团队")]
    public void Search_Keyword_ReturnsMatchedTeams()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var team = User.CreateTeam("searchteam" + suffix, "搜索团队", null);
        var controller = NewController(_admin);

        var result = controller.Search("searchteam" + suffix);

        var json = Assert.IsType<JsonResult>(result);
        var names = Assert.IsType<String[]>(json.Value);
        Assert.Contains(team.Name, names);
    }
}
