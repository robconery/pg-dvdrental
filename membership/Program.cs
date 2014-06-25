using Membership.DataAccess;
using Membership.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership
{


  class Program
  {
    static void Main(string[] args)
    {

      var db = new CommandRunner("dvds");
      var q = new Queries.Actors.ActorQuery(db);
      q.ActorId = 12;
      var actor = q.Execute();

      Console.WriteLine("it's {0}", actor.Fullname);
      foreach (var f in actor.Films)
      {
        Console.WriteLine(f.Title);
      }
      Console.Read();
    }
  }
}
