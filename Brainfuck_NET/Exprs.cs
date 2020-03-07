using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Brainfuck_NET
{
	static class Exprs
	{
		private const string pointerName = "pointer";
		private const string arrayName = "array";
		private const int arraySize = 30_000;
		private const string inputName = "input";
		private const string enumeratorName = "enumerator";

		static IEnumerable<StatementSyntax> MethodInit(IOKind ioKind)
		{
			yield return SyntaxFactory.ParseStatement($"int {pointerName} = 0;");
			yield return SyntaxFactory.ParseStatement($"byte[] {arrayName} = new byte[{arraySize}];");

			if (ioKind == IOKind.Argument)
			{
				yield return SyntaxFactory.ParseStatement($"IEnumerator<byte> {enumeratorName} = {inputName}.GetEnumerator();");
			}
		}

		static StatementSyntax Shift(int steps)
		{
			return SyntaxFactory.ParseStatement($"{pointerName} += {steps};");
		}

		static StatementSyntax Increment(int change)
		{
			return SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = (byte)(({arrayName}[{pointerName}] + {change}) % 256);");
		}

		static StatementSyntax Input(IOKind ioKind) => ioKind switch
		{
			IOKind.Argument => SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = {enumeratorName}.MoveNext() ? {enumeratorName}.Current : (byte)0;"),
			IOKind.Console => SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = (byte)Console.Read();"),
			_ => throw new ArgumentException($"Must have valid {nameof(IOKind)}", nameof(ioKind))
		};

		static StatementSyntax Output(IOKind ioKind) => ioKind switch
		{
			IOKind.Argument => SyntaxFactory.ParseStatement($"yield return {arrayName}[{pointerName}];"),
			IOKind.Console => SyntaxFactory.ParseStatement($"Console.Write((char){arrayName}[{pointerName}]);"),
			_ => throw new ArgumentException($"Must have valid {nameof(IOKind)}", nameof(ioKind))
		};

		static StatementSyntax Loop(IEnumerable<StatementSyntax> innerStatements)
		{
			return SyntaxFactory.WhileStatement(SyntaxFactory.ParseExpression($"{arrayName}[{pointerName}] != (byte)0"), SyntaxFactory.Block(innerStatements));
		}
	}
}