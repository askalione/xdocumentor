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
            var target = "Debug/Elibrary.Builder.dll";
            string dest = "Debug/md";
            MarkdownGenerateMode generateMode = MarkdownGenerateMode.FilePerClass;
            string namespaceMatch = string.Empty;
            if (args.Length == 1)
            {
                target = args[0];
            }
            else if (args.Length == 2)
            {
                target = args[0];
                dest = args[1];
            }
            else if (args.Length == 3)
            {
                target = args[0];
                dest = args[1];
                generateMode = Enum.Parse<MarkdownGenerateMode>(args[2]);
            }
            else if (args.Length == 4)
            {
                target = args[0];
                dest = args[1];
                generateMode = Enum.Parse<MarkdownGenerateMode>(args[2]);
                namespaceMatch = args[3];
            }

            var types = MarkdownGenerator.BuildFrom(target, namespaceMatch);

            if (types.Length == 0)
                return;

            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            else
            {
                var directory = new DirectoryInfo(dest);
                foreach (FileInfo file in directory.GetFiles()) file.Delete();
                foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            }

            var homeBuilder = new MarkdownBuilder();
            homeBuilder.Header(1, "Contents");
            homeBuilder.AppendLine();
            
            switch (generateMode)
            {
                case MarkdownGenerateMode.FilePerClass:
                    GenerateFilePerClass(homeBuilder, dest, types);
                    break;
                case MarkdownGenerateMode.FilePerNamespace:
                    GenerateFilePerNamespace(homeBuilder, dest, types);
                    break;
                default:
                    throw new Exception("Unknown MarkdownGenerateMode");
            }

            File.WriteAllText(Path.Combine(dest, "Home.md"), homeBuilder.ToString());
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
                    string renderedDisplayName = item.DisplayName.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "-");
                    var sb = new StringBuilder();
                    homeBuilder.ListLink(MarkdownHelper.RenderCode(item.DisplayName), g.Key + "/" + renderedDisplayName);
                    sb.Append(item.ToString());

                    File.WriteAllText(Path.Combine(namespacePath, renderedDisplayName + ".md"), sb.ToString());
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
                    homeBuilder.ListLink(MarkdownHelper.RenderCode(item.DisplayName), g.Key + "#" + item.DisplayName.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "-").ToLower());

                    sb.Append(item.ToString());
                }

                File.WriteAllText(Path.Combine(dest, g.Key + ".md"), sb.ToString());
                homeBuilder.AppendLine();
            }
        }
    }
}
