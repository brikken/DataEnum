using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace DataBiTemporal.Parser
{
    public class ParserHelpers
    {
        public static IParseTree GetRootContext(string code)
        {
            var ais = new AntlrInputStream(code);
            var lexer = new BiTempDefLexer(ais);
            var cts = new CommonTokenStream(lexer);
            var parser = new BiTempDefParser(cts);
            var rootContext = parser.compileUnit();
            return rootContext;
        }

        public static int WalkTree(string code)
        {
            var rootContext = GetRootContext(code);
            var walker = new ParseTreeWalker();
            var listener = new CountListener();
            walker.Walk(listener, rootContext);
            return listener.Counter;
        }

        class CountListener : BiTempDefBaseListener
        {
            public int Counter { get; private set; } = 0;
            public override void EnterEveryRule([NotNull] ParserRuleContext context)
            {
                base.EnterEveryRule(context);
                Counter++;
            }
        }

        public static string GetRaw(string input, ParserRuleContext context)
        {
            return input.Substring(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);
        }
    }
}
