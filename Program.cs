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
				e.Cancel = true;
				repl.Stop();
			};

			repl.Run();
		}

		static public void PrintHeader()
		{
			Console.WriteLine();
			try
			{
				string[] lines = File.ReadAllLines("header_banner.txt");
				foreach (string line in lines)
				{
					Console.WriteLine(line);
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Header banner file not found");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading header: {ex.Message}");
			}
		}
	}
 }
