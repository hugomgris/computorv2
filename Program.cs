 using ComputorV2.Core.Parsing;
 using ComputorV2.Interactive;
 using ComputorV2.IO;

 namespace ComputorV2
 {
	class Program
	{
		static void Main(string[] args)
		{
			// For testing purposes, run tests before starting REPL
			if (args.Length > 0 && args[0] == "--test")
			{
				TestUnifiedSystem.RunTests();
				return;
			}
			
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
