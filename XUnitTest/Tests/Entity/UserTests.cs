using System.ComponentModel;
using NewLife.GitCandy.Entity;
using NewLife.Data;
using Xunit;

namespace GitCandyXUnitTest.Entity;

/// <summary>用户业务逻辑测试</summary>
[Collection("GitCandyTests")]
public class UserTests
{
    public UserTests(DatabaseFixture fixture) { }

    [Fact]
    [DisplayName("Create_有效数据_成功创建用户")]
    public void Create_ValidData_CreatesUser()
    {
        var user = User.Create("testuser", "测试用户", "password123", "test@example.com", "测试描述");
        Assert.NotNull(user);
        Assert.True(user.ID > 0);
        Assert.Equal("testuser", user.Name);
        Assert.Equal("测试用户", user.NickName);
        Assert.Equal("test@example.com", user.Email);
        Assert.True(user.Enable);
    }

    [Fact]
    [DisplayName("Create_重复用户名_抛出异常")]
    public void Create_DuplicateName_ThrowsException()
    {
        User.Create("dupuser", "重复用户", "pwd", "dup@test.com", null);
        var ex = Assert.Throws<ArgumentException>(() =>
            User.Create("dupuser", "另一个", "pwd2", "other@test.com", null));
        Assert.Contains("名称", ex.Message);
    }

    [Fact]
    [DisplayName("Create_重复邮箱_抛出异常")]
    public void Create_DuplicateEmail_ThrowsException()
    {
        User.Create("user1", "用户1", "pwd", "same@test.com", null);
        var ex = Assert.Throws<ArgumentException>(() =>
            User.Create("user2", "用户2", "pwd", "same@test.com", null));
        Assert.Contains("邮件", ex.Message);
    }

    [Fact]
    [DisplayName("CreateTeam_有效数据_成功创建团队")]
    public void CreateTeam_ValidData_CreatesTeam()
    {
        var team = User.CreateTeam("devteam", "开发团队", "技术研发团队");
        Assert.NotNull(team);
        Assert.True(team.ID > 0);
        Assert.Equal("devteam", team.Name);
        Assert.True(team.IsTeam);
        Assert.False(team.Enable);
    }

    [Fact]
    [DisplayName("FindByName_存在用户_返回正确实体")]
    public void FindByName_ExistingUser_ReturnsCorrectUser()
    {
        User.Create("findbyname", "查找测试", "pwd", "find@test.com", null);
        var found = User.FindByName("findbyname");
        Assert.NotNull(found);
        Assert.Equal("findbyname", found.Name);
        Assert.Equal("find@test.com", found.Email);
    }

    [Fact]
    [DisplayName("FindByName_不存在用户_返回Null")]
    public void FindByName_NonExistingUser_ReturnsNull()
    {
        var found = User.FindByName("nonexistent_user_xyz");
        Assert.Null(found);
    }

    [Fact]
    [DisplayName("FindByEmail_存在用户_返回正确实体")]
    public void FindByEmail_ExistingUser_ReturnsCorrectUser()
    {
        User.Create("emailtest", "邮箱测试", "pwd", "unique@test.com", null);
        var found = User.FindByEmail("unique@test.com");
        Assert.NotNull(found);
        Assert.Equal("emailtest", found.Name);
    }

    [Fact]
    [DisplayName("SearchByName_关键词查询_返回匹配用户")]
    public void SearchByName_Keyword_ReturnsMatchingUsers()
    {
        User.Create("alice", "Alice", "pwd", "alice@test.com", null);
        User.Create("bob", "Bob", "pwd", "bob@test.com", null);
        User.Create("charlie", "Charlie", "pwd", "charlie@test.com", null);

        var param = new PageParameter { PageSize = 100 };
        var results = User.SearchByName("alice", param);
        Assert.Contains(results, u => u.Name == "alice");
        Assert.DoesNotContain(results, u => u.Name == "bob");
    }

    [Fact]
    [DisplayName("SearchUser_关键词搜索_正确过滤")]
    public void SearchUser_Keyword_FiltersCorrectly()
    {
        User.Create("searchuser1", "搜索1", "pwd", "s1@test.com", null);
        User.Create("searchuser2", "搜索2", "pwd", "s2@test.com", null);
        User.CreateTeam("searchteam", "搜索团队", null);

        var param = new PageParameter { PageSize = 100 };
        var results = User.SearchUser("searchuser1", param);
        Assert.Contains(results, u => u.Name == "searchuser1");
        Assert.DoesNotContain(results, u => u.Name == "searchteam");
    }

    [Fact]
    [DisplayName("Search_按团队标识过滤_正确筛选")]
    public void Search_ByIsTeam_FiltersCorrectly()
    {
        User.CreateTeam("searchteam2", "搜索团队2", null);

        var param = new PageParameter { PageSize = 100 };
        var teams = User.Search(0, null, true, DateTime.MinValue, DateTime.MinValue, "searchteam2", param);

        Assert.Contains(teams, u => u.Name == "searchteam2");
        Assert.All(teams, u => Assert.True(u.IsTeam));
    }

    [Fact]
    [DisplayName("Search_按拥有者过滤_返回团队成员")]
    public void Search_ByOwner_ReturnsTeamMembers()
    {
        var owner = User.Create("searchowner", "搜索拥有者", "pwd", "searchowner@test.com", null);
        var member = User.Create("searchmember", "搜索成员", "pwd", "searchmember@test.com", null);
        var team = User.CreateTeam("searchteam3", "搜索团队3", null);
        new UserTeam { UserID = member.ID, TeamID = team.ID }.Save();

        var param = new PageParameter { PageSize = 100 };
        var users = User.Search(team.ID, null, null, DateTime.MinValue, DateTime.MinValue, null, param);

        Assert.Contains(users, u => u.ID == member.ID);
        Assert.DoesNotContain(users, u => u.ID == owner.ID);
    }

    [Fact]
    [DisplayName("GetOrAdd_新用户_自动创建")]
    public void GetOrAdd_NewUser_CreatesAutomatically()
    {
        var created = User.GetOrAdd(999, "autocreated", "auto@test.com");
        Assert.NotNull(created);
        Assert.Equal("autocreated", created.Name);
        Assert.Equal(999, created.LinkID);
    }

    [Fact]
    [DisplayName("GetOrAdd_已有用户_返回现有记录")]
    public void GetOrAdd_ExistingUser_ReturnsExisting()
    {
        var first = User.GetOrAdd(1001, "existing_getoradd", "existing@test.com");
        var second = User.GetOrAdd(1001, "existing_getoradd", "existing@test.com");
        Assert.Equal(first.ID, second.ID);
    }
}
