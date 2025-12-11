using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
	public class RationalNumber : MathValue, IRationalNumber, IEquatable<RationalNumber>
	{
		private readonly long _numerator;
		private readonly long _denominator;

		#region Constructors

		public RationalNumber(long numerator, long denominator = 1)
		{
			if (denominator == 0)
				throw new ArgumentException("Denominator cannot be zero", nameof(denominator));

			if (denominator < 0)
			{
				numerator = -numerator;
				denominator = -denominator;
			}

			long gcd = GreatestCommonDivisor(CustomMath.Abs(numerator), denominator);
			_numerator = numerator / gcd;
			_denominator = denominator / gcd;
		}

		public RationalNumber(decimal value)
		{
			if (value == 0)
			{
				_numerator = 0;
				_denominator = 1;
				return;
			}

			string valueStr = value.ToString("0.#############################");
			
			if (!valueStr.Contains('.'))
			{
				_numerator = (long)value;
				_denominator = 1;
				return;
			}

			string[] parts = valueStr.Split('.');
			long wholePart = string.IsNullOrEmpty(parts[0]) ? 0 : long.Parse(parts[0]);
			string fractionalPart = parts[1];
			
			long denominator = 1;
			for (int i = 0; i < fractionalPart.Length; i++)
			{
				denominator *= 10;
			}
			
			long numerator = wholePart * denominator + long.Parse(fractionalPart);
			
			if (value < 0 && wholePart == 0)
			{
				numerator = -numerator;
			}

			long gcd = GreatestCommonDivisor(CustomMath.Abs(numerator), denominator);
			_numerator = numerator / gcd;
			_denominator = denominator / gcd;
		}

		#endregion

		#region Properties

		public long Numerator => _numerator;
		public long Denominator => _denominator;
		
		public bool IsInteger => _denominator == 1;
		public override bool IsZero => _numerator == 0;

		#endregion

		#region Static Factory Methods

		public static RationalNumber Zero => new RationalNumber(0, 1);

		public static RationalNumber One => new RationalNumber(1, 1);

		public static RationalNumber Parse(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				throw new ArgumentException("Input string cannot be null or empty", nameof(str));

			str = str.Trim();

			if (str.Contains('/'))
			{
				string[] parts = str.Split('/');
				if (parts.Length != 2)
					throw new ArgumentException($"Invalid fraction format: {str}", nameof(str));

				long numerator = long.Parse(parts[0].Trim());

				string denomStr = parts[1].Trim();
				if (denomStr.Contains('.'))
				{
					var denominatorRational = new RationalNumber(decimal.Parse(denomStr));
					return new RationalNumber(numerator) / denominatorRational;
				}
				else
				{
					long denominator = long.Parse(denomStr);
					return new RationalNumber(numerator, denominator);
				}
			}

			if (str.Contains('.'))
			{
				decimal value = decimal.Parse(str);
				return new RationalNumber(value);
			}

			long intValue = long.Parse(str);
			return new RationalNumber(intValue);
		}

		public static bool TryParse(string str, out RationalNumber? result)
		{
			result = null;
			try
			{
				result = Parse(str);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region Arithmetic Operations

		public static RationalNumber operator +(RationalNumber a, RationalNumber b)
		{
			long numerator = a._numerator * b._denominator + b._numerator * a._denominator;
			long denominator = a._denominator * b._denominator;
			return new RationalNumber(numerator, denominator);
		}

		public static RationalNumber operator -(RationalNumber a, RationalNumber b)
		{
			long numerator = a._numerator * b._denominator - b._numerator * a._denominator;
			long denominator = a._denominator * b._denominator;
			return new RationalNumber(numerator, denominator);
		}

		public static RationalNumber operator *(RationalNumber a, RationalNumber b)
		{
			long numerator = a._numerator * b._numerator;
			long denominator = a._denominator * b._denominator;
			return new RationalNumber(numerator, denominator);
		}

		public static RationalNumber operator /(RationalNumber a, RationalNumber b)
		{
			if (b.IsZero)
				throw new DivideByZeroException("Cannot divide by zero");
			
			long numerator = a._numerator * b._denominator;
			long denominator = a._denominator * b._numerator;
			return new RationalNumber(numerator, denominator);
		}

		public static RationalNumber operator -(RationalNumber a)
		{
			return new RationalNumber(-a._numerator, a._denominator);
		}

		public override MathValue Power(int exponent)
		{
			if (exponent == 0)
				return One;
			
			if (exponent < 0)
			{
				if (IsZero)
					throw new DivideByZeroException("Cannot raise zero to negative power");
				
				return new RationalNumber(_denominator, _numerator).Power(-exponent);
			}

			long numerator = 1;
			long denominator = 1;

			for (int i = 0; i < exponent; i++)
			{
				numerator *= _numerator;
				denominator *= _denominator;
			}

			return new RationalNumber(numerator, denominator);
		}

		#endregion

		#region Comparison Operations

		public static bool operator ==(RationalNumber? a, RationalNumber? b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (a is null || b is null) return false;
			return a._numerator == b._numerator && a._denominator == b._denominator;
		}

		public static bool operator !=(RationalNumber? a, RationalNumber? b)
		{
			return !(a == b);
		}

		public static bool operator <(RationalNumber a, RationalNumber b)
		{
			return a._numerator * b._denominator < b._numerator * a._denominator;
		}

		public static bool operator >(RationalNumber a, RationalNumber b)
		{
			return a._numerator * b._denominator > b._numerator * a._denominator;
		}

		public static bool operator <=(RationalNumber a, RationalNumber b)
		{
			return a._numerator * b._denominator <= b._numerator * a._denominator;
		}

		public static bool operator >=(RationalNumber a, RationalNumber b)
		{
			return a._numerator * b._denominator >= b._numerator * a._denominator;
		}

		#endregion

		#region Interface Implementation

		public decimal ToDecimal()
		{
			return (decimal)_numerator / (decimal)_denominator;
		}

		public IRationalNumber Simplify()
		{
			return this;
		}

		public IRationalNumber Abs()
		{
			return new RationalNumber(CustomMath.Abs(_numerator), _denominator);
		}

		public int CompareTo(IRationalNumber? other)
		{
			if (other is null) return 1;
			if (other is not RationalNumber otherRational) 
				throw new ArgumentException("Can only compare with other RationalNumber instances");

			if (this < otherRational) return -1;
			if (this > otherRational) return 1;
			return 0;
		}

		public bool Equals(IRationalNumber? other)
		{
			if (other is not RationalNumber otherRational) return false;
			return this == otherRational;
		}

		public bool Equals(RationalNumber? other)
		{
			if (other is null) return false;
			return this == other;
		}

		public override bool Equals(object? obj)
		{
			return obj is RationalNumber other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_numerator, _denominator);
		}

		#endregion

		#region String Representation

		public override string ToString()
		{
			if (_denominator == 1)
				return _numerator.ToString();
			
			return $"{_numerator}/{_denominator}";
		}

		public string ToDecimalString()
		{
			return ToDecimal().ToString("0.##########");
		}

		#endregion

		#region Helper Methods

		private static long GreatestCommonDivisor(long a, long b)
		{
			while (b != 0)
			{
				long temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		private static long LeastCommonMultiple(long a, long b)
		{
			return (a * b) / GreatestCommonDivisor(a, b);
		}

		#endregion

		#region Implicit Conversions

		public static implicit operator RationalNumber(int value)
		{
			return new RationalNumber(value);
		}

		public static implicit operator RationalNumber(long value)
		{
			return new RationalNumber(value);
		}

		public static explicit operator RationalNumber(decimal value)
		{
			return new RationalNumber(value);
		}

		public static explicit operator decimal(RationalNumber rational)
		{
			return rational.ToDecimal();
		}

		#endregion

		#region MathValue Implementation

		public override MathValue Add(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this + r,
				ComplexNumber c => new ComplexNumber(this) + c,
				_ => throw new ArgumentException($"Cannot add {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Subtract(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this - r,
				ComplexNumber c => new ComplexNumber(this) - c,
				_ => throw new ArgumentException($"Cannot subtract {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Multiply(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this * r,
				ComplexNumber c => new ComplexNumber(this) * c,
				_ => throw new ArgumentException($"Cannot multiply {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Divide(MathValue other)
		{
			return other switch
			{
				RationalNumber r => this / r,
				ComplexNumber c => new ComplexNumber(this) / c,
				_ => throw new ArgumentException($"Cannot divide {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Negate() => -this;

		public override bool IsReal => true;
		public override bool IsRational => true;

		public override RationalNumber AsRational() => this;
		public override ComplexNumber AsComplex() => new ComplexNumber(this);

		public override bool Equals(MathValue? other) => other is RationalNumber r && this.Equals(r);

		#endregion
	}
}
