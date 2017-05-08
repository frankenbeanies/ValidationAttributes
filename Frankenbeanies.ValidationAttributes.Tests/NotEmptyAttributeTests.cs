using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Frankenbeanies.ValidationAttributes.Tests
{
    public class NotEmptyAttributeTests
    {
        NotEmptyAttribute GetSut()
        {
            return new NotEmptyAttribute();
        }

        [Fact]
        public void NotEmptyAttribute_IsValid_Null_ReturnsTrue()
        {
            Assert.True(GetSut().IsValid(null));
        }

        [Fact]
        public void NotEmptyAttribute_IsValid_NotICollection_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => GetSut().IsValid(new { }));
        }

        [Fact]
        public void NotEmptyAttribute_IsValid_Empty_ReturnsFalse()
        {
            Assert.False(GetSut().IsValid(new List<object> { }));
        }

        [Fact]
        public void NotEmptyAttribute_IsValid_NotEmpty_ReturnsTrue()
        {
            Assert.True(GetSut().IsValid(new List<object> { new { } }));
        }
    }
}
