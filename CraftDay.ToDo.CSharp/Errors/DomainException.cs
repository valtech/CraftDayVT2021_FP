using System;
using Newtonsoft.Json;

namespace CraftDay.ToDo.CSharp.Errors
{
  public class DomainException : Exception
  {
    private readonly string _message;

    public DomainException(string message)
    {
      _message = message;
    }

    public override string ToString()
    {
      var errObj = new {type = "DomainError", error = _message};
      return JsonConvert.SerializeObject(errObj);
    }
  }
}