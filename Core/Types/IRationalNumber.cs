using System;

namespace ComputorV2.Core.Types
{
    /// <summary>
    /// Base interface for rational number types in ComputorV2
    /// Provides foundation for RationalNumber and ComplexNumber
    /// </summary>
    public interface IRationalNumber : IComparable<IRationalNumber>, IEquatable<IRationalNumber>
    {
        /// <summary>
        /// Gets the numerator of the rational number
        /// </summary>
        long Numerator { get; }
        
        /// <summary>
        /// Gets the denominator of the rational number (always positive)
        /// </summary>
        long Denominator { get; }
        
        /// <summary>
        /// Converts the rational number to decimal representation
        /// </summary>
        /// <returns>Decimal approximation of the rational number</returns>
        decimal ToDecimal();
        
        /// <summary>
        /// Returns the simplified form of the rational number
        /// </summary>
        /// <returns>New rational number in lowest terms</returns>
        IRationalNumber Simplify();
        
        /// <summary>
        /// Checks if the rational number is an integer (denominator = 1)
        /// </summary>
        bool IsInteger { get; }
        
        /// <summary>
        /// Checks if the rational number is zero
        /// </summary>
        bool IsZero { get; }
        
        /// <summary>
        /// Gets the absolute value of the rational number
        /// </summary>
        IRationalNumber Abs();
    }
}
