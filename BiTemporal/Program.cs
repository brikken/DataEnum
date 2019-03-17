using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using trans = DataBiTemporal.Translators;
using render = DataBiTemporal.Renderers;

namespace BiTemporal
{
    class Program
    {
        /// <summary>
        /// BiTemporal.exe -d &lt;definition script&gt; -o &lt;output script&gt;
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var config = new Configuration();
            int argPointer = 0;
            string @break = null;
            while (argPointer < args.Length)
            {
                switch (args[argPointer])
                {
                    case "-d":
                        (config.Definition, argPointer, @break) = GetFileInfo(args, argPointer);
                        break;
                    case "-o":
                        (config.Definition, argPointer, @break) = GetFileInfo(args, argPointer);
                        break;
                    default:
                        @break = $"Unexpected parameter {args[argPointer]}";
                        break;
                }

                if (@break != null)
                {
                    Console.WriteLine($"{@break}");
                    return;
                }
            }

            switch (Attributes.Configuration.Validate(config))
            {
                case var resValidate when resValidate.valid:
                    switch (Process(config))
                    {
                        case var resProcess when resProcess.success:
                            break;
                        case var resProcess when !resProcess.success:
                            Console.WriteLine($"{resProcess.message}");
                            break;
                    }
                    break;
                case var result when !result.valid:
                    Console.WriteLine($"{result.message}");
                    return;
            }
        }

        static (bool success, string message) Process(Configuration config)
        {
            string definition;
            try
            {
                definition = File.ReadAllText(config.Definition.FullName);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case DirectoryNotFoundException _:
                    case IOException _:
                    case UnauthorizedAccessException _:
                        return (false, $"Could not read definition file ({e.Message})");
                    default:
                        throw;
                }
            }

            IList<DataBiTemporal.Definitions.BiTemporal> defs;
            try
            {
                defs = trans.BiTemporal.GetDefinitions(definition);
            }
            catch (trans.ParseErrorException e)
            {
                return (false, $"Syntax error in definition{Environment.NewLine}{e.Message}");
            }

            var output = string.Join(
                Environment.NewLine,
                defs.Select(def => render.BiTemporal.Render(def))
                );

            if (config.Output != null)
            {
                try
                {
                    File.WriteAllText(config.Output.FullName, output);
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case DirectoryNotFoundException _:
                        case IOException _:
                        case UnauthorizedAccessException _:
                            return (false, $"Error saving to output destination ({e.Message})");
                        default:
                            throw;
                    }
                }
            }
            else
            {
                Console.WriteLine(output);
            }

            return (true, null);
        }

        static (FileInfo fi, int argPointer, string @break) GetFileInfo(string[] args, int argPointer)
        {
            FileInfo fi = null;
            string @break = null;
            try
            {
                fi = new FileInfo(args[argPointer + 1]);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case NullReferenceException _:
                        @break = $"Expected path after {args[argPointer]}";
                        break;
                    case ArgumentException _:
                    case UnauthorizedAccessException _:
                    case PathTooLongException _:
                    case NotSupportedException _:
                        @break = $"Invalid path: {args[argPointer + 1]}";
                        break;
                    default:
                        throw;
                }
            }
            return (fi, argPointer + 2, @break);
        }
    }

    [Attributes.Configuration]
    public class Configuration
    {
        [Attributes.Required(MissingMessage = "Definition script must be specified")]
        public FileInfo Definition { get; set; }
        public FileInfo Output { get; set; }
    }
}

namespace BiTemporal.Attributes
{
    public class Configuration : Attribute {
        public static (bool valid, string message) Validate(object config)
        {
            foreach (var req in config.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<Required>() != null))
            {
                if (req.GetValue(config) == null)
                {
                    return (false, req.GetCustomAttribute<Required>().MissingMessage);
                }
            }
            return (true, null);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Required : Attribute {
        public string MissingMessage { get; set; }
    }
}