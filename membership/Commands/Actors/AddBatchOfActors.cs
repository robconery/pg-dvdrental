using Membership.DataAccess;
using Membership.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Commands.Actors
{
  public class AddBatchOfActors
  {
    ICommandRunner Runner;
    public AddBatchOfActors(ICommandRunner runner)
    {
      Runner = runner;
    }
    
    public Actor[] Actors { get; set; }

    public int Execute()
    {
      var sql = "insert into actor(first_name, last_name, last_update) values (@0, @1, @2);";
      var commands = new List<NpgsqlCommand>();
      foreach (var actor in Actors)
      {
        commands.Add(Runner.BuildCommand(sql, actor.First, actor.Last, DateTime.Today));
      }

      var results = Runner.Transact(commands.ToArray());
      return results.Sum();
    }

  }
}
