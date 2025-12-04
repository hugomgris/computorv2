using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
    /// <summary>
    /// Represents a rational number (fraction) with arbitrary precision
    /// Automatically simplifies fractions to lowest terms
    /// </summary>
    public class RationalNumber : IRationalNumber
    {
        private readonly long _numerator;
        private readonly long _denominator;

        #region Constructors

        /// <summary>
        /// Creates a rational number from numerator and denominator
        /// </summary>
        /// <param name="numerator">The numerator</param>
        /// <param name="denominator">The denominator (must not be zero)</param>
        public RationalNumber(long numerator, long denominator = 1)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator cannot be zero", nameof(denominator));

            // Handle negative denominators by moving sign to numerator
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            // Simplify the fraction
            long gcd = GreatestCommonDivisor(CustomMath.Abs(numerator), denominator);
            _numerator = numerator / gcd;
            _denominator = denominator / gcd;
        }

        /// <summary>
        /// Creates a rational number from a decimal value
        /// </summary>
        /// <param name="value">The decimal value to convert</param>
        public RationalNumber(decimal value)
        {
            if (value == 0)
            {
                _numerator = 0;
                _denominator = 1;
                return;
            }

            // Convert decimal to fraction
            // Handle the decimal places by finding appropriate denominator
            string valueStr = value.ToString("0.#############################");
            
            if (!valueStr.Contains('.'))
            {
                // It's a whole number
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
            
            // Handle negative values
            if (value < 0 && wholePart == 0)
            {
                numerator = -numerator;
            }

            // Simplify
            long gcd = GreatestCommonDivisor(CustomMath.Abs(numerator), denominator);
            _numerator = numerator / gcd;
            _denominator = denominator / gcd;
        }

        #endregion

        #region Properties

        public long Numerator => _numerator;
        public long Denominator => _denominator;
        
        public bool IsInteger => _denominator == 1;
        public bool IsZero => _numerator == 0;

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Creates a RationalNumber representing zero
        /// </summary>
        public static RationalNumber Zero => new RationalNumber(0, 1);

        /// <summary>
        /// Creates a RationalNumber representing one
        /// </summary>
        public static RationalNumber One => new RationalNumber(1, 1);

        /// <summary>
        /// Parses a string representation of a rational number
        /// Supports formats: "3", "3/4", "-7/2", "2.5"
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>Parsed RationalNumber</returns>
        public static RationalNumber Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Input string cannot be null or empty", nameof(str));

            str = str.Trim();

            // Check if it contains a fraction
            if (str.Contains('/'))
            {
                string[] parts = str.Split('/');
                if (parts.Length != 2)
                    throw new ArgumentException($"Invalid fraction format: {str}", nameof(str));

                long numerator = long.Parse(parts[0].Trim());
                long denominator = long.Parse(parts[1].Trim());
                return new RationalNumber(numerator, denominator);
            }

            // Check if it's a decimal
            if (str.Contains('.'))
            {
                decimal value = decimal.Parse(str);
                return new RationalNumber(value);
            }

            // It's a whole number
            long intValue = long.Parse(str);
            return new RationalNumber(intValue);
        }

        /// <summary>
        /// Tries to parse a string representation of a rational number
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="result">Parsed RationalNumber if successful</param>
        /// <returns>True if parsing succeeded</returns>
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

        /// <summary>
        /// Raises the rational number to an integer power
        /// </summary>
        public RationalNumber Power(int exponent)
        {
            if (exponent == 0)
                return One;
            
            if (exponent < 0)
            {
                if (IsZero)
                    throw new DivideByZeroException("Cannot raise zero to negative power");
                
                // For negative exponent, flip the fraction and use positive exponent
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
            // Already simplified in constructor
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

        /// <summary>
        /// Returns a decimal representation for display purposes
        /// </summary>
        public string ToDecimalString()
        {
            return ToDecimal().ToString("0.##########");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the Greatest Common Divisor using Euclidean algorithm
        /// </summary>
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

        /// <summary>
        /// Calculates the Least Common Multiple
        /// </summary>
        private static long LeastCommonMultiple(long a, long b)
        {
            return (a * b) / GreatestCommonDivisor(a, b);
        }

        #endregion

        #region Implicit Conversions

        /// <summary>
        /// Implicit conversion from integer to RationalNumber
        /// </summary>
        public static implicit operator RationalNumber(int value)
        {
            return new RationalNumber(value);
        }

        /// <summary>
        /// Implicit conversion from long to RationalNumber
        /// </summary>
        public static implicit operator RationalNumber(long value)
        {
            return new RationalNumber(value);
        }

        /// <summary>
        /// Explicit conversion from decimal to RationalNumber
        /// </summary>
        public static explicit operator RationalNumber(decimal value)
        {
            return new RationalNumber(value);
        }

        /// <summary>
        /// Explicit conversion from RationalNumber to decimal
        /// </summary>
        public static explicit operator decimal(RationalNumber rational)
        {
            return rational.ToDecimal();
        }

        #endregion
    }
}
