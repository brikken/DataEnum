using DataEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tds = TableDefinition.FromFile(@"table-def02.json");
            foreach (var td in tds)
            {
                Console.WriteLine(td.GetSQL());
            }
            while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
        }
    }
}
