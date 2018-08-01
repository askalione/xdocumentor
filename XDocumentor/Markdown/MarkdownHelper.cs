using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace XDocumentor.Markdown
{
    static class MarkdownHelper
    {
        public static string RenderType(Type t, bool isFull = false)
        {
            if (t == null) return "";
            if (t == typeof(void)) return "void";
            if (!t.IsGenericType) return (isFull) ? t.FullName : t.Name;

            var innerFormat = string.Join(", ", t.GetGenericArguments().Select(x => RenderType(x)));
            return Regex.Replace(isFull ? t.GetGenericTypeDefinition().FullName : t.GetGenericTypeDefinition().Name, @"`.+$", "") + "<" + innerFormat + ">";
        }

        public static string RenderMethodInfo(MethodBase methodInfo)
        {
            var isExtension = methodInfo.GetCustomAttributes<System.Runtime.CompilerServices.ExtensionAttribute>(false).Any();

            var seq = methodInfo.GetParameters().Select(x =>
            {
                var suffix = x.HasDefaultValue ? (" = " + (x.DefaultValue ?? $"null")) : "";
                return "`" + RenderType(x.ParameterType) + "` " + x.Name + suffix;
            });

            return (methodInfo.IsConstructor ? methodInfo.DeclaringType.Name : methodInfo.Name) + "(" + (isExtension ? "this " : "") + string.Join(", ", seq) + ")";
        }

        public static string RenderCode(string code)
        {
            return "`" + code + "`";
        }
    }
}
