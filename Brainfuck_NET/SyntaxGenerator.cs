using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Brainfuck_NET
{
	delegate StatementSyntax SyntaxGenerator(IOKind ioKind);
}