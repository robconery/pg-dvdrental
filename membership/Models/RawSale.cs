using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Models
{
  public class RawSale
  {
    public string Title { get; set; }
    public decimal Total { get; set; }
    public int Quarter { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string QuarterYear { get; set; }
  }
}
