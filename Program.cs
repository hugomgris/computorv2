 using ComputorV2.Core.Parsing;
 using ComputorV2.Interactive;

 namespace ComputorV2
 {
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var parser = new Parser();
				var helpSystem = new HelpSystem();
				
				if (args.Length != 0)
				{
					Console.WriteLine("Error: Execution: please run './computorV2' with no arguments or execute 'make run'.");
					return;
				}

				string	input = "";
				bool	showGraphs = false;

				PrintHeader();
				Console.WriteLine("Welcome to Computor V2!");
				Console.WriteLine("Type your command or 'help' for assistence.");
				Console.Write("> ");

				input = Console.ReadLine() ?? string.Empty;

				while (true)
				{
					if (string.IsNullOrWhiteSpace(input))
					{
						while (string.IsNullOrWhiteSpace(input))
						{
							Console.WriteLine("No input detected.\nType your command or 'help' for assistence.");
							input = Console.ReadLine() ?? string.Empty;
						}
					}

					if (input == "exit")
					{
						Console.WriteLine("Thanks for using computorV2!");
						return;
					}
					else if (input == "help")
					{
						helpSystem.print_help();
					}
					else
					{
						parser.process_input(input);
					}

					input = "";
					Console.Write("> ");
					input = Console.ReadLine() ?? string.Empty;
				
				}
				
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
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
