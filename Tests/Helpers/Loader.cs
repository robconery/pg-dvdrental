using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
  public class Loader
  {
    static string LocateSQLDir()
    {
      var sqlDir = "";
      var currentDir = Directory.GetCurrentDirectory();
      if (currentDir.EndsWith("Debug") || currentDir.EndsWith("Release"))
      {
        var projectRoot = Directory.GetParent(@"..\..\").FullName;
        sqlDir = Path.Combine(projectRoot, "Scripts");
      }
      return sqlDir;
    }

    static string ReadOrCreate(string filePath)
    {
      if (!File.Exists(filePath))
      {
        File.Create(filePath);
        return "";
      }
      else
      {
        return File.ReadAllText(filePath);
      }
    }

    static List<string> ReadSQL()
    {

      var result = new List<string>();
      var sqlDir = LocateSQLDir();
      //there should be 3 files - Schema, Functions, Data
      var schemaPath = Path.Combine(sqlDir, "schema.sql");
      var functionPath = Path.Combine(sqlDir, "functions.sql");
      var dataPath = Path.Combine(sqlDir, "data.sql");

      result.Add(ReadOrCreate(schemaPath));
      result.Add(ReadOrCreate(functionPath));
      result.Add(ReadOrCreate(dataPath));

      return result;

    }

    public static bool ReloadDb()
    {
      var sqls = ReadSQL();
      var db = new Membership.DataAccess.CommandRunner("dvds");
      var commands = new List<NpgsqlCommand>();
      foreach (var sql in sqls)
      {
        //build a command
        commands.Add(db.BuildCommand(sql));
      }

      //run it
      var results = db.Transact(commands.ToArray());
      return results.Sum() > 0;
    }

  }
}
