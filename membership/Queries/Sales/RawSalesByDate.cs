using Membership.DataAccess;
using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Queries.Sales
{
  public class RawSalesByDate
  {
    ICommandRunner Runner;

    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Quarter { get; set; }
    public int? Day { get; set; }

    public RawSalesByDate(ICommandRunner runner)
    {
      this.Runner = runner;
    }

    public IEnumerable<RawSale> Execute()
    {
      var args = new List<object>();

      var sqlFormat = @"select distinct title, quarter, month, year, qyear as QuarterYear,
      sum(amount) over (partition by title{0}) as Total from raw_sales where 1=1 {1}";
      
      var partitionValue = "";
      var whereValue = "";
      
      if (this.Year.HasValue)
      {
        args.Add(this.Year.Value);
        partitionValue = ",year";
        whereValue = "and year=@0";
      }
      if (this.Quarter.HasValue)
      {
        if (!this.Year.HasValue)
          this.Year = DateTime.Today.Year;
        args.Add(this.Quarter.Value);
        partitionValue = ",year,quarter";
        whereValue = "and year = @0 and quarter =@1";
      }
      if (this.Month.HasValue)
      {
        if (!this.Year.HasValue)
          this.Year = DateTime.Today.Year;
        args.Add(this.Month.Value);
        partitionValue = ",year,month";
        whereValue = "and year = @0 and month =@1";
      }
      var sql = String.Format(sqlFormat, partitionValue, whereValue);
      return this.Runner.Execute<RawSale>(sql, args.ToArray());
    }
  }
}
