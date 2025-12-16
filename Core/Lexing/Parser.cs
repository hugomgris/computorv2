namespace ComputorV2.Core.Lexing
{
	public enum cmd_type
	{
		RATIONAL,
		COMPLEX,
		MATRIX,
		FUNCTION,
		POLYNOMIAL,
		INVALID
	};
	
	public class Parser
	{
		/*
		fun(x) = 2x + 3
		a = (2 + 3) * 5

		*/
		public cmd_type DetectInputType(string input)
		{
			if (input.Contains(")="))
				return cmd_type.FUNCTION;
			else if (input.Contains("["))
				return cmd_type.MATRIX;
			else if (input.Contains("i"))
				return cmd_type.COMPLEX;
			return cmd_type.RATIONAL;
		}

		public string ResolveVariables(string input)
		{
			return input;
		}
	}
}