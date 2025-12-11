// Add to ComputorV2.Tests/Core/Types/MatrixTests.cs
using Xunit;
using ComputorV2.Core.Types;
using System;

namespace ComputorV2.Tests.Core.Types
{
	public class MatrixTypeTests
	{
		#region Constructor Tests

		[Fact]
		public void Matrix_ZeroConstructor_CreatesCorrectMatrix()
		{
			var matrix = new Matrix(2, 3);
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(3, matrix.Cols);
			Assert.Equal(RationalNumber.Zero, matrix[0, 0]);
			Assert.Equal(RationalNumber.Zero, matrix[1, 2]);
		}

		[Fact]
		public void Matrix_ArrayConstructor_CreatesCorrectMatrix()
		{
			var elements = new MathValue[,] {
				{ new RationalNumber(1), new RationalNumber(2) },
				{ new RationalNumber(3), new RationalNumber(4) }
			};
			var matrix = new Matrix(elements);
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(2, matrix.Cols);
			Assert.Equal("1", matrix[0, 0].ToString());
			Assert.Equal("4", matrix[1, 1].ToString());
		}

		[Fact]
		public void Matrix_StringConstructor_2x2Matrix()
		{
			var matrix = new Matrix("[[1,2];[3,4]]");
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(2, matrix.Cols);
			Assert.Equal("1", matrix[0, 0].ToString());
			Assert.Equal("2", matrix[0, 1].ToString());
			Assert.Equal("3", matrix[1, 0].ToString());
			Assert.Equal("4", matrix[1, 1].ToString());
		}

		[Fact]
		public void Matrix_StringConstructor_3x3Matrix()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6];[7,8,9]]");
			
			Assert.Equal(3, matrix.Rows);
			Assert.Equal(3, matrix.Cols);
			Assert.Equal("5", matrix[1, 1].ToString());
			Assert.Equal("9", matrix[2, 2].ToString());
		}

		[Fact]
		public void Matrix_StringConstructor_2x3Matrix()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(3, matrix.Cols);
			Assert.Equal("6", matrix[1, 2].ToString());
		}

		[Fact]
		public void Matrix_StringConstructor_ComplexNumbers()
		{
			var matrix = new Matrix("[[1,i];[2+3i,4]]");
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(2, matrix.Cols);
			Assert.Equal("1", matrix[0, 0].ToString());
			Assert.Equal("i", matrix[0, 1].ToString());
		}

		[Fact]
		public void Matrix_2x2Constructor_CreatesCorrectMatrix()
		{
			var matrix = new Matrix(
				new RationalNumber(1), new RationalNumber(2),
				new RationalNumber(3), new RationalNumber(4)
			);
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(2, matrix.Cols);
			Assert.Equal("1", matrix[0, 0].ToString());
			Assert.Equal("4", matrix[1, 1].ToString());
		}

		[Fact]
		public void Matrix_Identity_CreatesIdentityMatrix()
		{
			var matrix = Matrix.Identity(3);
			
			Assert.Equal(3, matrix.Rows);
			Assert.Equal(3, matrix.Cols);
			Assert.True(matrix.IsSquare);
			Assert.Equal("1", matrix[0, 0].ToString());
			Assert.Equal("1", matrix[1, 1].ToString());
			Assert.Equal("1", matrix[2, 2].ToString());
			Assert.Equal("0", matrix[0, 1].ToString());
			Assert.Equal("0", matrix[1, 0].ToString());
		}

		[Fact]
		public void Matrix_DifferentFormats_ShouldWork()
		{
			// Row vector (1×2)
			var rowVector = new Matrix("[1,2]");
			Assert.Equal(1, rowVector.Rows);
			Assert.Equal(2, rowVector.Cols);
			
			// Column vector using semicolon (2×1) 
			var colVector = new Matrix("[1;2]");
			Assert.Equal(2, colVector.Rows);
			Assert.Equal(1, colVector.Cols);
			
			// Standard 2×2 matrix
			var matrix2x2 = new Matrix("[[1,2];[3,4]]");
			Assert.Equal(2, matrix2x2.Rows);
			Assert.Equal(2, matrix2x2.Cols);
			
			// Single element (1×1)
			var scalar = new Matrix("[5]");
			Assert.Equal(1, scalar.Rows);
			Assert.Equal(1, scalar.Cols);
		}

		#endregion

		#region Properties Tests

		[Fact]
		public void Matrix_Properties_ReturnCorrectValues()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Equal(2, matrix.Rows);
			Assert.Equal(3, matrix.Cols);
			Assert.Equal(3, matrix.Columns);
			Assert.False(matrix.IsSquare);
			Assert.True(matrix.IsReal);
			Assert.False(matrix.IsZero);
		}

		[Fact]
		public void Matrix_IsSquare_WorksCorrectly()
		{
			var square = new Matrix("[[1,2];[3,4]]");
			var rectangle = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.True(square.IsSquare);
			Assert.False(rectangle.IsSquare);
		}

		[Fact]
		public void Matrix_Indexer_WorksCorrectly()
		{
			var matrix = new Matrix(2, 2);
			matrix[0, 0] = new RationalNumber(42);
			matrix[1, 1] = new RationalNumber(24);
			
			Assert.Equal("42", matrix[0, 0].ToString());
			Assert.Equal("24", matrix[1, 1].ToString());
			Assert.Equal("0", matrix[0, 1].ToString());
		}

		#endregion

		#region Addition Tests

		[Fact]
		public void Matrix_Addition_2x2Matrices()
		{
			var m1 = new Matrix("[[1,2];[3,4]]");
			var m2 = new Matrix("[[5,6];[7,8]]");
			var result = m1 + m2;
			
			Assert.Equal("6", result[0, 0].ToString());
			Assert.Equal("8", result[0, 1].ToString());
			Assert.Equal("10", result[1, 0].ToString());
			Assert.Equal("12", result[1, 1].ToString());
		}

		[Fact]
		public void Matrix_Addition_3x3Matrices()
		{
			var m1 = new Matrix("[[1,2,3];[4,5,6];[7,8,9]]");
			var m2 = new Matrix("[[9,8,7];[6,5,4];[3,2,1]]");
			var result = m1 + m2;
			
			Assert.Equal("10", result[0, 0].ToString());
			Assert.Equal("10", result[1, 1].ToString());
			Assert.Equal("10", result[2, 2].ToString());
		}

		[Fact]
		public void Matrix_Addition_DifferentDimensions_ThrowsException()
		{
			var m1 = new Matrix("[[1,2];[3,4]]");
			var m2 = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Throws<ArgumentException>(() => m1 + m2);
		}

		#endregion

		#region Subtraction Tests

		[Fact]
		public void Matrix_Subtraction_2x2Matrices()
		{
			var m1 = new Matrix("[[5,6];[7,8]]");
			var m2 = new Matrix("[[1,2];[3,4]]");
			var result = m1 - m2;
			
			Assert.Equal("4", result[0, 0].ToString());
			Assert.Equal("4", result[0, 1].ToString());
			Assert.Equal("4", result[1, 0].ToString());
			Assert.Equal("4", result[1, 1].ToString());
		}

		[Fact]
		public void Matrix_Subtraction_DifferentDimensions_ThrowsException()
		{
			var m1 = new Matrix("[[1,2];[3,4]]");
			var m2 = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Throws<ArgumentException>(() => m1 - m2);
		}

		#endregion

		#region Multiplication Tests

		[Fact]
		public void Matrix_Multiplication_2x2Matrices()
		{
			var m1 = new Matrix("[[1,2];[3,4]]");
			var m2 = new Matrix("[[5,6];[7,8]]");
			var result = m1 * m2;
			
			Assert.Equal("19", result[0, 0].ToString());
			Assert.Equal("22", result[0, 1].ToString());
			Assert.Equal("43", result[1, 0].ToString());
			Assert.Equal("50", result[1, 1].ToString());
		}

		[Fact]
		public void Matrix_Multiplication_2x3And3x2()
		{
			var m1 = new Matrix("[[1,2,3];[4,5,6]]");
			var m2 = new Matrix("[[7,8];[9,10];[11,12]]");
			var result = m1 * m2;
			
			Assert.Equal(2, result.Rows);
			Assert.Equal(2, result.Cols);
			Assert.Equal("58", result[0, 0].ToString());
			Assert.Equal("64", result[0, 1].ToString());
		}

		[Fact]
		public void Matrix_Multiplication_IncompatibleDimensions_ThrowsException()
		{
			var m1 = new Matrix("[[1,2];[3,4]]");
			var m2 = new Matrix("[[1];[2];[3]]");
			
			Assert.Throws<ArgumentException>(() => m1 * m2);
		}

		[Fact]
		public void Matrix_Multiplication_IdentityMatrix()
		{
			var matrix = new Matrix("[[1,2];[3,4]]");
			var identity = Matrix.Identity(2);
			var result = matrix * identity;
			
			Assert.Equal("1", result[0, 0].ToString());
			Assert.Equal("2", result[0, 1].ToString());
			Assert.Equal("3", result[1, 0].ToString());
			Assert.Equal("4", result[1, 1].ToString());
		}

		#endregion

		#region Negate Tests

		[Fact]
		public void Matrix_Negate_ChangesSignOfAllElements()
		{
			var matrix = new Matrix("[[1,2];[-3,4]]");
			var result = (Matrix)matrix.Negate();
			
			Assert.Equal("-1", result[0,0].ToString());
			Assert.Equal("-2", result[0,1].ToString());
			Assert.Equal("3", result[1,0].ToString());
			Assert.Equal("-4", result[1,1].ToString());
		}

		#endregion

		#region Transpose Tests

		[Fact]
		public void Matrix_Transpose_2x3Matrix()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6]]");
			var result = matrix.Transpose();
			
			Assert.Equal(3, result.Rows);
			Assert.Equal(2, result.Cols);
			Assert.Equal("1", result[0, 0].ToString());
			Assert.Equal("4", result[0, 1].ToString());
			Assert.Equal("2", result[1, 0].ToString());
			Assert.Equal("5", result[1, 1].ToString());
			Assert.Equal("3", result[2, 0].ToString());
			Assert.Equal("6", result[2, 1].ToString());
		}

		[Fact]
		public void Matrix_Transpose_SquareMatrix()
		{
			var matrix = new Matrix("[[1,2];[3,4]]");
			var result = matrix.Transpose();
			
			Assert.Equal("1", result[0, 0].ToString());
			Assert.Equal("3", result[0, 1].ToString());
			Assert.Equal("2", result[1, 0].ToString());
			Assert.Equal("4", result[1, 1].ToString());
		}

		#endregion

		#region Determinant Tests

		[Fact]
		public void Matrix_Determinant_1x1Matrix()
		{
			var matrix = new Matrix("[[5]]");
			var det = matrix.Determinant();
			
			Assert.Equal("5", det.ToString());
		}

		[Fact]
		public void Matrix_Determinant_2x2Matrix()
		{
			var matrix = new Matrix("[[1,2];[3,4]]");
			var det = matrix.Determinant();
			
			Assert.Equal("-2", det.ToString());
		}

		[Fact]
		public void Matrix_Determinant_IdentityMatrix()
		{
			var identity = Matrix.Identity(2);
			var det = identity.Determinant();
			
			Assert.Equal("1", det.ToString());
		}

		[Fact]
		public void Matrix_Determinant_NonSquareMatrix_ThrowsException()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Throws<InvalidOperationException>(() => matrix.Determinant());
		}

		[Fact]
		public void Matrix_Determinant_3x3Matrix_ReturnsCorrectValue()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6];[7,8,9]]");
			var det = matrix.Determinant();
			
			// This matrix has determinant = 0 (singular)
			Assert.Equal("0", det.ToString());
		}
		
		[Fact]
		public void Matrix_Determinant_3x3Matrix_Invertible()
		{
			var matrix = new Matrix("[[1,0,2];[2,1,0];[1,1,1]]");
			var det = matrix.Determinant();
			
			// This matrix should have a non-zero determinant
			Assert.NotEqual("0", det.ToString());
		}

		#endregion

		#region ToString Tests

		[Fact]
		public void Matrix_ToString_FormatsCorrectly()
		{
			var matrix = new Matrix("[[1,2];[3,4]]");
			var str = matrix.ToString();
			
			Assert.Contains("1", str);
			Assert.Contains("2", str);
			Assert.Contains("3", str);
			Assert.Contains("4", str);
		}

		[Fact]
		public void Matrix_ToString_EmptyMatrix()
		{
			var matrix = new Matrix(0, 0);
			var str = matrix.ToString();
			
			Assert.Equal("[]", str);
		}

		#endregion

		#region Error Handling Tests

		[Fact]
		public void Matrix_StringConstructor_EmptyString_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => new Matrix(""));
			Assert.Throws<ArgumentException>(() => new Matrix("   "));
		}

		[Fact]
		public void Matrix_StringConstructor_InvalidFormat_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => new Matrix("[[1,2];[3,4,5]]"));
		}

		[Fact]
		public void Matrix_StringConstructor_UnparsableElement_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => new Matrix("[[1,abc];[3,4]]"));
		}

		#endregion

		#region Inverse Tests

		[Fact]
		public void Matrix_Inverse_2x2Matrix_ReturnsCorrectInverse()
		{
			var matrix = new Matrix("[[2,1];[1,2]]");
			var inverse = matrix.Inverse();
			
			Assert.Equal(2, inverse.Rows);
			Assert.Equal(2, inverse.Cols);

			Assert.Equal("2/3", inverse[0, 0].ToString());
			Assert.Equal("-1/3", inverse[0, 1].ToString());
			Assert.Equal("-1/3", inverse[1, 0].ToString());
			Assert.Equal("2/3", inverse[1, 1].ToString());
		}

		[Fact]
		public void Matrix_Inverse_IdentityMatrix_ReturnsIdentity()
		{
			var identity = Matrix.Identity(2);
			var inverse = identity.Inverse();

			Assert.Equal(identity, inverse);
		}

		[Fact]
		public void Matrix_Inverse_MultiplyByOriginal_GivesIdentity()
		{
			var matrix = new Matrix("[[2,1];[1,2]]");
			var inverse = matrix.Inverse();
			var result = matrix.Multiply(inverse);
			var identity = Matrix.Identity(2);
			
			Assert.Equal(identity, result);
		}

		[Fact]
		public void Matrix_Inverse_MultiplyByOriginalReversed_GivesIdentity()
		{
			var matrix = new Matrix("[[3,2];[1,1]]");
			var inverse = matrix.Inverse();
			var result = inverse.Multiply(matrix);
			var identity = Matrix.Identity(2);
			
			Assert.Equal(identity, result);
		}

		[Fact]
		public void Matrix_Inverse_2x2Matrix_ReturnsCorrectInverse_Alternative()
		{
			var matrix = new Matrix("[[1,0];[0,2]]");
			var inverse = matrix.Inverse();
			
			var result = matrix.Multiply(inverse);
			var identity = Matrix.Identity(2);
			
			Assert.Equal(identity, result);
		}

		[Fact]
		public void Matrix_Inverse_NonSquareMatrix_ThrowsException()
		{
			var matrix = new Matrix("[[1,2,3];[4,5,6]]");
			
			Assert.Throws<InvalidOperationException>(() => matrix.Inverse());
		}

		[Fact]
		public void Matrix_Inverse_SingularMatrix_ThrowsException()
		{
			var singularMatrix = new Matrix("[[1,2];[2,4]]");
			
			Assert.Throws<InvalidOperationException>(() => singularMatrix.Inverse());
		}

		[Fact]
		public void Matrix_Inverse_1x1Matrix_ReturnsCorrectInverse()
		{
			var matrix = new Matrix("[[5]]");
			var inverse = matrix.Inverse();
			
			Assert.Equal("1/5", inverse[0, 0].ToString());
		}

		[Fact]
		public void Matrix_Inverse_ComplexNumbers_WorksCorrectly()
		{
			var matrix = new Matrix("[[1,i];[0,1]]");
			var inverse = matrix.Inverse();
			
			var result = matrix.Multiply(inverse);
			var identity = Matrix.Identity(2);
			
			Assert.Equal(identity, result);
		}

		#endregion

	#region Division Tests

	[Fact]
	public void Matrix_Division_2x2ByInvertible2x2_ReturnsCorrectResult()
	{
		var matrixA = new Matrix("[[6,4];[3,2]]");
		var matrixB = new Matrix("[[2,1];[1,1]]");
		
		var result = matrixA / matrixB;
		
		var inverse = matrixB.Inverse();
		var expected = matrixA.Multiply(inverse);
		
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Matrix_Division_ByIdentityMatrix_ReturnsSameMatrix()
	{
		var matrix = new Matrix("[[1,2];[3,4]]");
		var identity = Matrix.Identity(2);
		var result = matrix / identity;
		
		Assert.Equal(matrix, result);
	}

	[Fact]
	public void Matrix_Division_DivideByItself_ReturnsIdentity()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var result = matrix / matrix;
		var identity = Matrix.Identity(2);
		
		Assert.Equal(identity, result);
	}

	[Fact]
	public void Matrix_Division_NonSquareDivisor_ThrowsException()
	{
		var matrixA = new Matrix("[[1,2];[3,4]]");
		var matrixB = new Matrix("[[1,2,3];[4,5,6]]");
		
		Assert.Throws<ArgumentException>(() => matrixA / matrixB);
	}

	[Fact]
	public void Matrix_Division_IncompatibleDimensions_ThrowsException()
	{
		var matrixA = new Matrix("[[1,2,3];[4,5,6]]");
		var matrixB = new Matrix("[[1,0];[0,1]]");
		
		Assert.Throws<ArgumentException>(() => matrixA / matrixB);
	}

	[Fact]
	public void Matrix_Division_SingularMatrix_ThrowsException()
	{
		var matrixA = new Matrix("[[1,2];[3,4]]");
		var singularMatrix = new Matrix("[[1,1];[2,2]]");
		
		Assert.Throws<InvalidOperationException>(() => matrixA / singularMatrix);
	}

	[Fact]
	public void Matrix_Division_ComplexNumbers_WorksCorrectly()
	{
		var matrixA = new Matrix("[[1,i];[0,2]]");
		var matrixB = new Matrix("[[1,0];[0,1]]");
		var result = matrixA / matrixB;
		
		Assert.Equal(matrixA, result);
	}

	[Fact]
	public void Matrix_Division_MethodAndOperator_GiveSameResult()
	{
		var matrixA = new Matrix("[[3,1];[2,1]]");
		var matrixB = new Matrix("[[1,1];[0,1]]");
		
		var resultOperator = matrixA / matrixB;
		var resultMethod = matrixA.Divide(matrixB);
		
		Assert.Equal(resultOperator, resultMethod);
	}

	#endregion

	#region Comprehensive Power Tests

	[Fact]
	public void Matrix_Power_Zero_ReturnsIdentityMatrix()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var result = (Matrix)matrix.Power(0);
		var identity = Matrix.Identity(2);
		
		Assert.Equal(identity, result);
	}

	[Fact]
	public void Matrix_Power_One_ReturnsSameMatrix()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var result = (Matrix)matrix.Power(1);
		
		Assert.Equal(matrix, result);
	}

	[Fact]
	public void Matrix_Power_Two_EqualsMatrixTimesItself()
	{
		var matrix = new Matrix("[[1,2];[3,4]]");
		var power2 = (Matrix)matrix.Power(2);
		var manual = matrix.Multiply(matrix);
		
		Assert.Equal(manual, power2);
	}

	[Fact]
	public void Matrix_Power_Three_ReturnsCorrectResult()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var power3 = (Matrix)matrix.Power(3);
		var power2 = (Matrix)matrix.Power(2);
		var expected = power2.Multiply(matrix);
		
		Assert.Equal(expected, power3);
	}

	[Fact]
	public void Matrix_Power_Large_UsesEfficientAlgorithm()
	{
		var matrix = new Matrix("[[1,1];[0,1]]");
		var result = (Matrix)matrix.Power(10);
		
		Assert.Equal("1", result[0, 0].ToString());
		Assert.Equal("10", result[0, 1].ToString());
		Assert.Equal("0", result[1, 0].ToString());
		Assert.Equal("1", result[1, 1].ToString());
	}

	[Fact]
	public void Matrix_Power_NegativeOne_ReturnsInverse()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var negPower1 = (Matrix)matrix.Power(-1);
		var inverse = matrix.Inverse();
		
		Assert.Equal(inverse, negPower1);
	}

	[Fact]
	public void Matrix_Power_NegativeTwo_ReturnsInverseSquared()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var negPower2 = (Matrix)matrix.Power(-2);
		var inverse = matrix.Inverse();
		var inverseSquared = (Matrix)inverse.Power(2);
		
		Assert.Equal(inverseSquared, negPower2);
	}

	[Fact]
	public void Matrix_Power_NegativeThree_ReturnsCorrectResult()
	{
		var matrix = new Matrix("[[2,1];[1,2]]");
		var negPower3 = (Matrix)matrix.Power(-3);
		var inverse = matrix.Inverse();
		var inverseCubed = (Matrix)inverse.Power(3);
		
		Assert.Equal(inverseCubed, negPower3);
	}

	[Fact]
	public void Matrix_Power_NegativeTimesPositive_EqualsIdentity()
	{
		var matrix = new Matrix("[[3,1];[1,2]]");
		var positive = (Matrix)matrix.Power(5);
		var negative = (Matrix)matrix.Power(-5);
		var result = positive.Multiply(negative);
		var identity = Matrix.Identity(2);
		
		Assert.Equal(identity, result);
	}

	[Fact]
	public void Matrix_Power_PositiveTimesNegative_EqualsIdentity()
	{
		var matrix = new Matrix("[[2,3];[1,2]]");
		var negative = (Matrix)matrix.Power(-4);
		var positive = (Matrix)matrix.Power(4);
		var result = negative.Multiply(positive);
		var identity = Matrix.Identity(2);
		
		Assert.Equal(identity, result);
	}

	[Fact]
	public void Matrix_Power_NonSquareMatrix_ThrowsException()
	{
		var matrix = new Matrix("[[1,2,3];[4,5,6]]");
		
		Assert.Throws<ArgumentException>(() => matrix.Power(2));
		Assert.Throws<ArgumentException>(() => matrix.Power(-1));
		Assert.Throws<ArgumentException>(() => matrix.Power(0));
	}

	[Fact]
	public void Matrix_Power_SingularMatrixNegativePower_ThrowsException()
	{
		var singular = new Matrix("[[1,1];[1,1]]");
		
		Assert.Throws<InvalidOperationException>(() => singular.Power(-1));
		Assert.Throws<InvalidOperationException>(() => singular.Power(-2));
	}

	[Fact]
	public void Matrix_Power_SingularMatrixPositivePower_Works()
	{
		var singular = new Matrix("[[1,1];[1,1]]");
		var result = (Matrix)singular.Power(2);
		
		Assert.Equal("2", result[0, 0].ToString());
		Assert.Equal("2", result[0, 1].ToString());
		Assert.Equal("2", result[1, 0].ToString());
		Assert.Equal("2", result[1, 1].ToString());
	}

	[Fact]
	public void Matrix_Power_IdentityMatrix_AlwaysIdentity()
	{
		var identity = Matrix.Identity(2);
		
		Assert.Equal(identity, (Matrix)identity.Power(0));
		Assert.Equal(identity, (Matrix)identity.Power(1));
		Assert.Equal(identity, (Matrix)identity.Power(5));
		Assert.Equal(identity, (Matrix)identity.Power(-1));
		Assert.Equal(identity, (Matrix)identity.Power(-10));
	}

	[Fact]
	public void Matrix_Power_PowerLaws_Work()
	{
		var matrix = new Matrix("[[2,0];[0,3]]");
		var power2 = (Matrix)matrix.Power(2);
		var power3OfPower2 = (Matrix)power2.Power(3);
		var power6 = (Matrix)matrix.Power(6);
		
		Assert.Equal(power6, power3OfPower2);
	}

	#endregion
	}
}