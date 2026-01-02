using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Core.Lexing
{
	public enum cmd_type
	{
		FUNCTION,
		FUNCTION_SOLVE_VALUE,
		ASSIGNMENT,
		RATIONAL,
		COMPLEX,
		MATRIX,
		
		POLYNOMIAL,
		INVALID
	};
	
	public enum var_error
	{
		INVALIDCHAR,
		NOALPHA,
		HASICHAR,
		GOOD
	}

	public enum fun_error
	{
		INVALIDCHAR,
		NOALPHA,
		GOOD
	}

	public class Parser
	{
		public cmd_type DetectInputType(string input)
		{
			// This should only have one regex check but regex is hell and I'm going to lose my mind
			Regex regex = new Regex("[A-Za-z]+\\([0-9]+\\)", RegexOptions.IgnoreCase);
			if (regex.IsMatch(input))
				return cmd_type.FUNCTION;

			var parts = input.Split('=');
			string leftSide = parts[0].Trim();

			var match = Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(\s*([a-zA-Z][a-zA-Z0-9_]*)\s*\)$");
			if (match.Success)
				return cmd_type.FUNCTION;
			else
				return cmd_type.ASSIGNMENT;	
		}

		public cmd_type DetectValueType(string value)
		{
			if (value.Contains("["))
			{
				if (!value.Contains("[["))
					return cmd_type.INVALID;
				else if (value.Contains("[[") && !value.Contains("]]"))
					return cmd_type.INVALID;
				return cmd_type.MATRIX;
			}
			else if (value.Contains("i"))
				return cmd_type.COMPLEX;
			else
				return cmd_type.RATIONAL;
		}

		public var_error ValidateVariableName(string name)
		{
			int alpha = 0;
			
			foreach(char c in name)
			{
				if (!Char.IsLetter(c))
					return var_error.INVALIDCHAR;

				if (c == 'i')
					return var_error.HASICHAR;
				
				if (char.IsLetter(c))
					alpha++;
			}

			if (alpha == 0)
				return var_error.NOALPHA;

			return var_error.GOOD;
		}

		public fun_error ValidateFunctionName(string name)
		{
			int alpha = 0;
			
			foreach(char c in name)
			{
				if (!Char.IsLetter(c))
					return fun_error.INVALIDCHAR;
				else
					alpha++;
			}

			if (alpha == 0)
				return fun_error.NOALPHA;

			return fun_error.GOOD;
		}

		public string ResolveVariables(string input)
		{
			return input;
		}
	}
}