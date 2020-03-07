using Prat;
using static Prat.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Brainfuck_NET
{
	static class Parsing
	{
		private static readonly IParser<SyntaxGenerator> shiftParser = (Common.Char('<') | Common.Char('>')).OnceOrMore().Select(cs => Exprs.Shift(cs.Sum(c => c == '<' ? -1 : 1)));
		private static readonly IParser<SyntaxGenerator> incrementParser = (Common.Char('-') | Common.Char('+')).OnceOrMore().Select(cs => Exprs.Increment(cs.Sum(c => c == '-' ? -1 : 1)));
		private static readonly IParser<SyntaxGenerator> outputParser = Common.Char('.').Select(_ => Exprs.Output());
		private static readonly IParser<SyntaxGenerator> inputParser = Common.Char(',').Select(_ => Exprs.Input());

		private static readonly IParser<SyntaxGenerator> loopParser = KeepLeft(KeepRight(Common.Char('['), ZeroOrMore(() => statementParser)), Common.Char(']')).Select(xs => Exprs.Loop(xs));
		private static readonly IParser<SyntaxGenerator> statementParser = shiftParser | incrementParser | outputParser | inputParser | loopParser;
		private static readonly IParser<IEnumerable<SyntaxGenerator>> parser = statementParser.OnceOrMore();
		
		internal static ParsingResult Parse(string source) => parser.Parse(Regex.Replace(source, @"", "")) switch
		{
			(IEnumerable<SyntaxGenerator> xs, "") => new ParsingResult(true, true, xs),
			(IEnumerable<SyntaxGenerator> xs, _) => new ParsingResult(true, false, xs),
			null => new ParsingResult(false, false, Enumerable.Empty<SyntaxGenerator>())
		};
	}

	readonly struct ParsingResult
	{
		internal readonly bool Success;
		internal readonly bool ParsedAll;
		internal readonly IEnumerable<SyntaxGenerator> SyntaxGenerators;

		public ParsingResult(bool success, bool parsedAll, IEnumerable<SyntaxGenerator> syntaxGenerators)
		{
			Success = success;
			ParsedAll = parsedAll;
			SyntaxGenerators = syntaxGenerators ?? throw new ArgumentNullException(nameof(syntaxGenerators));
		}
	}
}