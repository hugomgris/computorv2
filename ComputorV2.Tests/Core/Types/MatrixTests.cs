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
        public void Matrix_Determinant_3x3Matrix_ThrowsNotImplementedException()
        {
            var matrix = new Matrix("[[1,2,3];[4,5,6];[7,8,9]]");
            
            Assert.Throws<NotImplementedException>(() => matrix.Determinant());
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
    }
}