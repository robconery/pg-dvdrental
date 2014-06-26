using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Models
{

  public class LineItem
  {
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public LineItem()
    {
      this.Price = 0;
      this.Quantity = 1;
    }
  }
  public class SalesOrder
  {
    public int ID { get; set; }
    public List<LineItem> Items { get; set; }
    public SalesOrder()
    {
      this.Items = new List<LineItem>();
    }
  }
}
