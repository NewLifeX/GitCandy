using System.ComponentModel;
using GitCandy.Data;
using GitCandy.Models;
using NewLife.GitCandy.Entity;
using Xunit;

namespace GitCandyXUnitTest.Services;

/// <summary>成员服务测试</summary>
[Collection("GitCandyTests")]
public class MembershipServiceTests
{
    private readonly MembershipService _service;
    private User _user;
    private User _team;

    public MembershipServiceTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _service = new MembershipService();
        _user = User.Create("msuser_" + suffix, "成员服务用户", "pwd", "msuser_" + suffix + "@test.com", null);
        _team = User.CreateTeam("msteam_" + suffix, "成员服务团队", "团队描述");
    }

    [Fact]
    [DisplayName("GetUserModel_存在用户_返回正确模型")]
    public void GetUserModel_ExistingUser_ReturnsCorrectModel()
    {
        var model = _service.GetUserModel(_user.Name, true);
        Assert.NotNull(model);
        Assert.Equal(_user.Name, model.Name);
        Assert.Equal("成员服务用户", model.Nickname);
        Assert.Equal(_user.Email, model.Email);
    }

    [Fact]
    [DisplayName("GetUserModel_不存在用户_返回Null")]
    public void GetUserModel_NonexistentUser_ReturnsNull()
    {
        var model = _service.GetUserModel("nonexistent_person_" + Guid.NewGuid().ToString("N"));
        Assert.Null(model);
    }

    [Fact]
    [DisplayName("TeamAddUser_有效团队和用户_添加成功")]
    public void TeamAddUser_ValidTeamAndUser_AddsSuccessfully()
    {
        var result = _service.TeamAddUser(_team.Name, _user.Name);
        Assert.True(result);

        var ut = UserTeam.FindByUserAndTeam(_user.Name, _team.Name);
        Assert.NotNull(ut);
    }

    [Fact]
    [DisplayName("TeamRemoveUser_存在关联_移除成功")]
    public void TeamRemoveUser_ExistingRelation_RemovesSuccessfully()
    {
        _service.TeamAddUser(_team.Name, _user.Name);
        var result = _service.TeamRemoveUser(_team.Name, _user.Name);
        Assert.True(result);

        var ut = UserTeam.FindByUserAndTeam(_user.Name, _team.Name);
        Assert.Null(ut);
    }

    [Fact]
    [DisplayName("InTeam_用户在团队中_返回True")]
    public void InTeam_UserInTeam_ReturnsTrue()
    {
        _service.TeamAddUser(_team.Name, _user.Name);
        Assert.True(_service.InTeam(_team.Name, _user.Name));
    }

    [Fact]
    [DisplayName("InTeam_用户不在团队_返回False")]
    public void InTeam_UserNotInTeam_ReturnsFalse()
    {
        Assert.False(_service.InTeam(_team.Name, _user.Name));
    }

    [Fact]
    [DisplayName("GetTeamModel_存在团队_返回模型")]
    public void GetTeamModel_ExistingTeam_ReturnsModel()
    {
        var model = _service.GetTeamModel(_team.Name, false);
        Assert.NotNull(model);
        Assert.Equal(_team.Name, model.Name);
        Assert.Equal("成员服务团队", model.Nickname);
    }

    [Fact]
    [DisplayName("UpdateTeam_有效数据_更新团队信息")]
    public void UpdateTeam_ValidData_UpdatesTeam()
    {
        var model = new TeamModel
        {
            Name = _team.Name,
            Nickname = "更新后的团队名",
            Description = "更新后的描述",
        };
        var result = _service.UpdateTeam(model);
        Assert.True(result);

        var team = User.FindByName(_team.Name);
        Assert.NotNull(team);
        Assert.Equal("更新后的团队名", team.NickName);
    }

    [Fact]
    [DisplayName("DeleteTeam_存在团队_删除成功")]
    public void DeleteTeam_ExistingTeam_DeletesSuccessfully()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var testTeam = User.CreateTeam("delete_team_" + suffix, "待删除团队", null);
        var result = _service.DeleteTeam(testTeam.Name);
        Assert.True(result);

        var deleted = User.FindByName(testTeam.Name);
        Assert.Null(deleted);
    }
}
