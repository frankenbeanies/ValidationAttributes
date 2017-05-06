using System;
using System.Collections.Generic;
using Xunit;

namespace Frankenbeanies.ValidationAttributes.Tests
{
    public class ListRangeAttributeTests
    {
        #region Suts
        ListRangeAttribute GetSut(int min, int max)
        {
            return new ListRangeAttribute(min, max);
        }

        ListRangeAttribute GetSut(double min, double max)
        {
            return new ListRangeAttribute(min, max);
        }

        ListRangeAttribute GetSut(string min, string max)
        {
            return new ListRangeAttribute(typeof(decimal), min, max);
        }
        #endregion

        #region IntListRangeAttribute
        [Fact]
        public void ListRangeAttribute_Integer_IsValid_MinGreaterThanMax_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => GetSut(500, 0).IsValid(new List<int>()));
        }

        [Fact]
        public void ListRangeAttribute_Integer_IsValid_EmptyList_ReturnsTrue()
        {
            Assert.True(GetSut(0, 500).IsValid(new List<int>()));
        }

        [Fact]
        public void ListRangeAttribute_Integer_IsValid_ValidList_ReturnsTrue()
        {
            var sut = GetSut(0, 500);
            var list = new List<int>() { 0, 250, 500 };

            Assert.True(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Integer_IsValid_InvalidList_UnderBounds_ReturnsFalse()
        {
            var sut = GetSut(0, 500);
            var list = new List<int>() { -1, 250, 500 };

            Assert.False(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Integer_IsValid_InvalidList_OverBounds_ReturnsFalse()
        {
            var sut = GetSut(0, 500);
            var list = new List<int>() { 0, 250, 501 };

            Assert.False(sut.IsValid(list));
        }
        #endregion

        #region DoubleListRangeAttribute
        [Fact]
        public void ListRangeAttribute_Double_IsValid_MinGreaterThanMax_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => GetSut(500d, 0d).IsValid(new List<double>()));
        }

        [Fact]
        public void ListRangeAttribute_Double_IsValid_EmptyList_ReturnsTrue()
        {
            Assert.True(GetSut(0d, 500d).IsValid(new List<double>()));
        }

        [Fact]
        public void ListRangeAttribute_Double_IsValid_ValidList_ReturnsTrue()
        {
            var sut = GetSut(0d, 500d);
            var list = new List<double>() { 0d, 250d, 500d };

            Assert.True(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Double_IsValid_InvalidList_UnderBounds_ReturnsFalse()
        {
            var sut = GetSut(0d, 500d);
            var list = new List<double>() { -1d, 250d, 500d };

            Assert.False(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Double_IsValid_InvalidList_OverBounds_ReturnsFalse()
        {
            var sut = GetSut(0d, 500d);
            var list = new List<double>() { 0d, 250d, 501d };

            Assert.False(sut.IsValid(list));
        }
        #endregion

        #region DecimalListRangeAttribute
        [Fact]
        public void ListRangeAttribute_Decimal_IsValid_MinGreaterThanMax_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => GetSut("500", "0").IsValid(new List<decimal>()));
        }

        [Fact]
        public void ListRangeAttribute_Decimal_IsValid_EmptyList_ReturnsTrue()
        {
            Assert.True(GetSut("0", "500").IsValid(new List<decimal>()));
        }

        [Fact]
        public void ListRangeAttribute_Decimal_IsValid_ValidList_ReturnsTrue()
        {
            var sut = GetSut("0", "500");
            var list = new List<decimal>() { 0m, 250m, 500m };

            Assert.True(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Decimal_IsValid_InvalidList_UnderBounds_ReturnsFalse()
        {
            var sut = GetSut("0", "500");
            var list = new List<decimal>() { -1m, 250m, 500m };

            Assert.False(sut.IsValid(list));
        }

        [Fact]
        public void ListRangeAttribute_Decimal_IsValid_InvalidList_OverBounds_ReturnsFalse()
        {
            var sut = GetSut("0", "500");
            var list = new List<decimal>() { 0m, 250m, 501m };

            Assert.False(sut.IsValid(list));
        }
        #endregion
    }
}
