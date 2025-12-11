using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
	public class Matrix : MathValue, IEquatable<Matrix>
	{
		private readonly int _rows;
		private readonly int _cols;
		private readonly MathValue[,] _elements;

		#region Constructors

		public Matrix(int rows, int cols)
		{
			_rows = rows;
			_cols = cols;
			_elements = new MathValue[rows, cols];

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++)
					_elements[i, j] = RationalNumber.Zero;
		}

		public Matrix(MathValue[,] elements)
		{
			_rows = elements.GetLength(0);
			_cols = elements.GetLength(1);
			_elements = new MathValue[_rows, _cols];

			for (int i = 0; i < _rows; i++)
				for (int j = 0; j < _cols; j++)
					_elements[i, j] = elements[i, j];
		}

		public Matrix(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
        		throw new ArgumentException("Matrix string cannot be null or empty");

			value = value.Trim();

			if (value.StartsWith("[[") && value.EndsWith("]]"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			string[] rowStrings = value.Split(';');
			_rows = rowStrings.Length;
			
			string firstRow = rowStrings[0].Trim('[', ']');
			if (string.IsNullOrEmpty(firstRow))
			    throw new ArgumentException("Matrix cannot have empty rows");

			string[] firstRowElements = firstRow.Split(',');
			_cols = firstRowElements.Length;

			_elements = new MathValue[_rows, _cols];

			for (int i = 0; i < _rows; i++)
			{
				string rowData = rowStrings[i].Trim().Trim('[', ']');
				string[] elements = rowData.Split(',');

				if (elements.Length != _cols)
					throw new ArgumentException($"Row {i} has {elements.Length} elements, expected {_cols}. All rows must have the same number of elements.");

				for (int j = 0; j < _cols; j++)
				{
					string elementStr = elements[j].Trim();

					if (RationalNumber.TryParse(elementStr, out RationalNumber? rational) && rational is not null)
						_elements[i, j] = rational;
					else if (ComplexNumber.TryParse(elementStr, out ComplexNumber? complex) && complex is not null)
						_elements[i, j] = complex;
					else
						throw new ArgumentException($"Cannot parse matrix element '{elementStr}' at position ({i},{j})");
				}
			}
			
		}

		public static Matrix Identity(int size)
		{
			var matrix = new Matrix(size, size);
			for (int i = 0; i < size; i++)
				matrix[i, i] = RationalNumber.One;
			return matrix;
		}

		public Matrix(MathValue a00, MathValue a01, MathValue a10, MathValue a11)
		{
			_rows = 2;
			_cols = 2;
			_elements = new MathValue[2, 2];
			_elements[0, 0] = a00; _elements[0, 1] = a01;
			_elements[1, 0] = a10; _elements[1, 1] = a11;
		}

		#endregion

		#region Static Parse Methods

		public static bool TryParse(string input, out Matrix? matrix)
		{
			try
			{
				matrix = new Matrix(input);
				return true;
			}
			catch
			{
				matrix = null;
				return false;
			}
		}

		public static Matrix Parse(string input)
		{
			if (TryParse(input, out Matrix? matrix))
				return matrix!;

			throw new FormatException($"Unable to parse '{input}' as a matrix");
		}

		#endregion

		#region Properties

		public int Rows => _rows;
		public int Cols => _cols;
		public int Columns => _cols;
		public bool IsSquare => _rows == _cols;

		public MathValue this[int row, int col]
		{ 
			get => _elements[row, col];
			set => _elements[row, col] = value;
		}

		public override bool IsReal => true;
		public override bool IsZero => false;

		#endregion

		#region Arithmetic Operations
		public static Matrix operator +(Matrix left, Matrix right)
		{
			return left.Add(right);
		}

		public static Matrix operator -(Matrix left, Matrix right)
		{
			return left.Subtract(right);
		}

		public static Matrix operator *(Matrix left, Matrix right)
		{
			return left.Multiply(right);
		}

		public static Matrix operator /(Matrix left, Matrix right)
		{
			return left.Divide(right);
		}

		#endregion

		#region MathValue Implementation

		public override MathValue Add(MathValue other)
		{
			if (other is not Matrix otherMatrix)
        		throw new ArgumentException("Matrix addition requires another matrix");
    
    		return Add(otherMatrix);
		}

		public Matrix Add(Matrix other)
		{
			if (_rows != other._rows || _cols != other._cols)
				throw new ArgumentException($"Cannot add matrices of different dimensions: {_rows}x{_cols} and {other._rows}x{other._cols}");
			
			var result = new Matrix(_rows, _cols);
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					result[i, j] = _elements[i, j].Add(other[i, j]);
				}
			}
			return result;
		}
		
		public override MathValue Subtract(MathValue other)
		{
			if (other is not Matrix otherMatrix)
				throw new ArgumentException("Matrix subtraction requires another matrix");

			return Subtract(otherMatrix);
		}

		public Matrix Subtract(Matrix other)
		{
			if (_rows != other._rows || _cols != other._cols)
				throw new ArgumentException($"Cannot add matrices of different dimensions: {_rows}x{_cols} and {other._rows}x{other._cols}");
			
			var result = new Matrix(_rows, _cols);
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					result[i, j] = _elements[i, j].Subtract(other[i, j]);
				}
			}
			return result;
		}
		
		public override MathValue Multiply(MathValue other)
		{
			if (other is not Matrix otherMatrix)
				throw new ArgumentException("Matrix multiplication requires another matrix");

			return Multiply(otherMatrix);
		}

		public Matrix Multiply(Matrix other)
		{
			if (_cols != other._rows)
				throw new ArgumentException($"Cannot multiply matrices if the first one's rows are not equal to the second one's cols: {_rows}x{_cols} and {other._rows}x{other._cols}");

			var result = new Matrix(_rows, other._cols);
			for (int i = 0; i < result.Rows; i++)
			{
				for (int j = 0; j < result.Cols; j++)
				{
					MathValue sum = RationalNumber.Zero;
					for (int k = 0 ; k < _cols; k++)
					{
						var product = _elements[i,k].Multiply(other[k,j]);
						sum = sum.Add(product);
					}

					result[i,j] = sum;
				}
			}

			return result;
		}
		
		public override MathValue Divide(MathValue other)
		{
			if (other is not Matrix otherMatrix)
				throw new ArgumentException("Matrix division requires another matrix");

			return Divide(otherMatrix);
		}

		public Matrix Divide(Matrix other)
		{
			if (!other.IsSquare)
				throw new ArgumentException("Cannot divide by a non-square matrix (inverse doesn't exist)");
			
			if (_cols != other._rows)
				throw new ArgumentException($"Cannot divide matrices: left matrix has {_cols} columns but right matrix is {other._rows}×{other._cols}");
			
			var inverse = other.Inverse();
			return this.Multiply(inverse);
		}

		public override MathValue Power(int exponent)
		{
			if (!IsSquare)
				throw new ArgumentException("Matrix power is only defined for square matrices");
			
			if (exponent == 0)
				return Matrix.Identity(_rows);
			
			if (exponent == 1)
				return CreateCopy();
			
			if (exponent < 0)
			{
				Matrix inverse = this.Inverse();
				return inverse.Power(-exponent);
			}
			
			return MatrixPower(exponent);
		}

		private Matrix CreateCopy()
		{
			var result = new Matrix(_rows, _cols);
			for (int i = 0; i < _rows; i++)
				for (int j = 0; j < _cols; j++)
					result[i, j] = _elements[i, j];
			return result;
		}

		private Matrix MatrixPower(int exponent)
		{
			Matrix result = Matrix.Identity(_rows);
			Matrix baseMatrix = CreateCopy();
			
			while (exponent > 0)
			{
				if (exponent % 2 == 1)
				{
					result = result.Multiply(baseMatrix);
				}
				baseMatrix = baseMatrix.Multiply(baseMatrix);
				exponent /= 2;
			}
			
			return result;
		}

		public override MathValue Negate()
		{
			var result = new Matrix(_rows, _cols);
			for (int i = 0; i < _rows; i++)
				for (int j = 0; j < _cols; j++)
					result[i, j] = _elements[i, j].Negate();
			return result;
		}

		public Matrix Transpose()
		{
			var result = new Matrix(_cols, _rows);
			for (int i = 0; i < _rows; i++)
				for (int j = 0; j < _cols; j++)
					result[j, i] = _elements[i, j];
			return result;
		}

		public MathValue Determinant()
		{
			if (!IsSquare)
				throw new InvalidOperationException("Determinant is only defined for square matrices");
				
			if (_rows == 1) return _elements[0, 0];
			if (_rows == 2) 
				return _elements[0,0].Multiply(_elements[1,1])
					.Subtract(_elements[0,1].Multiply(_elements[1,0]));
			
			return CalculateDeterminantByExpansion();
		}

		private MathValue CalculateDeterminantByExpansion()
		{
			MathValue det = RationalNumber.Zero;
			for (int j = 0; j < _cols; j++)
			{
				var element = _elements[0, j];
				if (element.AsRational()?.IsZero != true)
				{
					var minor = GetMinor(0, j);
					var cofactor = minor.Determinant();
					if ((j % 2) == 1)
						cofactor = cofactor.Negate();
					det = det.Add(element.Multiply(cofactor));
				}
			}
			return det;
		}

		private Matrix GetMinor(int excludeRow, int excludeCol)
		{
			int newSize = _rows - 1;
			var minor = new Matrix(newSize, newSize);
			
			int targetRow = 0;
			for (int i = 0; i < _rows; i++)
			{
				if (i == excludeRow) continue;
				
				int targetCol = 0;
				for (int j = 0; j < _cols; j++)
				{
					if (j == excludeCol) continue;
					
					minor[targetRow, targetCol] = _elements[i, j];
					targetCol++;
				}
				targetRow++;
			}
			
			return minor;
		}

		public Matrix Inverse()
		{
			if (!IsSquare)
				throw new InvalidOperationException("Inverse is only defined for square matrices");
			
			var det = Determinant();
			if (det.AsRational()?.IsZero == true)
				throw new InvalidOperationException("Matrix is singular (non-invertible)");
			
			int n = _rows;
			
			var augmented = CreateAugmentedMatrix();
			
			GaussJordanElimination(augmented, n);
			
			return ExtractInverseFromAugmented(augmented, n);
		}
		
		private Matrix CreateAugmentedMatrix()
		{
			int n = _rows;
			var augmented = new Matrix(n, 2 * n);
			
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					augmented[i, j] = _elements[i, j];
				}
			}
			
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					augmented[i, j + n] = (i == j) ? RationalNumber.One : RationalNumber.Zero;
				}
			}
			
			return augmented;
		}
		
		private void GaussJordanElimination(Matrix augmented, int n)
		{
			for (int col = 0; col < n; col++)
			{
				int pivotRow = FindPivotRow(augmented, col, col, n);
				if (pivotRow == -1)
					throw new InvalidOperationException("Matrix is singular");
				
				if (pivotRow != col)
					SwapRows(augmented, col, pivotRow);
				
				MathValue pivot = augmented[col, col];
				for (int j = 0; j < augmented.Cols; j++)
				{
					augmented[col, j] = augmented[col, j].Divide(pivot);
				}
				
				for (int i = 0; i < n; i++)
				{
					if (i != col)
					{
						MathValue factor = augmented[i, col];
						
						bool isZero = false;
						var rationalFactor = factor.AsRational();
						var complexFactor = factor.AsComplex();
						
						if (rationalFactor != null)
						{
							isZero = rationalFactor.IsZero;
						}
						else if (complexFactor != null)
						{
							isZero = complexFactor.IsZero;
						}
						
						if (!isZero)
						{
							for (int j = 0; j < augmented.Cols; j++)
							{
								var subtraction = factor.Multiply(augmented[col, j]);
								augmented[i, j] = augmented[i, j].Subtract(subtraction);
							}
						}
					}
				}
			}
		}
		
		private int FindPivotRow(Matrix matrix, int startRow, int col, int n)
		{
			int bestRow = -1;
			double bestMagnitude = 0.0;
			
			for (int i = startRow; i < n; i++)
			{
				var element = matrix[i, col];
				double magnitude = 0.0;
				
				var rational = element.AsRational();
				var complex = element.AsComplex();
				
				if (rational != null && !rational.IsZero)
				{
					magnitude = CustomMath.Abs((double)rational.Numerator / (double)rational.Denominator);
				}
				else if (complex != null && !complex.IsZero)
				{
					var realPart = complex.Real.AsRational();
					var imagPart = complex.Imaginary.AsRational();
					if (realPart != null && imagPart != null)
					{
						double realVal = (double)realPart.Numerator / (double)realPart.Denominator;
						double imagVal = (double)imagPart.Numerator / (double)imagPart.Denominator;
						magnitude = CustomMath.Sqrt(realVal * realVal + imagVal * imagVal);
					}
				}
				
				if (magnitude > bestMagnitude)
				{
					bestMagnitude = magnitude;
					bestRow = i;
				}
			}
			
			return bestRow;
		}
		
		private void SwapRows(Matrix matrix, int row1, int row2)
		{
			for (int j = 0; j < matrix.Cols; j++)
			{
				var temp = matrix[row1, j];
				matrix[row1, j] = matrix[row2, j];
				matrix[row2, j] = temp;
			}
		}
		
		private Matrix ExtractInverseFromAugmented(Matrix augmented, int n)
		{
			var result = new Matrix(n, n);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					result[i, j] = augmented[i, j + n];
				}
			}
			return result;
		}

		#endregion

		#region String Representation
		
		public override string ToString()
		{
			if (_rows == 0 || _cols == 0) return "[]";
			
			var lines = new List<string>();
			for (int i = 0; i < _rows; i++)
			{
				var row = new List<string>();
				for (int j = 0; j < _cols; j++)
				{
					row.Add(_elements[i, j].ToString());
				}
				lines.Add("[ " + string.Join(" , ", row) + " ]");
			}
			return string.Join("\n", lines);
		}

		#endregion

		#region IEquatable inheritance implementations

		public override bool Equals(MathValue? other)
		{
			return other is Matrix matrix && Equals(matrix);
		}

		public bool Equals(Matrix? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			
			if (_rows != other._rows || _cols != other._cols) return false;
			
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					if (!ElementsAreEqual(_elements[i, j], other._elements[i, j]))
						return false;
				}
			}
			
			return true;
		}

		private static bool ElementsAreEqual(MathValue a, MathValue b)
		{
			if (a.Equals(b)) return true;
			
			var rationalA = a.AsRational();
			var rationalB = b.AsRational();
			
			if (rationalA != null && rationalB != null)
			{
				var leftNumerator = rationalA.Numerator * rationalB.Denominator;
				var rightNumerator = rationalB.Numerator * rationalA.Denominator;
				return leftNumerator == rightNumerator;
			}
			
			var complexA = a.AsComplex();
			var complexB = b.AsComplex();
			
			if (complexA != null && complexB != null)
			{
				return ElementsAreEqual(complexA.Real, complexB.Real) &&
				       ElementsAreEqual(complexA.Imaginary, complexB.Imaginary);
			}
			
			return false;
		}

		public override bool Equals(object? obj)
		{
			return obj is Matrix matrix && Equals(matrix);
		}

		public override int GetHashCode()
		{
			var hash = new HashCode();
			hash.Add(_rows);
			hash.Add(_cols);
			
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _cols; j++)
				{
					hash.Add(_elements[i, j]);
				}
			}
			
			return hash.ToHashCode();
		}

		#endregion
	}
}
