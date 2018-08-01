using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using XDocumentor.Markdown;

namespace XDocumentor
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerationOptions options = new GenerationOptions(args);

            var types = MarkdownGenerator.BuildFrom(options.Target, options.NamespaceMatch, options.AccessibilityLevel);

            if (types.Length == 0)
                return;

            if (!Directory.Exists(options.Output)) Directory.CreateDirectory(options.Output);
            else
            {
                DirectoryInfo directory = new DirectoryInfo(options.Output);
                foreach (FileInfo file in directory.GetFiles()) file.Delete();
                foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }

            var homeBuilder = new MarkdownBuilder();
            homeBuilder.Header(1, "Contents");
            homeBuilder.AppendLine();
            
            switch (options.Mode)
            {
                case GenerationMode.FilePerClass:
                    GenerateFilePerClass(homeBuilder, options.Output, types);
                    break;
                case GenerationMode.FilePerNamespace:
                    GenerateFilePerNamespace(homeBuilder, options.Output, types);
                    break;
                default:
                    throw new Exception("Unknown generation mode");
            }

            File.WriteAllText(Path.Combine(options.Output, "Home.md"), homeBuilder.ToString());
        }

        private static void GenerateFilePerClass(MarkdownBuilder homeBuilder, string dest, MarkdownableType[] types)
        {
            foreach (var g in types.GroupBy(x => x.Namespace).OrderBy(x => x.Key))
            {
                string namespacePath = Path.Combine(dest, g.Key);

                homeBuilder.Header(2, g.Key);
                homeBuilder.AppendLine();

                if (!Directory.Exists(namespacePath)) Directory.CreateDirectory(namespacePath);
                
                foreach (var item in g.OrderBy(x => x.Name))
                {
                    var sb = new StringBuilder();
                    homeBuilder.ListLink(MarkdownHelper.RenderCode(item.DisplayName), g.Key + "/" + item.ProcessedName);
                    sb.Append(item.ToString());

                    File.WriteAllText(Path.Combine(namespacePath, item.ProcessedName + ".md"), sb.ToString());
                }
                                
                homeBuilder.AppendLine();
            }
        }

        private static void GenerateFilePerNamespace(MarkdownBuilder homeBuilder, string dest, MarkdownableType[] types)
        {
            foreach (var g in types.GroupBy(x => x.Namespace).OrderBy(x => x.Key))
            {
                homeBuilder.HeaderWithLink(2, g.Key, g.Key);
                homeBuilder.AppendLine();

                var sb = new StringBuilder();
                foreach (var item in g.OrderBy(x => x.Name))
                {
                    homeBuilder.ListLink(MarkdownHelper.RenderCode(item.DisplayName), g.Key + "#" + item.ProcessedName.ToLower());

                    sb.Append(item.ToString());
                }

                File.WriteAllText(Path.Combine(dest, g.Key + ".md"), sb.ToString());
                homeBuilder.AppendLine();
            }
        }
    }
}
