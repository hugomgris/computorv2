using Xunit;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests.Core.Types
{
	public class PolynomialTests
	{
		[Fact]
		public void Constructor_CreatesConstantPolynomial()
		{
			var constant = new RationalNumber(5);
			var poly = new Polynomial(constant);
			
			Assert.Equal("5", poly.ToString());
			Assert.Equal(0, poly.Degree);
		}

		[Fact]
		public void Constructor_CreatesLinearPolynomial()
		{
			var a = new RationalNumber(2);
			var b = new RationalNumber(3);
			var poly = new Polynomial(a, b); // 2x + 3
			
			Assert.Equal("2 * X + 3", poly.ToString());
			Assert.Equal(1, poly.Degree);
		}

		[Fact]
		public void Constructor_CreatesQuadraticPolynomial()
		{
			var a = new RationalNumber(1);
			var b = new RationalNumber(-2);
			var c = new RationalNumber(1);
			var poly = new Polynomial(a, b, c); // x^2 - 2x + 1
			
			Assert.Equal("X^2 - 2 * X + 1", poly.ToString());
			Assert.Equal(2, poly.Degree);
		}

		[Fact]
		public void Addition_WorksCorrectly()
		{
			var poly1 = new Polynomial(new RationalNumber(1), new RationalNumber(2)); // x + 2
			var poly2 = new Polynomial(new RationalNumber(1), new RationalNumber(3)); // x + 3
			
			var result = poly1.Add(poly2);
			
			Assert.Equal("2 * X + 5", result.ToString());
		}

		[Fact]
		public void Subtraction_WorksCorrectly()
		{
			var poly1 = new Polynomial(new RationalNumber(2), new RationalNumber(5)); // 2x + 5
			var poly2 = new Polynomial(new RationalNumber(1), new RationalNumber(3)); // x + 3
			
			var result = poly1.Subtract(poly2);
			
			Assert.Equal("X + 2", result.ToString());
		}

		[Fact]
		public void Multiplication_WorksCorrectly()
		{
			var poly1 = new Polynomial(new RationalNumber(1), new RationalNumber(1)); // x + 1
			var poly2 = new Polynomial(new RationalNumber(1), new RationalNumber(2)); // x + 2
			
			var result = poly1.Multiply(poly2);
			
			Assert.Equal("X^2 + 3 * X + 2", result.ToString());
		}

		[Fact]
		public void Derivative_WorksCorrectly()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(-2), new RationalNumber(1)); // x^2 - 2x + 1
			
			var derivative = poly.Derivative();
			
			Assert.Equal("2 * X - 2", derivative.ToString());
		}

		[Fact]
		public void Integration_WorksCorrectly()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(2)); // x + 2
			
			var integral = poly.Integrate();
			
			// Should be (1/2)x^2 + 2x + C (with C=0)
			Assert.Equal("1/2 * X^2 + 2 * X", integral.ToString());
		}

		[Fact]
		public void SolveDegree0_ReturnsEmptyForNonZero()
		{
			var poly = new Polynomial(new RationalNumber(5)); // 5 = 0
			
			var solutions = poly.Solve();
			
			Assert.Empty(solutions);
		}

		[Fact]
		public void SolveDegree0_ReturnsInfiniteForZero()
		{
			var poly = new Polynomial(new RationalNumber(0)); // 0 = 0
			
			var solutions = poly.Solve();
			
			Assert.Single(solutions);
			Assert.Equal("All real numbers", solutions[0].ToString());
		}

		[Fact]
		public void SolveDegree1_WorksCorrectly()
		{
			var poly = new Polynomial(new RationalNumber(2), new RationalNumber(-4)); // 2x - 4 = 0
			
			var solutions = poly.Solve();
			
			Assert.Single(solutions);
			Assert.Equal("2", solutions[0].ToString());
		}

		[Fact]
		public void SolveDegree2_WithPositiveDiscriminant_ReturnsTwoRealSolutions()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(0), new RationalNumber(-4)); // x^2 - 4 = 0
			
			var solutions = poly.Solve();
			
			Assert.Equal(2, solutions.Count);
			// Solutions should be x = 2 and x = -2
		}

		[Fact]
		public void SolveDegree2_WithZeroDiscriminant_ReturnsOneSolution()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(-2), new RationalNumber(1)); // x^2 - 2x + 1 = 0 = (x-1)^2
			
			var solutions = poly.Solve();
			
			Assert.Single(solutions);
			Assert.Equal("1", solutions[0].ToString());
		}

		[Fact]
		public void SolveDegree2_WithNegativeDiscriminant_ReturnsTwoComplexSolutions()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(0), new RationalNumber(1)); // x^2 + 1 = 0
			
			var solutions = poly.Solve();
			
			Assert.Equal(2, solutions.Count);
			// Solutions should be complex: x = i and x = -i
			Assert.True(solutions[0] is ComplexNumber);
			Assert.True(solutions[1] is ComplexNumber);
		}

		[Fact]
		public void IsZero_WorksCorrectly()
		{
			var zero = new Polynomial(new RationalNumber(0));
			var nonZero = new Polynomial(new RationalNumber(1));
			
			Assert.True(zero.IsZero);
			Assert.False(nonZero.IsZero);
		}

		[Fact]
		public void GetTerms_ReturnsCorrectDictionary()
		{
			var poly = new Polynomial(new RationalNumber(1), new RationalNumber(-2), new RationalNumber(3)); // x^2 - 2x + 3
			
			var terms = poly.GetTerms();
			
			Assert.Equal(3, terms.Count);
			Assert.Equal("3", terms[0].ToString()); // constant term
			Assert.Equal("-2", terms[1].ToString()); // x coefficient  
			Assert.Equal("1", terms[2].ToString()); // x^2 coefficient
		}
	}
}
