using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XDocumentor.Markdown
{
    static class AccessibilityLevelManager
    {
        public static BindingFlags GetMethodBindingFlags(AccessibilityLevel accessibilityLevel)
        {
            switch (accessibilityLevel)
            {
                case AccessibilityLevel.All:
                    return BaseBindingFlags() | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
                case AccessibilityLevel.PublicOnly:
                    return BaseBindingFlags() | BindingFlags.InvokeMethod;
                default:
                    throw new InvalidOperationException("Unknown AccessibilityLevel");
            }
        }
        
        public static BindingFlags GetPropertyBindingFlags(AccessibilityLevel accessibilityLevel)
        {
            switch (accessibilityLevel)
            {
                case AccessibilityLevel.All:
                    return BaseBindingFlags() | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty;
                case AccessibilityLevel.PublicOnly:
                    return BaseBindingFlags() | BindingFlags.GetProperty | BindingFlags.SetProperty;
                default:
                    throw new InvalidOperationException("Unknown AccessibilityLevel");
            }
        }

        public static BindingFlags GetFieldBindingFlags(AccessibilityLevel accessibilityLevel)
        {
            switch (accessibilityLevel)
            {
                case AccessibilityLevel.All:
                    return BaseBindingFlags() | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField;
                case AccessibilityLevel.PublicOnly:
                    return BaseBindingFlags() | BindingFlags.GetField | BindingFlags.SetField;
                default:
                    throw new InvalidOperationException("Unknown AccessibilityLevel");
            }
        }

        public static BindingFlags GetBindingFlags(AccessibilityLevel accessibilityLevel)
        {
            switch (accessibilityLevel)
            {
                case AccessibilityLevel.All:
                    return BaseBindingFlags() | BindingFlags.NonPublic;
                case AccessibilityLevel.PublicOnly:
                    return BaseBindingFlags();
                default:
                    throw new InvalidOperationException("Unknown AccessibilityLevel");
            }
        }

        private static BindingFlags BaseBindingFlags()
        {
            return BindingFlags.Public | BindingFlags.DeclaredOnly;
        }
    }
}
