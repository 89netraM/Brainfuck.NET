using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BrainfuckNET
{
	delegate StatementSyntax SyntaxGenerator(IOKind ioKind);
}