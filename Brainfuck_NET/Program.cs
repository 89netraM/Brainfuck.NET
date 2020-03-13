using System;
using System.IO;

namespace Brainfuck_NET
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Provide two (2) arguments: <srcFile> <\"exe\"|\"dll\">");
			}
			else
			{
				string outPath = Path.GetDirectoryName(args[0]);

				try
				{
					if (args[1] == "exe")
					{
						Compiler.CompileExe(args[0], outPath);
					}
					else
					{
						Compiler.CompileDll(args[0], outPath, IOKind.Argument);
					}
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ResetColor();
				}
			}
		}
	}
}