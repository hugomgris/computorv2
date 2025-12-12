using System;

namespace ComputorV2.Core.Types
{
	public abstract class MathValue : IEquatable<MathValue>
	{
		public abstract MathValue Add(MathValue other);
		public abstract MathValue Subtract(MathValue other);
		public abstract MathValue Multiply(MathValue other);
		public abstract MathValue Divide(MathValue other);
		public abstract MathValue Modulo(MathValue other);
		public abstract MathValue Power(int exponent);
		public abstract MathValue Negate();

		public abstract bool IsZero { get; }
		public abstract bool IsReal { get; }
		public virtual bool IsComplex => !IsReal;
		public virtual bool IsRational => false;

		public abstract bool Equals(MathValue? other);
		public abstract override bool Equals(object? obj);
		public abstract override int GetHashCode();
		public abstract override string ToString();

		public virtual RationalNumber? AsRational() => null;
		public virtual ComplexNumber? AsComplex() => null;

		public static MathValue operator +(MathValue a, MathValue b) => a.Add(b);
		public static MathValue operator -(MathValue a, MathValue b) => a.Subtract(b);
		public static MathValue operator *(MathValue a, MathValue b) => a.Multiply(b);
		public static MathValue operator /(MathValue a, MathValue b) => a.Divide(b);
		public static MathValue operator -(MathValue a) => a.Negate();
	}
}