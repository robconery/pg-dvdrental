using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Models
{
  public class Actor
  {
    public Actor()
    {
      this.Films = new List<Film>();
    }
    public int ID { get; set; }
    public string First { get; set; }
    public string Last { get; set; }
    public string Fullname
    {
      get
      {
        return String.Format("{0}, {1}", this.Last, this.First);
      }
    }

    public ICollection<Film> Films { get; set; }
  }
}
