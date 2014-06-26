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
      var salesOrder = new SalesOrder();
      salesOrder.Items.Add(new LineItem { SKU = "Ducky", Price = 12.00M, Name = "Little Duck Buddy" });
      var cmd = new Commands.Sales.SaveSalesOrder(db).Execute(salesOrder);
      Console.WriteLine(cmd);
      Console.Read();
    }
  }
}
