using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class DebugNegativeDecimalTest
    {
        [Fact]
        public void DebugNegativeDecimalParsing()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize("-4.3");
            
            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Count);
            Assert.Equal("-", tokens[0]);
            Assert.Equal("4.3", tokens[1]);

            var positiveRational = new RationalNumber(43, 10);
            Assert.Equal("4.3", positiveRational.ToDecimalString());
            
            var negativeRational = new RationalNumber(-43, 10);
            Assert.Equal("-4.3", negativeRational.ToDecimalString());
            
            var evaluator = new MathEvaluator();
            var result = evaluator.Evaluate("-4.3");
            
            Assert.IsType<RationalNumber>(result);
            var resultRational = (RationalNumber)result;
            Assert.Equal("-4.3", resultRational.ToDecimalString());
        }
    }
}
