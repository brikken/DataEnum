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

namespace DataBiTemporal.Translators
{
    public class BiTemporal
    {
        public static ICollection<def.BiTemporal> GetDefinitions(string input)
        {
            var ais = new Antlr4.Runtime.AntlrInputStream(input);
            var lexer = new BiTempDefLexer(ais);
            var cts = new Antlr4.Runtime.CommonTokenStream(lexer);
            var parser = new BiTempDefParser(cts);
            var root = parser.compileUnit();

            var listener = new BiTemporalListener();
            var ptw = new ParseTreeWalker();
            ptw.Walk(listener, root);

            return listener.Definitions;
        }
    }

    class BiTemporalListener : BiTempDefBaseListener
    {
        public ICollection<def.BiTemporal> Definitions { get; set; } = new Collection<def.BiTemporal>();
        def.BiTemporal curDef;
        def.Column curCol;
        def.ITableOption curTabOpt;
        def.IDtWithOption curDtWithOpt;

        public override void EnterBtOptBtSchema([NotNull] BiTempDefParser.BtOptBtSchemaContext context)
        {
            (curDtWithOpt as def.BiTemporalOption).BtSchema.Raw = context.sch.Text;
        }

        public override void EnterDtWithOptBiTemporal([NotNull] BiTempDefParser.DtWithOptBiTemporalContext context)
        {
            curDtWithOpt = new def.BiTemporalOption();
        }

        public override void EnterTabOptDtWith([NotNull] BiTempDefParser.TabOptDtWithContext context)
        {
            curTabOpt = new def.DtWithOption();
        }

        public override void ExitColDef([NotNull] BiTempDefParser.ColDefContext context)
        {
            curDef.Columns.Add(curCol);
        }

        public override void EnterColDef([NotNull] BiTempDefParser.ColDefContext context)
        {
            curCol = new def.Column();
            curCol.Name.Raw = context.col.Text;
            curCol.Options = context._opts.Select(o => Option(o)) as ICollection<def.ColumnOption>;
        }

        private def.ColumnOption Option(BiTempDefParser.ColOptContext context)
        {
            switch (context)
            {
                case BiTempDefParser.ColOptNotNullContext _:
                    return def.ColumnOption.NOT_NULL;
                case BiTempDefParser.ColOptNullContext _:
                    return def.ColumnOption.NULL;
                case BiTempDefParser.ColOptPrimaryKeyContext _:
                    return def.ColumnOption.PRIMARY_KEY;
                default:
                    throw new ArgumentException($"Column option {context.GetText()} not recognized");
            }
        }

        public override void EnterColTypeDecimal([NotNull] BiTempDefParser.ColTypeDecimalContext context)
        {
            curCol.Type = def.Type.DECIMAL;
        }

        public override void EnterColTypeInt([NotNull] BiTempDefParser.ColTypeIntContext context)
        {
            curCol.Type = def.Type.INT;
        }

        public override void EnterColTypeVarchar([NotNull] BiTempDefParser.ColTypeVarcharContext context)
        {
            curCol.Type = def.Type.VARCHAR;
        }

        public override void EnterTableDef([NotNull] BiTempDefParser.TableDefContext context)
        {
            curDef = new def.BiTemporal();
            curDef.Database.Raw = context.db.Text;
            curDef.Schema.Raw = context.sch.Text;
            curDef.Table.Raw = context.tab.Text;
        }

        public override void ExitTableDef([NotNull] BiTempDefParser.TableDefContext context)
        {
            Definitions.Add(curDef);
        }

        public override void ExitDtWithOptBiTemporal([NotNull] BiTempDefParser.DtWithOptBiTemporalContext context)
        {
            (curTabOpt as def.DtWithOption).Options.Add(curDtWithOpt);
        }

        public override void ExitTabOptDtWith([NotNull] BiTempDefParser.TabOptDtWithContext context)
        {
            curDef.Options.Add(curTabOpt);
        }
    }
}
