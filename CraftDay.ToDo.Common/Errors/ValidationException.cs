using System;
using Newtonsoft.Json;

namespace CraftDay.ToDo.CSharp.Errors
{
  public class ValidationException : Exception
  {
    private readonly string _message;

    public ValidationException(string message)
    {
      _message = message;
    }

    public override string ToString()
    {
      var errObj = new {type = "ValidationError", error = _message};
      return JsonConvert.SerializeObject(errObj);
    }
  }
}