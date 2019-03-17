using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using def = DataBiTemporal.Definitions;
using DataBiTemporal.Parser;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace DataBiTemporal.Translators
{
    public class BiTemporal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ParseErrorException">Thrown at lexing and parsing errors</exception>
        public static IList<def.BiTemporal> GetDefinitions(string input)
        {
            var ais = new AntlrInputStream(input);

            var lexer = new BiTempDefLexer(ais);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new BiTemporalErrorListener<int>());

            var cts = new CommonTokenStream(lexer);

            var parser = new BiTempDefParser(cts);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new BiTemporalErrorListener<IToken>());

            var root = parser.compileUnit();

            var listener = new BiTemporalListener(input);
            var ptw = new ParseTreeWalker();
            ptw.Walk(listener, root);

            foreach (var definition in listener.Definitions)
            {
                foreach (var col in definition.Columns)
                {
                    if (col.Options.Content.Count(opt => opt == def.ColumnOption.PRIMARY_KEY) > 0)
                    {
                        definition.PrimaryKey.Add(col);
                    }
                }
            }

            return listener.Definitions;
        }
    }

    public class ParseErrorException : Exception
    {
        public ParseErrorException(string message) : base(message) { }
    }

    class BiTemporalErrorListener<T> : IAntlrErrorListener<T>
    {
        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] T offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            throw new ParseErrorException($"Line {line}:{charPositionInLine} {msg}");
        }
    }

    class BiTemporalListener : BiTempDefBaseListener
    {
        public IList<def.BiTemporal> Definitions { get; set; } = new List<def.BiTemporal>();
        def.BiTemporal curDef;
        def.Column curCol;
        string input;

        public BiTemporalListener(string input)
        {
            this.input = input;
        }

        public override void EnterBtOptBtSchema([NotNull] BiTempDefParser.BtOptBtSchemaContext context)
        {
            curDef.BtSchema = def.ObjectId.FromDef(context.sch.Text);
        }

        public override void ExitColDef([NotNull] BiTempDefParser.ColDefContext context)
        {
            curDef.Columns.Add(curCol);
        }

        public override void EnterColDef([NotNull] BiTempDefParser.ColDefContext context)
        {
            curCol = new def.Column();
            (curCol.Id = new def.ObjectId()).Raw = context.col.Text;
            curCol.Options.Raw = ParserHelpers.GetRaw(input, context.opts);
        }

        public override void EnterColType([NotNull] BiTempDefParser.ColTypeContext context)
        {
            curCol.Type = context.GetText();
        }

        public override void EnterTableDef([NotNull] BiTempDefParser.TableDefContext context)
        {
            curDef = new def.BiTemporal();
            (curDef.Table = new def.ObjectId()).Raw = context.tab.Text;
        }

        public override void ExitTableDef([NotNull] BiTempDefParser.TableDefContext context)
        {
            Definitions.Add(curDef);
        }

        public override void EnterMultiPartIdSch([NotNull] BiTempDefParser.MultiPartIdSchContext context)
        {
            (curDef.Schema = new def.ObjectId()).Raw = context.sch.Text;
        }

        public override void EnterMultiPartIdDbSch([NotNull] BiTempDefParser.MultiPartIdDbSchContext context)
        {
            (curDef.Database = new def.ObjectId()).Raw = context.db.Text;
            (curDef.Schema = new def.ObjectId()).Raw = context.sch.Text;
        }

        public override void EnterColOptPrimaryKey([NotNull] BiTempDefParser.ColOptPrimaryKeyContext context)
        {
            curCol.Options.Content.Add(def.ColumnOption.PRIMARY_KEY);
        }

        public override void EnterColOptNull([NotNull] BiTempDefParser.ColOptNullContext context)
        {
            curCol.Options.Content.Add(def.ColumnOption.NULL);
        }

        public override void EnterColOptNotNull([NotNull] BiTempDefParser.ColOptNotNullContext context)
        {
            curCol.Options.Content.Add(def.ColumnOption.NOT_NULL);
        }
    }
}
