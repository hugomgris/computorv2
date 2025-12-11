using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
	public class Function : MathValue
	{
		private readonly string _variable;
		private readonly MathValue _expression;
		private readonly string _name;

		#region Constructors

		public Function(string name, string variable, string expression)
		{
			_name = name;
			_variable = variable;

			if (ComplexNumber.TryParse(expression, out ComplexNumber? complex))
				_expression = complex!;
			else if (RationalNumber.TryParse(expression, out RationalNumber? rational))
				_expression = rational!;
			else if (Matrix.TryParse(expression, out Matrix? matrix))
				_expression = matrix!;
			else
				_expression = new Polynomial(expression);
		}

		public Function(string name, string variable, MathValue expression)
		{
			_name = name;
			_variable = variable;
			_expression = expression;
		}

		#endregion

		#region Properties

		public string Name => _name;
		public string Variable => _variable;
		public MathValue Expression => _expression;

		public override bool IsReal => true;
		public override bool IsZero => false;

		#endregion

		#region Core Functionality

		public MathValue Evaluate(MathValue input)
		{
			if (_expression is Polynomial poly)
			{
				return poly.Evaluate(input);
			}
			else if (_expression is RationalNumber rational)
			{
				return rational;
			}
			else if (_expression is ComplexNumber complex)
			{
				return complex;
			}
			else if (_expression is Matrix matrix)
			{
				return matrix;
			}

			throw new NotImplementedException(($"Function evaluation is not implemented fo {_expression.GetType()}"));
		}

		public override string ToString()
		{
			return $"{_name}({_variable}) = {_expression}";
		}

		/* public Function Compose(Function other)
		{

		}

		public Function Derive()
		{

		} */

		#endregion

		#region MathValue Implementation

		public override MathValue Add(MathValue other)
		{
			if (other is not Function otherFunction)
				throw new ArgumentException("Function addition requires another function");
	
			return Add(otherFunction);
		}

		public Function Add(Function other)
		{
			if (_variable != other._variable)
				throw new ArgumentException($"Cannot add functions with different variables: '{_variable}' and '{other._variable}'");

			try
			{
				string newName = $"({_name} + {other._name})";
				
				MathValue combinedExpression = _expression.Add(other._expression);
			
				return new Function(newName, _variable, combinedExpression);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot add functions: {e.Message}");
			}
		}

		public override MathValue Subtract(MathValue other)
		{
			if (other is not Function otherFunction)
        		throw new ArgumentException("Function subtraction requires another function");
    
    		return Subtract(otherFunction);
		}

		public Function Subtract(Function other)
		{
			if (_variable != other._variable)
			{
				throw new ArgumentException($"Cannot subtract functions with different variables: '{_variable}' and '{other._variable}'");
			}
			
			try 
			{
				string combinedName = $"({_name} - {other._name})";
				MathValue combinedExpression = _expression.Subtract(other._expression);
				return new Function(combinedName, _variable, combinedExpression);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Cannot subtract functions: {ex.Message}", ex);
			}
		}

		public override MathValue Multiply(MathValue other) => throw new NotImplementedException();
		public override MathValue Divide(MathValue other) => throw new NotImplementedException();
		public override MathValue Negate() => throw new NotImplementedException();
		public override MathValue Power(int exponent) => throw new NotImplementedException();

		#endregion

		#region Not implemented inheritances
		public override bool Equals(MathValue? other) => throw new NotImplementedException();
		public override bool Equals(object? obj) => throw new NotImplementedException();
		public override int GetHashCode() => throw new NotImplementedException();

		#endregion
	}
}