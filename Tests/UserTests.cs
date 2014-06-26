using Membership.DataAccess;
using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
  public class UserTests
  {
    ICommandRunner db;
    User user;
    public UserTests()
    {
      Helpers.Loader.ReloadDb();
      db = new CommandRunner("dvds");
      var userQuery = new Membership.Queries.Membership.UserQuery(db) { Email = "test@test.com" };
      user = userQuery.Execute();
    }

    [Fact]
    public void UserShouldExist()
    {
      Assert.NotNull(user);
    }
    [Fact]
    public void UserHasKey()
    {
      Assert.NotNull(user.UserKey);
    }

  }
}
