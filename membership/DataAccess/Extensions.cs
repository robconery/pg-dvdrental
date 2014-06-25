using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.DataAccess
{
  public static class Extensions
  {
    public static List<T> ToList<T>(this IDataReader rdr) where T : new()
    {
      var result = new List<T>();
      while (rdr.Read())
      {
        result.Add(rdr.ToSingle<T>());
      }
      return result;
    }

    public static T ToSingle<T>(this IDataReader rdr) where T : new()
    {

      var item = new T();
      var props = item.GetType().GetProperties();

      foreach (var prop in props)
      {
        for (int i = 0; i < rdr.FieldCount; i++)
        {
          if (rdr.GetName(i).Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase))
          {
            var val = rdr.GetValue(i);
            prop.SetValue(item, val);
          }
        }

      }

      return item;
    }
    /// <summary>
    /// Extension method for adding in a bunch of parameters
    /// </summary>
    public static void AddParams(this NpgsqlCommand cmd, params object[] args)
    {
      foreach (var item in args)
      {
        AddParam(cmd, item);
      }
    }
    /// <summary>
    /// Extension for adding single parameter
    /// </summary>
    public static void AddParam(this NpgsqlCommand cmd, object item)
    {
      var p = cmd.CreateParameter();
      p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
      if (item == null)
      {
        p.Value = DBNull.Value;
      }
      else
      {
        if (item.GetType() == typeof(Guid))
        {
          p.Value = item.ToString();
          p.DbType = DbType.String;
          p.Size = 4000;
        }
        else
        {
          p.Value = item;
        }
        if (item.GetType() == typeof(string))
          p.Size = ((string)item).Length > 4000 ? -1 : 4000;
      }
      cmd.Parameters.Add(p);
    }

    /// <summary>
    /// Turns an IDataReader to a Dynamic list of things
    /// </summary>
    public static List<dynamic> ToExpandoList(this IDataReader rdr)
    {
      var result = new List<dynamic>();
      while (rdr.Read())
      {
        result.Add(rdr.RecordToExpando());
      }
      return result;
    }
    public static dynamic RecordToExpando(this IDataReader rdr)
    {
      dynamic e = new ExpandoObject();
      var d = e as IDictionary<string, object>;
      for (int i = 0; i < rdr.FieldCount; i++)
        d.Add(rdr.GetName(i), DBNull.Value.Equals(rdr[i]) ? null : rdr[i]);
      return e;
    }
  }
}
