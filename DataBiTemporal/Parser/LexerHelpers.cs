using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace DataBiTemporal.Parser
{
    public class LexerHelpers
    {
        public static int GetTokenCount(string code)
        {
            return GetTokens(code).Count;
        }

        public static IList<IToken> GetTokens(string code)
        {
            var ais = new AntlrInputStream(code);
            var lexer = new BiTempDefLexer(ais);
            return lexer.GetAllTokens();
        }

        public static IList<int> GetChannels(string code)
        {
            return GetTokens(code).Select(token => token.Channel).Distinct().ToList();
        }
    }
}
