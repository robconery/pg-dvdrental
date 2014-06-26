using Membership.DataAccess;
using Membership.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Commands.Sales
{
  public class SaveSalesOrder
  {

    ICommandRunner Runner;
    public SaveSalesOrder(ICommandRunner runner)
    {
      Runner = runner;
    }

    public int Execute(SalesOrder order)
    {
      //validate
      if (order == null)
      {
        throw new InvalidOperationException("Can't save a non-existent sales order");
      }
      
      //check if exists
      var existing = Runner.ExecuteSingle<SalesOrder>("select * from sales_order where id=@0", order.ID);
      var items = JsonConvert.SerializeObject(order.Items);
      NpgsqlCommand cmd;
      if (existing != null)
      {
        cmd = Runner.BuildCommand("UPDATE sales_order SET data=@0 WHERE id=@1", items, order.ID);
      }
      else
      {
        cmd = Runner.BuildCommand("INSERT INTO sales_order (data) VALUES(@0)", items);
      }
      //check inventory

      //reporting

      //log it to the user's account

      return Runner.Transact(cmd).Sum();
    }
  }
}
