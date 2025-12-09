using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests;

public class InvalidCharacterTests
{
	[Fact]
	public void Evaluate_InvalidCharacterInExpression_ThrowsMeaningfulError()
	{
		
		var evaluator = new MathEvaluator();
		
		var exception = Assert.Throws<ArgumentException>(() => evaluator.Evaluate("@ + @"));
		Assert.Equal("Invalid character '@' in expression. Variable names must start with a letter and contain only letters and digits.", exception.Message);
	}

	[Fact]
	public void Evaluate_InvalidCharacterInComplexExpression_ThrowsMeaningfulError()
	{
		
		var evaluator = new MathEvaluator();
		
		var exception = Assert.Throws<ArgumentException>(() => evaluator.Evaluate("2 + $ - 3"));
		Assert.Equal("Invalid character '$' in expression. Variable names must start with a letter and contain only letters and digits.", exception.Message);
	}

	[Fact]
	public void Evaluate_InvalidCharacterWithValidVariables_ThrowsMeaningfulError()
	{
		
		var evaluator = new MathEvaluator();
		evaluator.Evaluate("a = 5");
		
		var exception = Assert.Throws<ArgumentException>(() => evaluator.Evaluate("a + #"));
		Assert.Equal("Invalid character '#' in expression. Variable names must start with a letter and contain only letters and digits.", exception.Message);
	}

	[Fact]
	public void Evaluate_ValidExpression_StillWorks()
	{
		
		var evaluator = new MathEvaluator();
		evaluator.Evaluate("x = 10");
		evaluator.Evaluate("y = 5");
		
		
		var result = evaluator.Evaluate("x + y");
		
		
		Assert.Equal("15", result.ToString());
	}
}
