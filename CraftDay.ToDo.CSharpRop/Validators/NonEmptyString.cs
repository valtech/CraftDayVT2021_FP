using System;
using CraftDay.ToDo.CSharp.Errors;
using LanguageExt;
using LanguageExt.Common;

namespace CraftDay.ToDo.CSharpRop.Validators
{
  public class NonEmptyString
  {
    private readonly string _str;
    
    private NonEmptyString(string str)
    {
      _str = str;
    }

    public string Value() => _str;

    public static Either<Exception, NonEmptyString> From(string str)
    {
      if (!string.IsNullOrEmpty(str))
      {
        return new NonEmptyString(str);
      }
      return new ValidationException("String cannot be empty");
    }
  }
}