using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputorV2.Core.Types
{
	public class Polynomial : MathValue, IEquatable<Polynomial>
	{
		private readonly Dictionary<int, MathValue> _terms;

		#region Constructors

		public Polynomial()
		{
			_terms = new Dictionary<int, MathValue>();
		}

		public Polynomial(Dictionary<int, MathValue> terms)
		{
			_terms = new Dictionary<int, MathValue>(terms);
			CleanZeroTerms();
		}

		public Polynomial(string expression)
		{
			_terms = new Dictionary<int, MathValue>();
			ParseExpression(expression.Trim());
		}

		public Polynomial(MathValue constant)
		{
			_terms = new Dictionary<int, MathValue> { { 0, constant } };
		}

		public Polynomial(MathValue linearCoeff, MathValue constant)
		{
			_terms = new Dictionary<int, MathValue>();
			if (!constant.IsZero) _terms[0] = constant;
			if (!linearCoeff.IsZero) _terms[1] = linearCoeff;
		}

		public Polynomial(MathValue quadraticCoeff, MathValue linearCoeff, MathValue constant)
		{
			_terms = new Dictionary<int, MathValue>();
			if (!constant.IsZero) _terms[0] = constant;
			if (!linearCoeff.IsZero) _terms[1] = linearCoeff;
			if (!quadraticCoeff.IsZero) _terms[2] = quadraticCoeff;
		}

		#endregion

		#region Properties

		public override bool IsZero => !_terms.Any() || _terms.All(kvp => kvp.Value.IsZero);

		public override bool IsReal => _terms.All(kvp => kvp.Value.IsReal);

		public int Degree
		{
			get
			{
				CleanZeroTerms();
				return _terms.Any() ? _terms.Keys.Max() : 0;
			}
		}

		public MathValue LeadingCoefficient
		{
			get
			{
				if (IsZero) return new RationalNumber(0);
				return _terms[Degree];
			}
		}

		#endregion

		#region Term Management

		public void AddTerm(int power, MathValue coefficient)
		{
			if (coefficient.IsZero) return;

			if (_terms.ContainsKey(power))
				_terms[power] = _terms[power].Add(coefficient);
			else
				_terms[power] = coefficient;

			if (_terms[power].IsZero)
				_terms.Remove(power);
		}

		public MathValue GetCoefficient(int power)
		{
			return _terms.TryGetValue(power, out MathValue? coefficient) ? coefficient : new RationalNumber(0);
		}

		public Dictionary<int, MathValue> GetTerms()
		{
			CleanZeroTerms();
			return new Dictionary<int, MathValue>(_terms);
		}

		private void CleanZeroTerms()
		{
			var keysToRemove = _terms.Where(kvp => kvp.Value.IsZero).Select(kvp => kvp.Key).ToList();
			foreach (var key in keysToRemove)
			{
				_terms.Remove(key);
			}
		}

		#endregion

		#region Evaluation

		public MathValue Evaluate(MathValue x)
		{
			if (IsZero) return new RationalNumber(0);

			MathValue result = new RationalNumber(0);
			foreach (var term in _terms)
			{
				MathValue termValue = term.Value.Multiply(x.Power(term.Key));
				result = result.Add(termValue);
			}
			return result;
		}

		#endregion

		#region Arithmetic Operations (MathValue Implementation)

		public override MathValue Add(MathValue other)
		{
			return other switch
			{
				Polynomial p => this + p,
				RationalNumber r => this + new Polynomial(r),
				ComplexNumber c => this + new Polynomial(c),
				_ => throw new ArgumentException($"Cannot add {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Subtract(MathValue other)
		{
			return other switch
			{
				Polynomial p => this - p,
				RationalNumber r => this - new Polynomial(r),
				ComplexNumber c => this - new Polynomial(c),
				_ => throw new ArgumentException($"Cannot subtract {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Multiply(MathValue other)
		{
			return other switch
			{
				Polynomial p => this * p,
				RationalNumber r => this * r,
				ComplexNumber c => this * c,
				_ => throw new ArgumentException($"Cannot multiply {GetType().Name} and {other.GetType().Name}")
			};
		}

		public override MathValue Divide(MathValue other)
		{
			return other switch
			{
				RationalNumber r when !r.IsZero => this / r,
				ComplexNumber c when !c.IsZero => this / c,
				_ => throw new ArgumentException($"Cannot divide {GetType().Name} by {other.GetType().Name}")
			};
		}

		public override MathValue Power(int exponent)
		{
			if (exponent < 0) throw new ArgumentException("Negative exponents not supported for polynomials");
			if (exponent == 0) return new Polynomial(new RationalNumber(1));
			if (exponent == 1) return this;

			Polynomial result = this;
			for (int i = 1; i < exponent; i++)
			{
				result = result * this;
			}
			return result;
		}

		public override MathValue Negate() => -this;

		#endregion

		#region Polynomial-Specific Operations

		public static Polynomial operator +(Polynomial left, Polynomial right)
		{
			var result = new Polynomial();
			var allPowers = left._terms.Keys.Union(right._terms.Keys);

			foreach (int power in allPowers)
			{
				var leftCoeff = left.GetCoefficient(power);
				var rightCoeff = right.GetCoefficient(power);
				result.AddTerm(power, leftCoeff.Add(rightCoeff));
			}

			return result;
		}

		public static Polynomial operator -(Polynomial left, Polynomial right)
		{
			return left + (-right);
		}

		public static Polynomial operator -(Polynomial polynomial)
		{
			var result = new Polynomial();
			foreach (var term in polynomial._terms)
			{
				result._terms[term.Key] = term.Value.Negate();
			}
			return result;
		}

		public static Polynomial operator *(Polynomial left, Polynomial right)
		{
			var result = new Polynomial();

			foreach (var leftTerm in left._terms)
			{
				foreach (var rightTerm in right._terms)
				{
					int newPower = leftTerm.Key + rightTerm.Key;
					MathValue newCoeff = leftTerm.Value.Multiply(rightTerm.Value);
					result.AddTerm(newPower, newCoeff);
				}
			}

			return result;
		}

		public static Polynomial operator *(Polynomial polynomial, MathValue scalar)
		{
			var result = new Polynomial();
			foreach (var term in polynomial._terms)
			{
				result._terms[term.Key] = term.Value.Multiply(scalar);
			}
			return result;
		}

		public static Polynomial operator *(MathValue scalar, Polynomial polynomial)
		{
			return polynomial * scalar;
		}

		public static Polynomial operator /(Polynomial polynomial, MathValue scalar)
		{
			if (scalar.IsZero) throw new DivideByZeroException("Cannot divide polynomial by zero");

			var result = new Polynomial();
			foreach (var term in polynomial._terms)
			{
				result._terms[term.Key] = term.Value.Divide(scalar);
			}
			return result;
		}

		#endregion

		#region Calculus Operations

		public Polynomial Derivative()
		{
			var result = new Polynomial();

			foreach (var term in _terms)
			{
				if (term.Key > 0)
				{
					var newCoeff = term.Value.Multiply(new RationalNumber(term.Key));
					result._terms[term.Key - 1] = newCoeff;
				}
			}

			return result;
		}

		public Polynomial Integrate()
		{
			var result = new Polynomial();

			foreach (var term in _terms)
			{
				var newPower = term.Key + 1;
				var newCoeff = term.Value.Divide(new RationalNumber(newPower));
				result._terms[newPower] = newCoeff;
			}

			return result;
		}

		#endregion

		#region Solving (Integration with V1 Logic)

		public List<MathValue> Solve()
		{
			CleanZeroTerms();

			return Degree switch
			{
				0 => SolveDegree0(),
				1 => SolveDegree1(),
				2 => SolveDegree2(),
				_ => throw new NotSupportedException($"Polynomial equations of degree {Degree} are not supported. The polynomial degree is strictly greater than 2.")
			};
		}

		private List<MathValue> SolveDegree0()
		{
			if (IsZero)
			{
				return new List<MathValue> { new InfiniteSolutions() };
			}
			else
			{
				return new List<MathValue>();
			}
		}

		private List<MathValue> SolveDegree1()
		{
			var a = GetCoefficient(1);
			var b = GetCoefficient(0);

			var solution = b.Negate().Divide(a);
			return new List<MathValue> { solution };
		}

		private List<MathValue> SolveDegree2()
		{
			var a = GetCoefficient(2);
			var b = GetCoefficient(1);
			var c = GetCoefficient(0);

			var discriminant = b.Power(2).Subtract(new RationalNumber(4).Multiply(a).Multiply(c));

			if (discriminant.AsRational()?.IsZero == true)
			{
				var solution = b.Negate().Divide(new RationalNumber(2).Multiply(a));
				return new List<MathValue> { solution };
			}
			else if (discriminant.AsRational()?.Numerator > 0)
			{
				var sqrtDiscriminant = CalculateSquareRoot(discriminant.AsRational()!);
				var twoA = new RationalNumber(2).Multiply(a);

				var solution1 = b.Negate().Add(sqrtDiscriminant).Divide(twoA);
				var solution2 = b.Negate().Subtract(sqrtDiscriminant).Divide(twoA);

				return new List<MathValue> { solution1, solution2 };
			}
			else
			{
				var negDiscriminant = discriminant.Negate().AsRational()!;
				var sqrtDiscriminant = CalculateSquareRoot(negDiscriminant);
				var twoA = new RationalNumber(2).Multiply(a);

				var realPart = b.Negate().Divide(twoA);
				var imaginaryPart = sqrtDiscriminant.Divide(twoA.AsRational()!);

				var solution1 = new ComplexNumber(realPart.AsRational()!, imaginaryPart.AsRational()!);
				var solution2 = new ComplexNumber(realPart.AsRational()!, imaginaryPart.AsRational()!.Negate().AsRational()!);

				return new List<MathValue> { solution1, solution2 };
			}
		}

		private RationalNumber CalculateSquareRoot(RationalNumber value)
		{
			if (value.Numerator < 0) throw new ArgumentException("Cannot calculate square root of negative number");

			double doubleValue = (double)value.Numerator / value.Denominator;
			double sqrt = System.Math.Sqrt(doubleValue);

			long sqrtNum = (long)System.Math.Round(sqrt * value.Denominator);
			if (sqrtNum * sqrtNum == value.Numerator * value.Denominator)
			{
				return new RationalNumber(sqrtNum, value.Denominator);
			}

			return new RationalNumber((decimal)sqrt);
		}

		#endregion

		#region Equality and Comparison

		public bool Equals(Polynomial? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			CleanZeroTerms();
			other.CleanZeroTerms();

			if (_terms.Count != other._terms.Count) return false;

			foreach (var term in _terms)
			{
				if (!other._terms.TryGetValue(term.Key, out MathValue? otherCoeff))
					return false;
				if (!term.Value.Equals(otherCoeff))
					return false;
			}

			return true;
		}

		public override bool Equals(MathValue? other) => other is Polynomial p && this.Equals(p);

		public override bool Equals(object? obj) => obj is Polynomial p && this.Equals(p);

		public override int GetHashCode()
		{
			CleanZeroTerms();
			int hash = 0;
			foreach (var term in _terms.OrderBy(kvp => kvp.Key))
			{
				hash = HashCode.Combine(hash, term.Key, term.Value.GetHashCode());
			}
			return hash;
		}

		#endregion

		#region String Representation

		public override string ToString()
		{
			CleanZeroTerms();

			if (IsZero) return "0";

			var terms = new List<string>();
			var sortedTerms = _terms.OrderByDescending(kvp => kvp.Key);

			foreach (var term in sortedTerms)
			{
				string termStr = FormatTerm(term.Key, term.Value, terms.Count == 0);
				if (!string.IsNullOrEmpty(termStr))
					terms.Add(termStr);
			}

			return terms.Count > 0 ? string.Join(" ", terms) : "0";
		}

		private string FormatTerm(int power, MathValue coefficient, bool isFirst)
		{
			if (coefficient.IsZero) return "";

			string sign = "";
			string coeffStr = "";
			string varStr = "";

			if (coefficient.AsRational()?.Numerator < 0 || (coefficient.AsComplex()?.Real.Numerator < 0))
			{
				sign = isFirst ? "-" : "- ";
				coefficient = coefficient.Negate();
			}
			else if (!isFirst)
			{
				sign = "+ ";
			}

			if (power == 0 || !IsOne(coefficient))
			{
				coeffStr = coefficient.ToString();
			}

			if (power == 1)
			{
				varStr = coeffStr.Length > 0 ? " * X" : "X";
			}
			else if (power > 1)
			{
				varStr = coeffStr.Length > 0 ? $" * X^{power}" : $"X^{power}";
			}

			return $"{sign}{coeffStr}{varStr}";
		}

		private bool IsOne(MathValue value)
		{
			return value.AsRational()?.Numerator == 1 && value.AsRational()?.Denominator == 1;
		}

		public string ToReducedForm()
		{
			if (IsZero) return "0 = 0";
			return $"{ToString()} = 0";
		}

		#endregion

		#region MathValue Abstract Members

		public override RationalNumber? AsRational()
		{
			if (Degree == 0 && _terms.Count == 1)
			{
				return _terms[0].AsRational();
			}
			return null;
		}

		public override ComplexNumber? AsComplex()
		{
			if (Degree == 0 && _terms.Count == 1)
			{
				return _terms[0].AsComplex();
			}
			return null;
		}

		#endregion

		#region String Parsing

		/// <summary>
		/// Parse a polynomial expression string like "x^2 + 2*x + 1" or "2x + 3"
		/// </summary>
		private void ParseExpression(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return; // Empty polynomial
			}

			// Normalize the expression
			expression = NormalizeExpression(expression);
			
			// Split by + and - while preserving the operators
			var terms = SplitIntoTerms(expression);
			
			foreach (var term in terms)
			{
				if (string.IsNullOrWhiteSpace(term)) continue;
				ParseTerm(term.Trim());
			}
		}

		/// <summary>
		/// Normalize expression: add spaces, handle implicit multiplication, etc.
		/// </summary>
		private string NormalizeExpression(string expression)
		{
			// Replace common patterns
			expression = System.Text.RegularExpressions.Regex.Replace(expression, @"(\d)([a-zA-Z])", "$1*$2"); // 2x -> 2*x
			expression = System.Text.RegularExpressions.Regex.Replace(expression, @"([a-zA-Z])(\d)", "$1*$2"); // x2 -> x*2
			expression = System.Text.RegularExpressions.Regex.Replace(expression, @"([a-zA-Z])\^", "$1^"); // Remove space before ^
			expression = expression.Replace(" ", ""); // Remove all spaces for easier parsing
			
			// Ensure we have + at the beginning if no sign
			if (!expression.StartsWith("+") && !expression.StartsWith("-"))
			{
				expression = "+" + expression;
			}
			
			return expression;
		}

		/// <summary>
		/// Split expression into individual terms while preserving signs
		/// </summary>
		private List<string> SplitIntoTerms(string expression)
		{
			var terms = new List<string>();
			var currentTerm = new StringBuilder();
			bool isFirst = true;
			
			for (int i = 0; i < expression.Length; i++)
			{
				char c = expression[i];
				
				if ((c == '+' || c == '-') && !isFirst)
				{
					// Save current term and start new one
					if (currentTerm.Length > 0)
					{
						terms.Add(currentTerm.ToString());
						currentTerm.Clear();
					}
				}
				
				currentTerm.Append(c);
				isFirst = false;
			}
			
			// Add the last term
			if (currentTerm.Length > 0)
			{
				terms.Add(currentTerm.ToString());
			}
			
			return terms;
		}

		/// <summary>
		/// Parse a single term like "+2*x^2" or "-3x" or "+5"
		/// </summary>
		private void ParseTerm(string term)
		{
			if (string.IsNullOrWhiteSpace(term)) return;
			
			// Handle sign
			bool isNegative = term.StartsWith("-");
			if (term.StartsWith("+") || term.StartsWith("-"))
			{
				term = term.Substring(1);
			}
			
			// Default values
			MathValue coefficient = new RationalNumber(1);
			int power = 0;
			
			// Check if term contains variable
			if (term.Contains("x") || term.Contains("X"))
			{
				// Parse coefficient and power
				var parts = term.Split(new char[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);
				
				// Handle coefficient
				if (parts.Length == 0 || string.IsNullOrEmpty(parts[0]))
				{
					// Just "x" or "X"
					coefficient = new RationalNumber(1);
					power = 1;
				}
				else
				{
					// Coefficient exists
					var coeffPart = parts[0].Replace("*", "");
					if (string.IsNullOrEmpty(coeffPart))
					{
						coefficient = new RationalNumber(1);
					}
					else
					{
						if (RationalNumber.TryParse(coeffPart, out var parsedCoeff))
						{
							coefficient = parsedCoeff!;
						}
					}
					power = 1; // Default power for variable terms
				}
				
				// Handle power
				if (term.Contains("^"))
				{
					var powerPart = term.Substring(term.IndexOf('^') + 1);
					if (int.TryParse(powerPart, out int parsedPower))
					{
						power = parsedPower;
					}
				}
			}
			else
			{
				// Constant term (no variable)
				if (RationalNumber.TryParse(term, out var parsedCoeff))
				{
					coefficient = parsedCoeff!;
				}
				power = 0;
			}
			
			// Apply sign
			if (isNegative && coefficient != null)
			{
				coefficient = coefficient.Negate();
			}
			
			// Add term to polynomial
			if (coefficient != null)
			{
				AddTerm(power, coefficient);
			}
		}

		#endregion
	}

	public class InfiniteSolutions : MathValue
	{
		public override bool IsZero => false;
		public override bool IsReal => true;

		public override string ToString() => "All real numbers";

		public override MathValue Add(MathValue other) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Subtract(MathValue other) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Multiply(MathValue other) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Divide(MathValue other) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Power(int exponent) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Negate() => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");

		public override RationalNumber? AsRational() => null;
		public override ComplexNumber? AsComplex() => null;

		public override bool Equals(MathValue? other) => other is InfiniteSolutions;
		public override bool Equals(object? obj) => obj is InfiniteSolutions;
		public override int GetHashCode() => typeof(InfiniteSolutions).GetHashCode();
	}
}
