using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Frankenbeanies.ValidationAttributes.Tests
{
  public class StringMapsToEnumAttributeTests
  {
    private enum TestEnum
    {
      TestCases,
      Are,
      Very,
      Much,
      Fun
    }

    private StringMapsToEnumAttribute GetSut()
    {
      return new StringMapsToEnumAttribute(typeof(TestEnum));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_Null_ReturnsFalse()
    {
      Assert.True(GetSut().IsValid(null));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_Not_String_Throws()
    {
      Assert.Throws<InvalidOperationException>(() => GetSut().IsValid(1));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_PascalCase_InEnum_Returns_True()
    {
      Assert.True(GetSut().IsValid("TestCases"));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_UpperCase_InEnum_Returns_False()
    {
      Assert.False(GetSut().IsValid("TESTCASES"));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_CamelCase_InEnum_Returns_False()
    {
      Assert.False(GetSut().IsValid("testCases"));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_LowerCase_InEnum_Returns_False()
    {
      Assert.False(GetSut().IsValid("testcases"));
    }
    
    [Fact]
    public void StringMapsToEnumAttribute_IsValid_NotFirst_InEnum_Returns_False()
    {
      Assert.False(GetSut().IsValid("much"));
    }

    [Fact]
    public void StringMapsToEnumAttribute_IsValid_Not_InEnum_Returns_False()
    {
      Assert.False(GetSut().IsValid("is"));
    }
  }
}
