using Xunit;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class RationalNumberDebugTest
    {
        [Fact]
        public void TestDecimalConstructor()
        {
            // Test positive decimal
            var positive = new RationalNumber(4.3m);
            Assert.Equal("4.3", positive.ToDecimalString());
            
            // Test negative decimal
            var negative = new RationalNumber(-4.3m);
            Assert.Equal("-4.3", negative.ToDecimalString());
            
            // Debug the parsing 
            decimal value = -4.3m;
            string valueStr = value.ToString("0.#############################");
            Assert.Equal("-4.3", valueStr);
            
            string[] parts = valueStr.Split('.');
            Assert.Equal(2, parts.Length);
            Assert.Equal("-4", parts[0]);
            Assert.Equal("3", parts[1]);
            
            long wholePart = long.Parse(parts[0]);  // Should be -4
            string fractionalPart = parts[1];       // Should be "3"
            Assert.Equal(-4, wholePart);
            Assert.Equal("3", fractionalPart);
            
            long denominator = 10; // 10^1 for 1 decimal place
            long numerator = wholePart * denominator + long.Parse(fractionalPart);
            // numerator = -4 * 10 + 3 = -40 + 3 = -37  <-- This is the bug!
            
            // The issue: We should have -43, not -37
            Assert.Equal(-37, numerator); // This shows the bug
        }
        
        [Fact]
        public void TestCorrectNegativeDecimalParsing()
        {
            // The correct logic for -4.3:
            // numerator should be -43 and denominator should be 10
            var rational = new RationalNumber(-43, 10);
            Assert.Equal("-4.3", rational.ToDecimalString());
        }
    }
}
