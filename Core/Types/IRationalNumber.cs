using System;

namespace ComputorV2.Core.Types
{
    public interface IRationalNumber : IComparable<IRationalNumber>, IEquatable<IRationalNumber>
    {
        long Numerator { get; }
        long Denominator { get; }
        
        bool IsInteger { get; }
        bool IsZero { get; }
        
        decimal ToDecimal();
        
        IRationalNumber Simplify();
        IRationalNumber Abs();
    }
}
