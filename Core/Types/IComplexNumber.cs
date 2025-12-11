using System;

namespace ComputorV2.Core.Types
{
	public interface IComplexNumber : IComparable<IComplexNumber>, IEquatable<IComplexNumber>
	{
		RationalNumber Real { get; }
		RationalNumber Imaginary { get; }

		bool IsReal { get; }
        bool IsImaginary { get; }

		public double Magnitude { get; }
	}
}