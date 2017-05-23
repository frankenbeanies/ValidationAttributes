using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankenbeanies.ValidationAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
  public class StringMapsToEnumAttribute : ValidationAttribute
  {
    private Type _enumType;

    public StringMapsToEnumAttribute(Type enumType)
    {
      _enumType = enumType;
    }

#if SILVERLIGHT
        internal
#else
    public
#endif
    override bool IsValid(object value)
    {
      if (value == null) return true;

      if (!(value is string)) throw new InvalidOperationException("StringMapsToEnumAttribute requires a string.");

      return Enum.IsDefined(_enumType, value);
    }
  }
}
