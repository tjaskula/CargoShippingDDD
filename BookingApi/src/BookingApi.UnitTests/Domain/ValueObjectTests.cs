using System.Collections.Generic;
using BookingApi.Domain;
using Xunit;

namespace BookingApi.UnitTests.Domain
{
    public class ValueObjectTest
    {
        [Fact]
        public void EqualityOperator_BothNulls_TrueReturned()
        {
            FakeValueObject left = null;
            FakeValueObject right = null;
            Assert.True(left == right);
        }

        [Fact]
        public void EqualityOperator_LeftNull_FalseReturned()
        {
            FakeValueObject left = null;
            FakeValueObject right = new FakeValueObject("A");
            Assert.False(left == right);
            Assert.True(left != right);
        }

        [Fact]
        public void EqualityOperator_RightNull_FalseReturned()
        {
            FakeValueObject left = new FakeValueObject("A");
            FakeValueObject right = null;
            Assert.False(left == right);
            Assert.True(left != right);
        }

        [Fact]
        public void EqualityOperator_NotEqual_FalseReturned()
        {
            FakeValueObject left = new FakeValueObject("B");
            FakeValueObject right = new FakeValueObject("A");
            Assert.False(left == right);
            Assert.True(left != right);
        }

        [Fact]
        public void EqualityOperator_Equal_TrueReturned()
        {
            FakeValueObject left = new FakeValueObject("A");
            FakeValueObject right = new FakeValueObject("A");
            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void GetHashCode_SingleValue_ThisValueHashCodeReturned()
        {
            string singleValue = "abcd";
            FakeValueObject obj = new FakeValueObject(singleValue);

            Assert.Equal(singleValue.GetHashCode(), obj.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NullValue_ZeroReturned()
        {
            FakeValueObject obj = new FakeValueObject(new object[] { null });

            Assert.Equal(0, obj.GetHashCode());
        }

        [Fact]
        public void GetHashCode_TwoValues_XorOfHashCodesReturned()
        {
            string firstValue = "abcd";
            int secodValue = 15;
            FakeValueObject obj = new FakeValueObject(firstValue, secodValue);

            Assert.Equal((firstValue.GetHashCode() * 1) ^ (secodValue.GetHashCode() * 2), obj.GetHashCode());
        }

        private class FakeValueObject : ValueObject
        {
            private readonly List<object> _fakeValues;

            public FakeValueObject(params object[] fakeValues)
            {
                _fakeValues = new List<object>(fakeValues);
            }

            protected override IEnumerable<object> GetAtomicValues()
            {
                return _fakeValues;
            }

            public static bool operator ==(FakeValueObject left, FakeValueObject right)
            {
                return EqualOperator(left, right);
            }

            public static bool operator !=(FakeValueObject left, FakeValueObject right)
            {
                return NotEqualOperator(left, right);
            }
        }
    }
}