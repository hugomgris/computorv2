using System;
using System.Collections;
using System.Text;

using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Core.Lexing
{
	public enum cmd_type
	{
		FUNCTION,
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

	public class Parser
	{
		public cmd_type DetectInputType(string input)
		{
			var parts = input.Split('=');
			string leftSide = parts[0].Trim();

			var match = System.Text.RegularExpressions.Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(\s*([a-zA-Z][a-zA-Z0-9_]*)\s*\)$");
			if (match.Success)
				return cmd_type.FUNCTION;
			else
				return cmd_type.ASSIGNMENT;
			
		}

		public cmd_type DetectValueType(string value)
		{
			if (value.Contains("["))
				return cmd_type.MATRIX;
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
				if (!Char.IsLetterOrDigit(c))
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

		public string ResolveVariables(string input)
		{
			return input;
		}
	}
}