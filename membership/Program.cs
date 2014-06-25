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
      var q = new Commands.Actors.AddBatchOfActors(db);
      var actor1 = new Actor { First = "Joe", Last = "Tonks" };
      var actor2 = new Actor { First = "Joe", Last = "Biff" };
      var actor3 = new Actor { First = "Jolene", Last = "Silidkdk" };
      q.Actors = new Actor[]{actor1, actor2, actor3};
      var result = q.Execute();
      Console.WriteLine("There were {0} actors added", result);
      Console.Read();
    }
  }
}
