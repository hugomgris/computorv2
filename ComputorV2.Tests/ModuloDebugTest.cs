using System;
using System.Collections.Generic;
using Xunit;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;

namespace ComputorV2.Tests
{
    public class ModuloDebugTest
    {
        [Fact]
        public void TestModuloTokenization()
        {
            var tokenizer = new Tokenizer();
            
            try
            {
                var tokens = tokenizer.Tokenize("4 % 2");
                Assert.Equal(3, tokens.Count);
                Assert.Equal("4", tokens[0]);
                Assert.Equal("%", tokens[1]);
                Assert.Equal("2", tokens[2]);
            }
            catch (Exception ex)
            {
                throw new Exception($"Tokenization failed: {ex.GetType().Name} - {ex.Message}", ex);
            }
        }
        
        [Fact]
        public void TestSimpleModuloEvaluation()
        {
            var evaluator = new MathEvaluator();
            
            try
            {
                var simpleResult = evaluator.Evaluate("4 + 2");
                Assert.NotNull(simpleResult);
                
                var result = evaluator.Evaluate("4 % 2");
                Assert.NotNull(result);
                
                if (result is ComputorV2.Core.Types.RationalNumber rational)
                {
                    Assert.Equal(0, rational.Numerator);
                }
                else
                {
                    throw new Exception($"Expected RationalNumber, got {result.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Evaluation failed: {ex.GetType().Name} - {ex.Message}\nStack trace: {ex.StackTrace}", ex);
            }
        }
    }
}
