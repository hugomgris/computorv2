 using ComputorV2.Core.Parsing;
 using ComputorV2.Interactive;
 using ComputorV2.IO;

 namespace ComputorV2
 {
	class Program
	{
		static void Main(string[] args)
		{
			var repl = new REPL();
			
			Console.CancelKeyPress += (sender, e) =>
			{
				repl.Stop();
				Environment.Exit(0);
			};

			repl.Run();
		}
	}
 }
