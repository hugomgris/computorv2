using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class EvaluationTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public EvaluationTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}

		#region Assignation Part

		[Fact]
		public void EvaluationTests_AssignationPart_BasicErrorTest_01()
		{
			Action act = () => _evaluator.Assign("x==2");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Parser: expression can only contain one '=' token: x==2 (Parameter 'input')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_BasicErrorTest_02()
		{
			Action act = () => _evaluator.Assign("x=23edd23-+-+");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Variable Substitution: expression contains undefined variables: 23edd23-+-+ (Parameter 'expression')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_SemiAdvanced_01()
		{
			Action act = () => _evaluator.Assign("==2");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Parser: expression can only contain one '=' token: ==2 (Parameter 'input')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_SemiAdvanced_02()
		{
			Action act = () => _evaluator.Assign("f(x = 2");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Assignation: variable name can only contain letters: f(x=2 (Parameter 'input')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_SemiAdvanced_03()
		{
			Action act = () => _evaluator.Assign("x=[[4,2]");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Parser: invalid type (check syntax!) (Parameter 'input')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_Advanced_01()
		{
			Action act = () => _evaluator.Assign("x = --2");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Tokenizer: invalid expression (Parameter 'rawTokens')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_Advanced_02()
		{
			Action act = () => _evaluator.Assign("i = 2");
			ArgumentException exception = Assert.Throws<ArgumentException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Assignation: variable name can not contain 'i' character: i=2 (Parameter 'input')", exception.Message);
		}

		[Fact]
		public void EvaluationTests_AssignationPart_BasicValidTest()
		{
			_evaluator.Assign("x = 2");
			_evaluator.Assign("y = 4i");
			_evaluator.Assign("z = [[2,3];[3,5]]");
			
			Dictionary<string, MathValue> variables = _evaluator.Variables;
			Assert.True(variables.ContainsKey("x"));
			Assert.True(variables.ContainsKey("y"));
			Assert.True(variables.ContainsKey("z"));

			Assert.Equal("2", variables["x"].ToString());
			Assert.Equal("4i", variables["y"].ToString());
			Assert.Equal("[[2,3];[3,5]]", variables["z"].ToString()!.Replace(" ", ""));
		}

		[Fact]
		public void EvaluationTests_AssignationPart_SemiAdvancedValidTest()
		{
			_evaluator.Assign("x = 2");
			_evaluator.Assign("y = x");
			
			Dictionary<string, MathValue> variables = _evaluator.Variables;
			Assert.True(variables.ContainsKey("x"));
			Assert.True(variables.ContainsKey("y"));

			Assert.Equal("2", variables["x"].ToString());
			Assert.Equal("2", variables["y"].ToString());

			_evaluator.Assign("x = 5");
			Assert.True(variables.ContainsKey("x"));
			Assert.Equal("5", variables["x"].ToString());

			_evaluator.Assign("x = -4+89i");
			_evaluator.Assign("y = x");
			
			variables = _evaluator.Variables;
			Assert.True(variables.ContainsKey("x"));
			Assert.True(variables.ContainsKey("y"));

			Assert.Equal("-4+89i", variables["x"].ToString());
			Assert.Equal("-4+89i", variables["y"].ToString());

			_evaluator.Assign("x = [[1,2];[3,4];[5,6]]");
			_evaluator.Assign("y = x");
			
			variables = _evaluator.Variables;
			Assert.True(variables.ContainsKey("x"));
			Assert.True(variables.ContainsKey("y"));

			Assert.Equal("[[1,2];[3,4];[5,6]]", variables["x"].ToString()!.Replace(" ", ""));
			Assert.Equal("[[1,2];[3,4];[5,6]]", variables["y"].ToString()!.Replace(" ", ""));
		}

		[Fact]
		public void EvaluationTests_AssignationPart_AdvancedValidTest()
		{
			_evaluator.Assign("x = 2");
			_evaluator.Assign("y = x * [[4,2]]");
			_evaluator.Assign("f(z) = z * y");

			Dictionary<string, MathValue> variables = _evaluator.Variables;
			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(variables.ContainsKey("x"));
			Assert.True(variables.ContainsKey("y"));
			Assert.True(functions.ContainsKey("f"));

			string result1 = _evaluator.Compute("f(z)=?");
			Assert.Equal("f(z)=[[8,4]]*z", result1.Replace(" ", ""));

			_evaluator.Assign("x = 2");
			_evaluator.Assign("f(x) = x * 5");
			string result2 = _evaluator.Compute("f(x)=?");
			Assert.Equal("10", result2.Replace(" ", ""));
		}

		#endregion

		#region Calculation Part

		[Fact]
		public void EvaluationTests_CalculationPart_BasicValidTest()
		{
			string r1 = _evaluator.Compute("2 + 2 = ?");
			string r2 = _evaluator.Compute("3 * 4 = ?");
			_evaluator.Assign("x = 2");
			string r3 = _evaluator.Compute("x + 2 = ?");
			string r4 = _evaluator.Compute("1.5 + 1 = ?");

			Assert.Equal("4", r1);
			Assert.Equal("12", r2);
			Assert.Equal("4", r3);
			Assert.Equal("5/2", r4);

			Action act = () => _evaluator.Compute("2 / 0 = ?");
			DivideByZeroException exception = Assert.Throws<DivideByZeroException>(act);
			Assert.NotNull(exception);
			Assert.Equal("Cannot divide by zero", exception.Message);
		}

		[Fact]
		public void EvaluationTests_CalculationPart_SemiAdvancedValidTest()
		{
			_evaluator.Assign("x = 2 * i");
			string r1 = _evaluator.Compute("x ^ 2 = ?");
			
			_evaluator.Assign("A = [[2,4];[3,4]]");
			_evaluator.Assign("B = [[1,0];[0,1]]");
			string r2 = _evaluator.Compute("A ** B = ?");

			_evaluator.Assign("f(x) = x + 2");
			_evaluator.Assign("p = 4");
			string r3 = _evaluator.Compute("f(p) = ?");

			Assert.Equal("-4", r1);
			Assert.Equal("[[2,4];[3,4]]", r2.Replace(" ", ""));
			Assert.Equal("6", r3);
		}

		[Fact]
		public void EvaluationTests_CalculationPart_AdvancedValidTest()
		{
			string r1 = _evaluator.Compute("4 - 3 - ( 2 * 3) ^ 2 * ( 2 - 4 ) + 4 = ?");

			_evaluator.Assign("f(x) = 2*(x + 3*(x - 4))");
			_evaluator.Assign("p = 2");
			string r2 = _evaluator.Compute("f(3)=?");
			string r3 = _evaluator.Compute("f(2)=?");
			string r4 = _evaluator.Compute("f(3) - f(p) + 2 = ?");

			Assert.Equal("77", r1);
			Assert.Equal("0", r2);
			Assert.Equal("-8", r3);
			Assert.Equal("10", r4);

			_evaluator.Assign("f(x) = 2 *x *i");
			string r5 = _evaluator.Compute("f(2) = ?");
			Assert.Equal("4i", r5);
		}

		#endregion
	}
}