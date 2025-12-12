using Xunit;
using ComputorV2.Interactive;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class ComplexNumberTypeTests
	{
		[Fact] public void Evaluate_ComplexNumberType_BasicConstructorWithRealAndImaginaryNonZero()
		{
			ComplexNumber c = new ComplexNumber(1, 2);

			Assert.Equal(c.Real, 1);
			Assert.Equal(c.Imaginary, 2);
		}

		[Fact] public void Evaluate_ComplexNumberType_ComplexNumberNoImaginaryIsReal()
		{
			ComplexNumber c = new ComplexNumber(1, 0);

			Assert.True(c.IsReal);
		}

		[Fact] public void Evaluate_ComplexNumberType_ComplexNumberNoRealIsImaginary()
		{
			ComplexNumber c = new ComplexNumber(0, 1);

			Assert.True(c.IsImaginary);
		}

		[Fact] public void Evaluate_ComplexNumberType_StringConstructor()
		{
			ComplexNumber c1 = new ComplexNumber("5 + 6i");
			ComplexNumber c2 = new ComplexNumber("-3i");

			Assert.Equal(c1.Real, 5);
			Assert.Equal(c1.Imaginary, 6);

			Assert.True(c2.IsImaginary);
			Assert.Equal(RationalNumber.Zero, c2.Real);
			Assert.Equal(-3, c2.Imaginary);
		}

		[Fact] public void Evaluate_ComplexNumberType_JustRealConstructor()
		{
			ComplexNumber c = new ComplexNumber(10);

			Assert.Equal(c.Real, 10);
			Assert.Equal(c.Imaginary, 0);
		}

		[Fact] public void Evaluate_ComplexNumberType_EqualNumbersCheck()
		{
			ComplexNumber c1 = new ComplexNumber(10);
			ComplexNumber c2 = new ComplexNumber("10");

			Assert.True(c1 == c2);
		}


		[Fact] public void Evaluate_ComplexNumberType_EqualMagnitudesCheck()
		{
			ComplexNumber[] c = {
				new ComplexNumber(3, 4),
				new ComplexNumber(5, 0),
				new ComplexNumber(0, 5),
				new ComplexNumber(-3, 4),
				new ComplexNumber(4, -3),
				new ComplexNumber(-5, 0),
				new ComplexNumber(-3, -4),
				new ComplexNumber("1/2")
			};

			Assert.True(c[0].Magnitude == c[1].Magnitude);
			Assert.True(c[2].Magnitude == c[3].Magnitude);
			Assert.True(c[0].Magnitude == c[4].Magnitude);
			Assert.True(c[5].Magnitude == c[6].Magnitude);

			Assert.Equal(0.5, c[7].Magnitude);
		}

		[Fact] public void Evaluate_ComplexNumberType_ComparisonsCheck()
		{
			ComplexNumber[] c = {
				new ComplexNumber("1"),
				new ComplexNumber("2"),
				new ComplexNumber("3i"),
				new ComplexNumber("4"),
				new ComplexNumber("1 + i")
			};

			Assert.True(c[0] < c[1]);
			Assert.True(c[2] < c[3]);
			Assert.True(c[4] > c[0]);
		}

		[Fact] public void Evaluate_ComplexNumberType_ArithmeticOperations()
		{
			ComplexNumber[] c = {
				new ComplexNumber("1 + i"),
				new ComplexNumber(2, 2),
				new ComplexNumber(2, 5),
				new ComplexNumber(4, -3),
				new ComplexNumber(2, 4),
				new ComplexNumber("3 + i"),
				new ComplexNumber(3, 4)
			};

			Assert.Equal(c[0] + c[1], new ComplexNumber(3, 3));
			Assert.Equal(c[1] - c[0], new ComplexNumber(1, 1));
			Assert.Equal(c[2] * c[3], new ComplexNumber(23, 14));
			Assert.Equal(c[4] / c[5], new ComplexNumber(1, 1));
			Assert.Equal(-c[4], new ComplexNumber(-2, -4));
			Assert.Equal(c[6].Power(2), new ComplexNumber(-7, 24));
			Assert.Equal(c[6].Power(3), new ComplexNumber(-117, 44));
		}
	}
}