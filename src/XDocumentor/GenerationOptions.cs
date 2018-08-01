using System;
using System.Collections.Generic;
using System.Text;

namespace XDocumentor
{
    class GenerationOptions
    {
        public string Target { get; }
        public string Output { get; } = "md";
        public AccessibilityLevel AccessibilityLevel { get; } = AccessibilityLevel.All;
        public GenerationMode Mode { get; } = GenerationMode.FilePerClass;        
        public string NamespaceMatch { get; } = String.Empty;

        public GenerationOptions(string[] args)
        {
#if DEBUG
            Target = "Debug/Elibrary.Builder.dll";
            Output = "Debug/md";
            AccessibilityLevel = AccessibilityLevel.PublicOnly;
            Mode = GenerationMode.FilePerClass;
            NamespaceMatch = String.Empty;
#else
            if (args == null || args.Length < 2)
                throw new InvalidOperationException("Invalid arguments");

            Target = args[0].Trim();
            Output = args[1].Trim();
            if (args.Length >= 3)
                AccessibilityLevel = Enum.Parse<AccessibilityLevel>(args[2]);
            if (args.Length >= 4)
                Mode = Enum.Parse<GenerationMode>(args[3]);
            if (args.Length >= 5)
                NamespaceMatch = String.Empty;
#endif
        }
    }
}
