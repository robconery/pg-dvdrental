using Membership.DataAccess;
using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Queries.Membership
{
  public class UserQuery
  {
    ICommandRunner Runner;
    public UserQuery(ICommandRunner runner)
    {
      Runner = runner;
    }
    public int? ID { get; set; }
    public string Email { get; set; }
    public string UserKey { get; set; }

    public User Execute()
    {
      var sqlFormat = @"select first, last, email, user_key as userkey,
                  current_signin_at as currentsigninat,
                  last_signin_at as lastsigninat,created_at as createdat, 
                  signin_count as signincount,ip,status 
                  from users where {0} = @0";
      var sql = "";
      object arg = null;

      if (this.ID.HasValue)
      {
        sql = String.Format(sqlFormat, "id");
        arg = this.ID;
      }
      else if (!String.IsNullOrWhiteSpace(Email))
      {
        sql = String.Format(sqlFormat, "email");
        arg = this.Email;

      }
      else if (!String.IsNullOrWhiteSpace(UserKey))
      {
        sql = String.Format(sqlFormat, "user_key");
        arg = this.UserKey;
      }
      else
      {
        throw new InvalidOperationException("You need to set the ID, Email or Key properties");
      }

      return Runner.ExecuteSingle<User>(sql, arg);
    }
  }
}
