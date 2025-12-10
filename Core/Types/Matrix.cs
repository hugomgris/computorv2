using System;

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

		#region Interface Implementations (Temporary)

		// IEquatable<Matrix> implementation
		public bool Equals(Matrix? other) => throw new NotImplementedException();

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

		/* public static Matrix operator /(Matrix left, Matrix right)
		{
			return left.Divide(right);
		} */

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
		
		public override MathValue Divide(MathValue other) => throw new NotImplementedException();

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
			
			throw new NotImplementedException("Determinant for matrices larger than 2x2 not implemented");
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

		#region MathValue Abstract Implementations (Temporary)

		public override MathValue Power(int exponent) => throw new NotImplementedException();
		public override bool Equals(MathValue? other) => throw new NotImplementedException();
		public override bool Equals(object? obj) => throw new NotImplementedException();
		public override int GetHashCode() => throw new NotImplementedException();

		#endregion
	}
}
