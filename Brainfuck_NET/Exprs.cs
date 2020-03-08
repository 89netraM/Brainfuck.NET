using Microsoft.CodeAnalysis.CSharp;
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

		private static readonly IEnumerable<string> usings = new[]
		{
			"System",
			"System.Collections.Generic"
		};

		internal static NamespaceDeclarationSyntax Namespace(string namespaceName)
		{
			NamespaceDeclarationSyntax @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));
			@namespace = @namespace.WithUsings(SyntaxFactory.List(usings.Select(n => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(n)))));

			return @namespace;
		}

		internal static ClassDeclarationSyntax Class(string className)
		{
			ClassDeclarationSyntax @class = SyntaxFactory.ClassDeclaration(className);
			@class = @class.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

			return @class;
		}

		internal static MethodDeclarationSyntax Method(string methodName, IOKind ioKind, IEnumerable<SyntaxGenerator> innerStatements)
		{
			TypeSyntax returnType = ioKind switch
			{
				IOKind.Argument => SyntaxFactory.ParseTypeName("IEnumerable<byte>"),
				IOKind.Console => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
				_ => throw new ArgumentException($"Must have valid {nameof(IOKind)}", nameof(ioKind))
			};

			MethodDeclarationSyntax method = SyntaxFactory.MethodDeclaration(returnType, methodName);
			method = method.WithBody(SyntaxFactory.Block(Enumerable.Concat(MethodInit(ioKind), innerStatements.Select(g => g(ioKind)))));
			method = method.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

			return method;
		}

		private static IEnumerable<StatementSyntax> MethodInit(IOKind ioKind)
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