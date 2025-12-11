using System;
using ComputorV2.Core.Math;

namespace ComputorV2.Core.Types
{
	public class Function : MathValue, IEquatable<Function>
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

		public Function Compose(Function other)
		{
			if (other._variable != _variable)
				throw new ArgumentException($"Cannot compose functions with different variables: '{_variable}' and '{other._variable}'");

			string newName = $"({_name} ∘ {other._name})";
			
			if (_expression is Polynomial thisPolynom && other._expression is Polynomial otherPolynom)
			{
				var composedPoly = thisPolynom.Compose(otherPolynom);
				return new Function(newName, _variable, composedPoly);
			}
			else if (_expression is RationalNumber rational)
			{
				return new Function(newName, _variable, rational);
			}
			else if (_expression is ComplexNumber complex)
			{
				return new Function(newName, _variable, complex);
			}
			else if (_expression is Matrix matrix)
			{
				return new Function(newName, _variable, matrix);
			}

			throw new NotImplementedException($"Function composition not implemented for expression type: {_expression.GetType()}");
		}

		public Function Derive()
		{
			string newName = $"{_name}'";
			
			if (_expression is Polynomial polynomial)
			{
				var derivedPoly = polynomial.Derive();
				return new Function(newName, _variable, derivedPoly);
			}
			else if (_expression is RationalNumber || _expression is ComplexNumber || _expression is Matrix)
			{
				return new Function(newName, _variable, RationalNumber.Zero);
			}

			throw new NotImplementedException($"Function derivation not implemented for expression type: {_expression.GetType()}");
		}

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

		public override MathValue Multiply(MathValue other)
		{
			if (other is not Function otherFunction)
				throw new ArgumentException("Function multiplication requires another function");
	
			return Multiply(otherFunction);
		}

		public Function Multiply(Function other)
		{
			if (_variable != other._variable)
				throw new ArgumentException($"Cannot multiply functions with different variables: '{_variable}' and '{other._variable}'");

			try
			{
				string newName = $"({_name} * {other._name})";
				MathValue combinedExpression = _expression.Multiply(other._expression);
				return new Function(newName, _variable, combinedExpression);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot multiply functions: {e.Message}");
			}
		}

		public override MathValue Divide(MathValue other)
		{
			if (other is not Function otherFunction)
				throw new ArgumentException("Function division requires another function");
	
			return Divide(otherFunction);
		}

		public Function Divide(Function other)
		{
			if (_variable != other._variable)
				throw new ArgumentException($"Cannot divide functions with different variables: '{_variable}' and '{other._variable}'");

			try
			{
				string newName = $"({_name} / {other._name})";
				MathValue combinedExpression = _expression.Divide(other._expression);
				return new Function(newName, _variable, combinedExpression);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot divide functions: {e.Message}");
			}
		}

		public override MathValue Negate()
		{
			try
			{
				string newName = $"-{_name}";
				MathValue negatedExpression = _expression.Negate();
				return new Function(newName, _variable, negatedExpression);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot negate function: {e.Message}");
			}
		}

		public override MathValue Power(int exponent)
		{
			try
			{
				string newName = exponent == 0 ? "1" : 
				                exponent == 1 ? _name : 
				                $"{_name}^{exponent}";
				
				MathValue poweredExpression = _expression.Power(exponent);
				return new Function(newName, _variable, poweredExpression);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Cannot raise function to power {exponent}: {e.Message}");
			}
		}

		#endregion

		#region Inheritance implementations

		public override bool Equals(MathValue? other)
		{
			return other is Function function && Equals(function);
		}

		public bool Equals(Function? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			
			return _name == other._name &&
			       _variable == other._variable &&
			       _expression.Equals(other._expression);
		}

		public override bool Equals(object? obj)
		{
			return obj is Function function && Equals(function);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_name, _variable, _expression);
		}

		#endregion
	}
}