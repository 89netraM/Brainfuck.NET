using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BrainfuckNET
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Provide two (2) arguments: <srcFile> <\"exe\"|\"dll\">");
				Console.WriteLine("When compiling to dll, a thrid argument can be provided: <NamesapceName.ClassName.MethodName>");
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
						if (args.Length >= 3)
						{
							Match match = Regex.Match(args[2], @"^(\w+)\.(\w+)\.(\w+)$");
							if (match.Success)
							{
								string method = match.Groups[3].Value;
								string @class = match.Groups[2].Value;
								string @namespace = match.Groups[1].Value;

								if (method != @class)
								{
									Compiler.CompileDll(
										args[0],
										outPath,
										IOKind.Argument,
										method,
										@class,
										@namespace);
								}
								else
								{
									throw new Exception("The class name and the method name can't be the same.");
								}
							}
							else
							{
								throw new Exception("Couldn't parse the naming parameter.");
							}
						}
						else
						{
							Compiler.CompileDll(args[0], outPath, IOKind.Argument);
						}
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