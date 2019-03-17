using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using def = DataBiTemporal.Definitions;
using st = Antlr4.StringTemplate;

namespace DataBiTemporal.Renderers
{
    public class BiTemporal
    {
        const string templateDirectory = "Templates";
        const string templateFilename = "BiTemporal.stg";

        public static string Render(def.BiTemporal definition)
        {
            return Render(definition, "create");
        }

        public static string Render(def.BiTemporal definition, string templateName)
        {
            var templateDI = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            var templateFI = new FileInfo(Path.Combine(templateDI.FullName, templateDirectory, templateFilename));

            var stg = new st.TemplateGroupFile(templateFI.FullName);
            var template = stg.GetInstanceOf(templateName);
            template.Add("def", definition);

            return template.Render();
        }
    }
}
