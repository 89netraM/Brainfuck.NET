using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Brainfuck_NET
{
	static class Compiler
	{
		private const string mainMethodName = "Main";
		private const string defaultMethodName = "BFMethod";
		private const string defaultClassName = "BFClass";
		private const string defaultNamespaceName = "BFNamespace";

		internal static void CompileExe(string sourceFilePath, string outDirectoryPath)
		{
			Compile(sourceFilePath, outDirectoryPath, false, IOKind.Console, mainMethodName, defaultClassName, defaultNamespaceName);
		}

		internal static void CompileDll(string sourceFilePath, string outDirectoryPath, IOKind ioKind, string methodName = defaultMethodName, string className = defaultClassName, string namespaceName = defaultNamespaceName)
		{
			Compile(sourceFilePath, outDirectoryPath, true, ioKind, methodName, className, namespaceName);
		}

		private static void Compile(string sourceFilePath, string outDirectoryPath, bool isLib, IOKind ioKind, string methodName, string className, string namespaceName)
		{
			SyntaxTree tree = MakeProgram(sourceFilePath, ioKind, methodName, className, namespaceName);

			CSharpCompilationOptions options = new CSharpCompilationOptions(
				isLib ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
				mainTypeName: className,
				optimizationLevel: OptimizationLevel.Release);
			PortableExecutableReference mscorelib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

			CSharpCompilation compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(sourceFilePath), new[] { tree }, new[] { mscorelib }, options);
			EmitResult result = compilation.Emit(Path.Combine(outDirectoryPath, WithFileExtension(Path.GetFileNameWithoutExtension(sourceFilePath), isLib)));

			if (result.Success)
			{
				Console.WriteLine("Build successfull!");
			}
			else
			{
				Console.WriteLine("Build errors:");
				foreach (Diagnostic diag in result.Diagnostics)
				{
					Console.WriteLine('\t' + diag.ToString());
				}
			}
		}

		private static SyntaxTree MakeProgram(string sourceFilePath, IOKind ioKind, string methodName, string className, string namespaceName)
		{
			ParsingResult result = Parsing.ParseFromFile(sourceFilePath);

			if (!result.Success)
			{
				throw new Exception("Syntax error in source file.");
			}
			else
			{
				if (!result.ParsedAll)
				{
					Console.WriteLine("Could not parse the whole source file, output might not work as intended.");
				}

				MethodDeclarationSyntax method = Exprs.Method(methodName, ioKind, result.SyntaxGenerators);

				ClassDeclarationSyntax @class = Exprs.Class(className);
				@class = @class.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(method));

				NamespaceDeclarationSyntax @namespace = Exprs.Namespace(namespaceName);
				@namespace = @namespace.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(@class));

				return SyntaxFactory.SyntaxTree(@namespace, path: sourceFilePath);
			}
		}

		private static string WithFileExtension(string fileName, bool isLib) => fileName + isLib switch
		{
			true => ".dll",
			false => ".exe"
		};
	}
}