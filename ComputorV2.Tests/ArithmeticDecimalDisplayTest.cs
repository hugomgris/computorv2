using Xunit;
using ComputorV2.Interactive;

namespace ComputorV2.Tests
{
	public class ArithmeticDecimalDisplayTest
	{
		[Fact]
		public void TestArithmeticWithDecimalNumbers()
		{
			var repl = new REPL();
			
			var assignOutput = repl.ProcessCommand("varB = 4.242");
			Assert.Equal("Variable 'varB' assigned: 4.242", assignOutput);
			
			var multiplyOutput = repl.ProcessCommand("varB * 2");
			Assert.Equal("8.484", multiplyOutput);
			
			var addOutput = repl.ProcessCommand("varB + 1.5");
			Assert.Equal("5.742", addOutput);
			
			var negativeAssign = repl.ProcessCommand("varC = -4.3");
			Assert.Equal("Variable 'varC' assigned: -4.3", negativeAssign);
			
			var negativeMultiply = repl.ProcessCommand("varC * 3");
			Assert.Equal("-12.9", negativeMultiply);
		}
		
		[Fact]
		public void TestDirectDecimalArithmetic()
		{
			var repl = new REPL();
			
			var result1 = repl.ProcessCommand("4.242 * 2");
			Assert.Equal("8.484", result1);
			
			var result2 = repl.ProcessCommand("3.14 + 2.86");
			Assert.Equal("6", result2);
			
			var result3 = repl.ProcessCommand("-4.3 * 2");
			Assert.Equal("-8.6", result3);
		}
	}
}
