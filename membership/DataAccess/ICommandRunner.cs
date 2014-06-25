using System;
namespace Membership.DataAccess
{
  public interface ICommandRunner
  {
    Npgsql.NpgsqlCommand BuildCommand(string sql, params object[] args);
    string ConnectionString { get; set; }
    System.Collections.Generic.IEnumerable<T> Execute<T>(string sql, params object[] args) where T : new();
    System.Collections.Generic.IEnumerable<dynamic> ExecuteDynamic(string sql, params object[] args);
    T ExecuteSingle<T>(string sql, params object[] args) where T : new();
    dynamic ExecuteSingleDynamic(string sql, params object[] args);
    Npgsql.NpgsqlDataReader OpenReader(string sql, params object[] args);
    System.Collections.Generic.List<int> Transact(params Npgsql.NpgsqlCommand[] cmds);
  }
}
