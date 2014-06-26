using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Models
{
  public class User
  {
    public long ID { get; set; }
    public string Email { get; set; }
    public string First { get; set; }
    public string Last { get; set; }
    public string UserKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public int SignInCount { get; set; }
    public IPAddress IP { get; set; }
    public string Status { get; set; }
    public DateTime CurrentSignInAt { get; set; }
    public DateTime LastSignInAt { get; set; }
  }
}
