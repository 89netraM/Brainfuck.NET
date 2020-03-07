﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brainfuck_NET
{
	static class Exprs
	{
		private const string pointerName = "pointer";
		private const string arrayName = "array";
		private const int arraySize = 30_000;
		private const string inputName = "input";
		private const string enumeratorName = "enumerator";

		internal static IEnumerable<StatementSyntax> MethodInit(IOKind ioKind)
		{
			yield return SyntaxFactory.ParseStatement($"int {pointerName} = 0;");
			yield return SyntaxFactory.ParseStatement($"byte[] {arrayName} = new byte[{arraySize}];");

			if (ioKind == IOKind.Argument)
			{
				yield return SyntaxFactory.ParseStatement($"IEnumerator<byte> {enumeratorName} = {inputName}.GetEnumerator();");
			}
		}

		internal static SyntaxGenerator Shift(int steps)
		{
			return _ => SyntaxFactory.ParseStatement($"{pointerName} += {steps};");
		}

		internal static SyntaxGenerator Increment(int change)
		{
			return _ => SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = (byte)(({arrayName}[{pointerName}] + {change}) % 256);");
		}

		internal static SyntaxGenerator Input() => ioKind => ioKind switch
		{
			IOKind.Argument => SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = {enumeratorName}.MoveNext() ? {enumeratorName}.Current : (byte)0;"),
			IOKind.Console => SyntaxFactory.ParseStatement($"{arrayName}[{pointerName}] = (byte)Console.Read();"),
			_ => throw new ArgumentException($"Must have valid {nameof(IOKind)}", nameof(ioKind))
		};

		internal static SyntaxGenerator Output() => ioKind => ioKind switch
		{
			IOKind.Argument => SyntaxFactory.ParseStatement($"yield return {arrayName}[{pointerName}];"),
			IOKind.Console => SyntaxFactory.ParseStatement($"Console.Write((char){arrayName}[{pointerName}]);"),
			_ => throw new ArgumentException($"Must have valid {nameof(IOKind)}", nameof(ioKind))
		};

		internal static SyntaxGenerator Loop(IEnumerable<SyntaxGenerator> innerStatements)
		{
			return ioKind => SyntaxFactory.WhileStatement(SyntaxFactory.ParseExpression($"{arrayName}[{pointerName}] != (byte)0"), SyntaxFactory.Block(innerStatements.Select(g => g(ioKind))));
		}
	}
}