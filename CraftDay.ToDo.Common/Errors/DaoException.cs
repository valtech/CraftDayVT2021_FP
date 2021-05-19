using System;
using Newtonsoft.Json;

namespace CraftDay.ToDo.CSharp.Errors
{
  public class DaoException : Exception
  {
    private readonly string _message;

    public DaoException(string message)
    {
      _message = message;
    }

    public override string ToString()
    {
      var errObj = new {type = "DaoError", error = _message};
      return JsonConvert.SerializeObject(errObj);
    }
  }
}