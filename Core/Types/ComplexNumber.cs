using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
	public class ComplexNumber : MathValue, IEquatable<ComplexNumber>
	{
		private readonly RationalNumber	_real;
		private readonly RationalNumber	_imaginary;

		#region Constructors

		public ComplexNumber(RationalNumber real, RationalNumber imaginary)
		{
			_real = real;
			_imaginary = imaginary;
		}

		public ComplexNumber(RationalNumber real): this(real, RationalNumber.Zero) {}

		public ComplexNumber(string value)
		{
			value = value.Trim().Replace(" ", "").ToLower();
			decimal simpleNegative;

			if (checkIfSimpleNegativeComplexNumber(value, out simpleNegative))
			{
				_real = 0;
				_imaginary = new RationalNumber(simpleNegative);
				return;
			}

			if (value == "i")
			{
				_real = RationalNumber.Zero;
				_imaginary = RationalNumber.One;
				return;
			}

			if (value == "-i")
			{
				_real = RationalNumber.Zero;
				_imaginary = new RationalNumber(-1);
				return;
			}

			if (!value.Contains('i'))
			{
				_real = new RationalNumber(value);
				_imaginary = RationalNumber.Zero;
				return;
			}

			if (value.EndsWith('i') && !value.Contains('+') && value.IndexOf('-') == -1)
			{
				string imagPart = value.Substring(0, value.Length - 1);
				if (string.IsNullOrEmpty(imagPart)) imagPart = "1";

				_real = RationalNumber.Zero;
				_imaginary = new RationalNumber(imagPart);
				return;
			}

			int lastSignIndex = -1;
			for (int i = value.Length - 2; i > 0; i--)
			{
				if (value[i] == '+' || value[i] == '-')
				{
					lastSignIndex = i;
					break;
				}
			}

			if (lastSignIndex == -1)
				throw new ArgumentException($"Invalid complex number format: {value}");

			string realPart = value.Substring(0, lastSignIndex);
			string imaginaryPart = value.Substring(lastSignIndex).Replace("i", "");

			if (realPart.Contains("i"))
			{
				string tmp = realPart;
				realPart = imaginaryPart;
				imaginaryPart = tmp.Replace("i", "");
			}

			if (string.IsNullOrEmpty(imaginaryPart) || imaginaryPart == "+")
				imaginaryPart = "1";
			if (imaginaryPart == "-")
				imaginaryPart = "-1";

			_real = new RationalNumber(realPart);
			_imaginary = new RationalNumber(imaginaryPart);
		}

		#endregion

		#region Properties

		public RationalNumber Real => _real;
		public RationalNumber Imaginary => _imaginary;

		public override bool IsReal => _imaginary == RationalNumber.Zero;
		public override bool IsZero => _real == RationalNumber.Zero && _imaginary == RationalNumber.Zero;
		public bool IsImaginary => _real == RationalNumber.Zero;
		
		public double Magnitude
		{
			get
			{
				double realValue = (double)_real.ToDecimal();
				double imaginaryValue = (double)_imaginary.ToDecimal();
				return CustomMath.Sqrt(realValue * realValue + imaginaryValue * imaginaryValue);
			}
		}

		#endregion

		#region Static Factory Methods

		public static ComplexNumber Parse(string input)
		{
			return new ComplexNumber(input);
		}

		public static bool TryParse(string value, out ComplexNumber? result)
		{
			try
			{
				value = value.Trim().Replace(" ", "").ToLower();
				if (!value.Contains('i'))
				{
					result = null;
					return false;
				}
				
				result = new ComplexNumber(value);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		public static ComplexNumber Zero => new ComplexNumber(0, 0);

		public static ComplexNumber One => new ComplexNumber(1, 0);

		public static ComplexNumber I => new ComplexNumber(RationalNumber.Zero, RationalNumber.One);

		#endregion

		#region Comparison Operations

		public static bool operator ==(ComplexNumber? a, ComplexNumber? b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (a is null || b is null) return false;
			return a._real == b._real && a._imaginary == b._imaginary;
		}

		public static bool operator <(ComplexNumber a, ComplexNumber b)
		{
			return a.Magnitude < b.Magnitude;
		}

		public static bool operator !=(ComplexNumber? a, ComplexNumber? b)
		{
			return !(a == b);
		}

		public static bool operator >(ComplexNumber a, ComplexNumber b)
		{
			return a.Magnitude > b.Magnitude;
		}

		public static bool operator <=(ComplexNumber a, ComplexNumber b)
		{
			return a.Magnitude <= b.Magnitude;
		}

		public static bool operator >=(ComplexNumber a, ComplexNumber b)
		{
			return a.Magnitude >= b.Magnitude;
		}

		#endregion

		#region Arithmetic Operations

		public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
		{
			RationalNumber real = a._real + b._real;
			RationalNumber imaginary = a._imaginary + b._imaginary;
			return new ComplexNumber(real, imaginary);
		}

		public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
		{
			RationalNumber real = a._real - b._real;
			RationalNumber imaginary = a._imaginary - b._imaginary;
			return new ComplexNumber(real, imaginary);
		}

		public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
		{
			RationalNumber real = (a._real * b._real) - (a._imaginary * b._imaginary);
			RationalNumber imaginary = (a._imaginary * b._real) + (a._real * b._imaginary);
			return new ComplexNumber(real, imaginary);
		}

		public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
		{
			RationalNumber real = ((a._real * b._real) + (a._imaginary * b._imaginary)) / ((b._real * b._real) + (b._imaginary * b._imaginary));
			RationalNumber imaginary = ((a._imaginary * b._real) - (a._real * b._imaginary)) / ((b._real * b._real) + (b._imaginary * b._imaginary));
			return new ComplexNumber(real, imaginary);
		}

		public ComplexNumber Conjugate()
		{
			return new ComplexNumber(_real, -_imaginary);
		}

		public static ComplexNumber operator -(ComplexNumber a)
		{
			return new ComplexNumber(-a._real, -a._imaginary);
		}

		public ComplexNumber Reciprocal()
		{
			RationalNumber denominator = (_real * _real) + (_imaginary * _imaginary);
			if (denominator == RationalNumber.Zero)
				throw new DivideByZeroException("Cannot compute reciprocal of zero complex number");
				
			return new ComplexNumber(_real / denominator, -_imaginary / denominator);
		}

		public virtual ComplexNumber PowerComplex(int exponent)
		{
			if (exponent == 0) return new ComplexNumber(RationalNumber.One, RationalNumber.Zero);
			if (exponent == 1) return this;
			if (exponent < 0) return PowerComplex(-exponent).Reciprocal();
			
			ComplexNumber result = this;
			for (int i = 1; i < exponent; i++)
			{
				result *= this;
			}
			return result;
		}

		#endregion

		#region IEquatable interface implementations

		public int CompareTo(ComplexNumber? other)
		{
			if (other is null) return 1;
			if (other is not ComplexNumber otherComplex)
				throw new ArgumentException("Can only compare with other ComplexNumber instances");

			if (this < otherComplex) return -1;
			if (this > otherComplex) return 1;
			return 0;
		}

		public bool Equals(ComplexNumber? other)
		{
			if (other is not ComplexNumber otherComplex) return false;
			return this == otherComplex;
		}

		public override bool Equals(object? obj)
		{
			return obj is ComplexNumber other && Equals(other);
		}

		public override int GetHashCode()
        {
            return HashCode.Combine(_real, _imaginary);
        }

		public override string ToString()
		{
			if (IsReal) return _real.ToString();
			if (IsImaginary) 
			{
				if (_imaginary == RationalNumber.One) return "i";
				if (_imaginary == -RationalNumber.One) return "-i";
				return $"{_imaginary}i";
			}
			
			string imaginaryPart;
			if (_imaginary == RationalNumber.One)
				imaginaryPart = "+i";
			else if (_imaginary == -RationalNumber.One)
				imaginaryPart = "-i";
			else
				imaginaryPart = _imaginary >= RationalNumber.Zero ? 
					$"+{_imaginary}i" : $"-{(-_imaginary)}i";
			
			return $"{_real}{imaginaryPart}";
		}

		#endregion

		#region Type Conversions
		
		public static implicit operator ComplexNumber(RationalNumber real) 
			=> new ComplexNumber(real);
		public static implicit operator ComplexNumber(int value) 
			=> new ComplexNumber(new RationalNumber(value));
		
		#endregion

		#region Math Operations

		public override MathValue Add(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this + new ComplexNumber(r),
				ComplexNumber c => this + c,
				_ => throw new ArgumentException($"Cannot add {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Subtract(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this - new ComplexNumber(r),
				ComplexNumber c => this - c,
				_ => throw new ArgumentException($"Cannot subtract {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Multiply(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this * new ComplexNumber(r),
				ComplexNumber c => this * c,
				_ => throw new ArgumentException($"Cannot multiply {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Divide(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this / new ComplexNumber(r),
				ComplexNumber c => this / c,
				_ => throw new ArgumentException($"Cannot divide {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Modulo(MathValue other)
		{
			throw new ArgumentException("Modulo operation is not supported for complex numbers");
		}

		public override MathValue Power(int exponent) => PowerComplex(exponent);
		public override MathValue Negate() => -this;

		public override RationalNumber? AsRational() => IsReal ? _real : null;
		public ComplexNumber AsComplex() => this;

		#endregion

		#region Helpers

		public bool checkIfSimpleNegativeComplexNumber(string value, out decimal parsedNegative)
		{
			int		numbers = 0;

			foreach (char c in value)
			{
				if ("+*/^%".Contains(c))
				{
					parsedNegative = 0;
					return false;
				}
				else if (Char.IsNumber(c)) numbers++;
			}

			if (value.Count(x => x == '-') != 1 ||
				value.Count(x => x == 'i') != 1 ||
				numbers == 0)
			{
				parsedNegative = 0;
				return false;
			}

			parsedNegative = decimal.Parse(value.Replace("i", ""));
			return true;
		}

		#endregion
	}
}