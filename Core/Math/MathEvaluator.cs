namespace ComputorV2.Core.Math
{
	public class MathEvaluator
	{
		public decimal Evaluate(string expression)
		{
			if (expression.Contains('+'))
			{
				var parts = expression.Split('+');
				return decimal.Parse(parts[0].Trim()) + decimal.Parse(parts[1].Trim());
			}
			else if (expression.Contains('-'))
			{
				var parts = expression.Split('-');
				return decimal.Parse(parts[0].Trim()) - decimal.Parse(parts[1].Trim());
			}
			else if (expression.Contains('*'))
			{
				var parts = expression.Split('*');
				return decimal.Parse(parts[0].Trim()) * decimal.Parse(parts[1].Trim());
			}
			else if (expression.Contains('/'))
			{
				var parts = expression.Split('/');
				return decimal.Parse(parts[0].Trim()) / decimal.Parse(parts[1].Trim());
			}
			else
			{
				return decimal.Parse(expression);
			}
		}
	}
}