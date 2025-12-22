using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ComputorV2.Core.Math;
using ComputorV2.Core.Lexing;

namespace ComputorV2.Core.Types
{
	public class Polynomial : MathValue, IEquatable<Polynomial>
	{
		private readonly Dictionary<int, MathValue> _terms;
		private string								_originalVariable = "x";

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
			GetVariable(expression);
			ParseExpression(expression.Trim());
			Console.WriteLine($"final poly->{this.ToString()}");
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
			{
				Console.WriteLine($"_terms contains [{power}] and is going to add {_terms[power]} and {coefficient}");
				_terms[power] = _terms[power].Add(coefficient);
			}
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

		public override MathValue Modulo(MathValue other)
		{
			throw new ArgumentException("Modulo operation is not supported for polynomials");
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

		public Polynomial Derive()
		{
			return Derivative();
		}

		public Polynomial Compose(Polynomial other)
		{
			var result = new Polynomial();
			
			foreach (var term in _terms)
			{
				int power = term.Key;
				var coefficient = term.Value;
				
				if (power == 0)
				{
					result = (Polynomial)result.Add(new Polynomial(coefficient));
				}
				else
				{
					var otherPowered = (Polynomial)other.Power(power);
					var scaledTerm = (Polynomial)otherPowered.Multiply(coefficient);
					result = (Polynomial)result.Add(scaledTerm);
				}
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

		public bool Equals(MathValue? other) => other is Polynomial p && this.Equals(p);

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
				coeffStr = coefficient.ToString()!;
			}

			if (power == 1)
			{
				varStr = coeffStr.Length! > 0 ? " * " + _originalVariable : _originalVariable;
			}
			else if (power > 1)
			{
				varStr = coeffStr.Length > 0 ? $" * " + _originalVariable + $"^{power}" : _originalVariable + $"^{power}";
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

		private void ParseExpression(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return;
			}

			Console.WriteLine($"At parsed expression, expression is->{expression}");

			expression = NormalizeExpression(expression);

			var terms = SplitIntoTerms(expression);
			
			foreach (var term in terms)
			{
				Console.WriteLine($"Parsing term->{term}");
				if (string.IsNullOrWhiteSpace(term)) continue;
				ParseTerm(term.Trim());
			}
		}

		private string NormalizeExpression(string expression)
		{
			/* if (CheckIfNeedsExpansion(expression))
				expression = Expand(expression); */

			/* expression = System.Text.RegularExpressions.Regex.Replace(expression, @"(\d)([a-zA-Z])", "$1*$2");
			expression = System.Text.RegularExpressions.Regex.Replace(expression, @"([a-zA-Z])(\d)", "$1*$2");
			expression = System.Text.RegularExpressions.Regex.Replace(expression, @"([a-zA-Z])\^", "$1^"); */
			expression = expression.Replace(" ", "");
			
			if (!expression.StartsWith("+") && !expression.StartsWith("-"))
			{
				expression = "+" + expression;
			}

			Console.WriteLine($"At normalizeExpression, expression->{expression}");

			/* if (CheckIfNeedsExpansion(expression))
				expression = Expand(expression); */
			
			return expression;
		}

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
					if (currentTerm.Length > 0)
					{
						terms.Add(currentTerm.ToString());
						currentTerm.Clear();
					}
				}
				
				currentTerm.Append(c);
				isFirst = false;
			}
			
			if (currentTerm.Length > 0)
			{
				terms.Add(currentTerm.ToString());
			}
			
			return terms;
		}

		private void ParseTerm(string term)
		{
			if (string.IsNullOrWhiteSpace(term)) return;

			bool isNegative = term.StartsWith("-");
			if (term.StartsWith("+") || term.StartsWith("-"))
			{
				term = term.Substring(1);
			}
			
			MathValue coefficient = new RationalNumber(1);
			int power = 0;

			if (term.Contains(_originalVariable))
				term = ConvertToXTerm(term);
			Console.WriteLine($"ConvertedTerm->{term}");
			
			if (term.Contains("x") || term.Contains("X"))
			{
				if (term.Count(f => f == 'x') > 1)
					term = ComputeTerm(term);
				
				var parts = term.Split(new char[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);
				Console.WriteLine($"Length of split parts -> {parts.Length}");
				foreach (string part in parts) Console.WriteLine($"part->{part}");
				
				if (parts.Length == 0 || string.IsNullOrEmpty(parts[0]))
				{
					coefficient = new RationalNumber(1);
					power = 1;
				}
				else
				{
					var coeffPart = parts[0].Replace("*", "");
					Console.WriteLine($"coeffPart->{coeffPart}");
					if (string.IsNullOrEmpty(coeffPart))
					{
						coefficient = new RationalNumber(1);
					}
					else
					{
						Console.WriteLine($"Trying to parse->{coeffPart}");
						if (RationalNumber.TryParse(coeffPart, out var parsedCoeff))
						{
							coefficient = parsedCoeff!;
						}
					}
					power = 1;
				}
				
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
				Tokenizer tokenizer = new Tokenizer();
				List<string> tokens = tokenizer.Tokenize(term);
				Postfix postfix = new Postfix(tokens);
				MathValue value = postfix.Calculate();
				term = value.ToString()!;
				
				if (RationalNumber.TryParse(term, out var parsedCoeff))
				{
					coefficient = parsedCoeff!;
				}
				power = 0;
			}
			
			if (isNegative && coefficient != null)
			{
				coefficient = coefficient.Negate();
			}
			
			if (coefficient != null)
			{
				AddTerm(power, coefficient);
			}
		}

		#endregion

		#region Helpers

		private void GetVariable(string expression)
		{
			Console.WriteLine($"expressionAAAAAAA->{expression}");
			for (int i = 0; i < expression.Length; i++)
			{
				if (char.IsLetter(expression[i]))
				{
					int j = i;
					int count = 0;
					while (j < expression.Length && char.IsLetter(expression[j++]))
						count++;
					_originalVariable = expression.Substring(i, count);
					i += _originalVariable.Length;
				}
			}
		}

		private string ConvertToXTerm(string term)
		{
			string newTerm = "";
			
			for (int i = 0; i < term.Length; i++)
			{
				if (term[i] == _originalVariable[0])
				{
					newTerm += "x";
					i += _originalVariable.Length - 1;
				}
				else
					newTerm += term[i].ToString();
			}

			return newTerm;
		}

		private string ConvertToOriginalVariableTerm(string term)
		{
			string newTerm = "";

			for (int i = 0; i < term.Length; i++)
			{
				if (term[i] == 'x' || term[i] == 'X')
					newTerm += _originalVariable;
				else
					newTerm += term[i].ToString();
			}

			Console.WriteLine($"ReversionToOriginal->{newTerm}");

			return newTerm;
		}

		private string ComputeTerm(string term)
		{
			if (term.Length == 1 && term.Contains("x"))
				return "x";
			bool containsVariable = term.Contains("x");
			
			term = term.Replace("X", "x");
			string newTerm = "";

			for (int i = 0; i < term.Length; i++)
			{
				if (term[i] == '*')
				{
					if (term[i + 1] == 'x')
						continue;
					else
						newTerm += term[i].ToString();
				}
				else
					newTerm += term[i].ToString();
			}

			newTerm = newTerm.Replace("x", "");
			Console.WriteLine($"at ComputeTerm, newTerm->{newTerm}");
			
			Tokenizer tokenizer = new Tokenizer();
			List<string> tokens = tokenizer.Tokenize(newTerm);
			foreach (string tok in tokens) Console.WriteLine(tok);
			Postfix postfix = new Postfix(tokens);
			MathValue value = postfix.Calculate();

			string? returnTerm = (containsVariable) ? value.ToString() + "x" : value.ToString();
			Console.WriteLine($"returnTerm->{returnTerm}");
			
			return returnTerm!;
		}
		
		private bool CheckIfNeedsExpansion(string expression)
		{
			var match = System.Text.RegularExpressions.Regex.Match(expression, @"\((.*)\)");
			return match.Success;
		}

		private string Expand(string expression)
		{
			int openIndex = 0;
			int closeIndex = 0;

			for (int i = 0; i < expression.Length; i++)
			{
				if (expression[i] == '(')
					openIndex = i;
				else if (expression[i] == ')')
					closeIndex = i;
				
				if (openIndex != 0 && closeIndex != 0)
					break;
			}


			string left = expression.Substring(0, openIndex);
			string parenthesis = expression.Substring(openIndex);
			string op = expression[closeIndex + 1].ToString();

			int len = 0;

			while(parenthesis[len] != ')') len++;
			len += 2;
			while(char.IsDigit(parenthesis[len])) len++;

			string right = parenthesis.Substring(len);
			parenthesis = parenthesis.Substring(0, len);

			parenthesis = ExecuteExpansion(parenthesis, op);

			//TODO: more than one and nested parenthesis
			/* if (CheckIfNeedsExpansion(expression))
				Expand(expression); */

			string result = left + parenthesis + right;
			return result;
		}

		private string ExecuteExpansion(string expression, string op)
		{
			string[] parts = expression.Split(op);
			string left = parts[0].Trim('(', ')');
			string right = parts[1];

			switch (op)
			{
				case "*":
					expression = DistributeMultiplication(left, right);
					break;
				
				default:
					break;

			}
			
			return expression;
		}

		private string DistributeMultiplication(string left, string right)
		{
			StringBuilder sb = new StringBuilder();

			string accumulatedNumber = "";
			for (int i = 0; i < left.Length; i++)
			{
				if (char.IsDigit(left[i]) || left[i] == 'x')
				{
					if (i != left.Length - 1)
						accumulatedNumber += left[i].ToString();
					else
					{
						if (char.IsDigit(left[i]))
							sb.Append((decimal.Parse(left[i].ToString()) * decimal.Parse(right)).ToString());
						else if (left[i] == 'x')
						{
							if (accumulatedNumber.Length > 0)
							{
								sb.Append((decimal.Parse(accumulatedNumber) * decimal.Parse(right)).ToString() + "x");
							}
							else
							{
								sb.Append(right + "x");
							}
						}
					}
				}
				else if ("+-*/&^".Contains(left[i]) || i == left.Length - 1)
				{
					if (accumulatedNumber.Contains("x"))
					{
						if (accumulatedNumber.Length > 1)
							accumulatedNumber.Trim('x');
						else
							accumulatedNumber = "1";
						decimal convertedNumber = decimal.Parse(accumulatedNumber);
						decimal partial = convertedNumber * decimal.Parse(right);
						sb.Append(partial.ToString() + "x");
						sb.Append(left[i].ToString());
						accumulatedNumber = "";
					}
					else
					{
						decimal convertedNumber = decimal.Parse(accumulatedNumber);
						decimal partial = convertedNumber * decimal.Parse(right);
						sb.Append(partial.ToString());
						sb.Append(left[i].ToString());
						accumulatedNumber = "";
					}
				}
			}

			return sb.ToString();
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
		public override MathValue Modulo(MathValue other) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Power(int exponent) => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");
		public override MathValue Negate() => throw new InvalidOperationException("Cannot perform arithmetic on infinite solutions");

		public override RationalNumber? AsRational() => null;
		public override ComplexNumber? AsComplex() => null;

		public bool Equals(MathValue? other) => other is InfiniteSolutions;
		public override bool Equals(object? obj) => obj is InfiniteSolutions;
		public override int GetHashCode() => typeof(InfiniteSolutions).GetHashCode();
	}
}