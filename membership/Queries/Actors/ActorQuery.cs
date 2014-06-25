using Membership.DataAccess;
using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Queries.Actors
{
  public class ActorQuery
  {
    ICommandRunner Runner;
    public int ActorId { get; set; }
    public ActorQuery(ICommandRunner runner)
    {
      this.Runner = runner;
    }

    public Actor Execute()
    {
      var sql = @"select first_name as first, last_name as last, actor_id as ID 
                  from actor where actor_id=@0;";

      var filmSql = @"select film.film_id as ID, title, description, 
                      release_year as year from film 
                      inner join film_actor on film_actor.film_id = film.film_id
                      where film_actor.actor_id = @0;";
      
      var result = new Actor();
      using (var rdr = Runner.OpenReader(sql + filmSql, this.ActorId))
      {
        if (rdr.Read())
        {
          result = rdr.ToSingle<Actor>();
          rdr.NextResult();
          result.Films = rdr.ToList<Film>();
        }
        rdr.Dispose();

      }
      return result;
    }
  }
}
