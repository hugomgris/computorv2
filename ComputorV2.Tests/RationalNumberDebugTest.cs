using Xunit;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class RationalNumberDebugTest
    {
        [Fact]
        public void TestDecimalConstructor()
        {
            var positive = new RationalNumber(4.3m);
            Assert.Equal("4.3", positive.ToDecimalString());
            
            var negative = new RationalNumber(-4.3m);
            Assert.Equal("-4.3", negative.ToDecimalString());
            
            decimal value = -4.3m;
            string valueStr = value.ToString("0.#############################");
            Assert.Equal("-4.3", valueStr);
            
            string[] parts = valueStr.Split('.');
            Assert.Equal(2, parts.Length);
            Assert.Equal("-4", parts[0]);
            Assert.Equal("3", parts[1]);
            
            long wholePart = long.Parse(parts[0]);
            string fractionalPart = parts[1];
            Assert.Equal(-4, wholePart);
            Assert.Equal("3", fractionalPart);
            
            long denominator = 10;
            long numerator = wholePart * denominator + long.Parse(fractionalPart);

            Assert.Equal(-37, numerator);
        }
        
        [Fact]
        public void TestCorrectNegativeDecimalParsing()
        {
            var rational = new RationalNumber(-43, 10);
            Assert.Equal("-4.3", rational.ToDecimalString());
        }
    }
}
