using System.ComponentModel;
using NewLife.GitCandy.Entity;
using NewLife.Data;
using Xunit;

namespace GitCandyXUnitTest.Entity;

/// <summary>用户团队关联测试</summary>
[Collection("GitCandyTests")]
public class UserTeamTests
{
    private User _user;
    private User _team;

    public UserTeamTests(DatabaseFixture fixture)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        _user = User.Create("teammember_" + suffix, "团队成员", "pwd", "member_" + suffix + "@test.com", null);
        _team = User.CreateTeam("testteam_" + suffix, "测试团队", "团队描述");
    }

    private UserTeam AddToTeam(User user, User team, Boolean isAdmin = false)
    {
        var ut = new UserTeam
        {
            UserID = user.ID,
            TeamID = team.ID,
            IsAdmin = isAdmin
        };
        ut.Save();
        return ut;
    }

    [Fact]
    [DisplayName("FindByUserIDAndTeamID_存在关联_返回正确记录")]
    public void FindByUserIDAndTeamID_ExistingRelation_ReturnsCorrect()
    {
        AddToTeam(_user, _team);
        var found = UserTeam.FindByUserIDAndTeamID(_user.ID, _team.ID);
        Assert.NotNull(found);
        Assert.Equal(_user.ID, found.UserID);
        Assert.Equal(_team.ID, found.TeamID);
    }

    [Fact]
    [DisplayName("FindByUserIDAndTeamID_不存在关联_返回Null")]
    public void FindByUserIDAndTeamID_NoRelation_ReturnsNull()
    {
        var found = UserTeam.FindByUserIDAndTeamID(_user.ID, -999);
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("FindByUserAndTeam_有效用户和团队_返回关联")]
    public void FindByUserAndTeam_ValidUserAndTeam_ReturnsRelation()
    {
        AddToTeam(_user, _team);
        var found = UserTeam.FindByUserAndTeam(_user.Name, _team.Name);
        Assert.NotNull(found);
    }

    [Fact]
    [DisplayName("FindAllByUserID_有效用户_返回所有团队")]
    public void FindAllByUserID_ValidUser_ReturnsAllTeams()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var team2 = User.CreateTeam("team2_" + suffix, "团队2", null);
        AddToTeam(_user, _team);
        AddToTeam(_user, team2);

        var list = UserTeam.FindAllByUserID(_user.ID);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    [DisplayName("FindAllByTeamID_有效团队_返回所有成员")]
    public void FindAllByTeamID_ValidTeam_ReturnsAllMembers()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var user2 = User.Create("member2_" + suffix, "成员2", "pwd", "m2_" + suffix + "@test.com", null);
        AddToTeam(_user, _team);
        AddToTeam(user2, _team);

        var list = UserTeam.FindAllByTeamID(_team.ID);
        Assert.Equal(2, list.Count);
    }
}
