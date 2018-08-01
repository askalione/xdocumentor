using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XDocumentor.Xml;

namespace XDocumentor.Markdown
{
    class MarkdownableType
    {
        private readonly Type _type;
        private readonly ILookup<string, XmlComment> _commentLookup;
        private readonly AccessibilityLevel _accessibilityLevel; 

        public string Namespace => _type.Namespace;
        public string Name => _type.Name;
        public string DisplayName => MarkdownHelper.RenderType(_type);
        public string ProcessedName => DisplayName.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "-");

        public MarkdownableType(Type type, ILookup<string, XmlComment> commentLookup, AccessibilityLevel accessibilityLevel)
        {
            _type = type;
            _commentLookup = commentLookup;
            _accessibilityLevel = accessibilityLevel;
        }

        ConstructorInfo[] GetConstructors()
        {
            var test = _type.GetConstructors(AccessibilityLevelManager.GetMethodBindingFlags(_accessibilityLevel) | BindingFlags.Instance)
                .Where(x => !x.GetCustomAttributes<ObsoleteAttribute>().Any() && !x.IsPrivate)
                .ToArray();

            return test;
        }

        MethodInfo[] GetMethods()
        {
            return _type.GetMethods(AccessibilityLevelManager.GetMethodBindingFlags(_accessibilityLevel) | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any() && !x.IsPrivate)
                .ToArray();
        }

        PropertyInfo[] GetProperties()
        {
            return _type.GetProperties(AccessibilityLevelManager.GetPropertyBindingFlags(_accessibilityLevel) | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any())
                .Where(y =>
                {
                    var get = y.GetGetMethod(true);
                    var set = y.GetSetMethod(true);
                    if (get != null && set != null)
                    {
                        return !(get.IsPrivate && set.IsPrivate);
                    }
                    else if (get != null)
                    {
                        return !get.IsPrivate;
                    }
                    else if (set != null)
                    {
                        return !set.IsPrivate;
                    }
                    else
                    {
                        return false;
                    }
                })
                .ToArray();
        }

        FieldInfo[] GetFields()
        {
            return _type.GetFields(AccessibilityLevelManager.GetFieldBindingFlags(_accessibilityLevel) | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any() && !x.IsPrivate)
                .ToArray();
        }

        EventInfo[] GetEvents()
        {
            return _type.GetEvents(AccessibilityLevelManager.GetBindingFlags(_accessibilityLevel) | BindingFlags.Instance)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any())
                .ToArray();
        }

        FieldInfo[] GetStaticFields()
        {
            return _type.GetFields(AccessibilityLevelManager.GetFieldBindingFlags(_accessibilityLevel) | BindingFlags.Static)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any() && !x.IsPrivate)
                .ToArray();
        }

        PropertyInfo[] GetStaticProperties()
        {
            return _type.GetProperties(AccessibilityLevelManager.GetPropertyBindingFlags(_accessibilityLevel) | BindingFlags.Static)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any())
                .Where(y =>
                {
                    var get = y.GetGetMethod(true);
                    var set = y.GetSetMethod(true);
                    if (get != null && set != null)
                    {
                        return !(get.IsPrivate && set.IsPrivate);
                    }
                    else if (get != null)
                    {
                        return !get.IsPrivate;
                    }
                    else if (set != null)
                    {
                        return !set.IsPrivate;
                    }
                    else
                    {
                        return false;
                    }
                })
                .ToArray();
        }

        MethodInfo[] GetStaticMethods()
        {
            return _type.GetMethods(AccessibilityLevelManager.GetMethodBindingFlags(_accessibilityLevel) | BindingFlags.Static)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any() && !x.IsPrivate)
                .ToArray();
        }

        EventInfo[] GetStaticEvents()
        {
            return _type.GetEvents(AccessibilityLevelManager.GetBindingFlags(_accessibilityLevel) | BindingFlags.Static)
                .Where(x => !x.IsSpecialName && !x.GetCustomAttributes<ObsoleteAttribute>().Any())
                .ToArray();
        }

        void BuildDescription<T>(MarkdownBuilder mb, string label, T[] array, IEnumerable<XmlComment> docs, Func<T, string> type, Func<T, string> name, Func<T, string> finalName)
        {
            if (array.Any())
            {
                mb.Header(2, label);
                mb.AppendLine();

                string[] head = (this._type.IsEnum)
                    ? new[] { "Value", "Name", "Description" }
                    : new[] { "Type", "Name", "Description" };

                IEnumerable<T> seq = array;
                if (!this._type.IsEnum)
                {
                    seq = array.OrderBy(x => name(x));
                }

                var data = seq.Select(item2 =>
                {
                    var summary = docs.FirstOrDefault(x => x.MemberName == name(item2) || x.MemberName.StartsWith(name(item2) + "`"))?.Summary ?? "";
                    return new[] { MarkdownHelper.RenderCode(type(item2)), finalName(item2), summary };
                });

                mb.Table(head, data);
                mb.AppendLine();
                mb.AppendLine();
            }
        }
        
        void BuildFullDescription<T>(MarkdownBuilder mb, string label, T[] array, IEnumerable<XmlComment> docs, Func<T, string> type, Func<T, string> name, Func<T, string> finalName) where T : MethodBase
        {
            if (array.Any())
            {
                mb.Header(2, label);
                mb.AppendLine();

                foreach (T item in array)
                {
                    var doc = docs.FindComment(name(item), item.MemberType, item.GetParameters()?.Select(x => x.Name).ToArray());
                    var itemSummary = doc?.Summary;

                    mb.List(MarkdownHelper.RenderCode(finalName(item).Replace("`", "")));
                    mb.AppendLine();

                    if (!item.IsConstructor)
                    {
                        mb.AppendLine("   **Return type:** " + MarkdownHelper.RenderCode(type(item)));
                        mb.AppendLine();
                    }
                                        
                    if (!String.IsNullOrWhiteSpace(itemSummary))
                    {
                        mb.AppendLine("   " + itemSummary);
                        mb.AppendLine();
                    }

                    var parameters = item.GetParameters();
                    if (parameters.Any())
                    {
                        string[] head = (this._type.IsEnum)
                            ? new[] { "Value", "Name", "Description" }
                            : new[] { "Type", "Name", "Description" };

                        var data = parameters.Select(param =>
                        {
                            var paramSummary = doc?.Parameters[param.Name] ?? "";
                            return new[] { MarkdownHelper.RenderCode(MarkdownHelper.RenderType(param.ParameterType)), param.Name, paramSummary };
                        });

                        mb.Table(head, data, true);
                        mb.AppendLine();
                        mb.AppendLine();
                    }                    
                }
                
                mb.AppendLine();
            }
        }

        public override string ToString()
        {
            var mb = new MarkdownBuilder();

            mb.HeaderWithCode(1, MarkdownHelper.RenderType(_type, false));
            mb.AppendLine();

            var desc = _commentLookup[_type.FullName].FirstOrDefault(x => x.MemberType == MemberType.Type)?.Summary ?? "";
            if (desc != "")
            {
                mb.AppendLine(desc);
            }
            {
                var sb = new StringBuilder();

                var stat = (_type.IsAbstract && _type.IsSealed) ? "static " : "";
                var abst = (_type.IsAbstract && !_type.IsInterface && !_type.IsSealed) ? "abstract " : "";
                var classOrStructOrEnumOrInterface = _type.IsInterface ? "interface" : _type.IsEnum ? "enum" : _type.IsValueType ? "struct" : "class";

                sb.AppendLine($"public {stat}{abst}{classOrStructOrEnumOrInterface} {MarkdownHelper.RenderType(_type, true)}");
                var impl = string.Join(", ", new[] { _type.BaseType }.Concat(_type.GetInterfaces()).Where(x => x != null && x != typeof(object) && x != typeof(ValueType)).Select(x => MarkdownHelper.RenderType(x)));
                if (impl != "")
                {
                    sb.AppendLine("    : " + impl);
                }

                mb.Code("csharp", sb.ToString());
            }

            mb.AppendLine();

            if (_type.IsEnum)
            {
                var enums = Enum.GetNames(_type)
                    .Select(x => new { Name = x, Value = ((Int32)Enum.Parse(_type, x)) })
                    .OrderBy(x => x.Value)
                    .ToArray();

                BuildDescription(mb, "Enum", enums, _commentLookup[_type.FullName], x => x.Value.ToString(), x => x.Name, x => x.Name);
            }
            else
            {
                BuildFullDescription(mb, "Constructors", GetConstructors(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.DeclaringType), x => x.Name, x => MarkdownHelper.RenderMethodInfo(x));
                BuildDescription(mb, "Fields", GetFields(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.FieldType), x => x.Name, x => x.Name);
                BuildDescription(mb, "Properties", GetProperties(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.PropertyType), x => x.Name, x => x.Name);
                BuildDescription(mb, "Events", GetEvents(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.EventHandlerType), x => x.Name, x => x.Name);
                BuildFullDescription(mb, "Methods", GetMethods(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.ReturnType), x => x.Name, x => MarkdownHelper.RenderMethodInfo(x));
                BuildDescription(mb, "Static Fields", GetStaticFields(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.FieldType), x => x.Name, x => x.Name);
                BuildDescription(mb, "Static Properties", GetStaticProperties(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.PropertyType), x => x.Name, x => x.Name);
                BuildFullDescription(mb, "Static Methods", GetStaticMethods(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.ReturnType), x => x.Name, x => MarkdownHelper.RenderMethodInfo(x));
                BuildDescription(mb, "Static Events", GetStaticEvents(), _commentLookup[_type.FullName], x => MarkdownHelper.RenderType(x.EventHandlerType), x => x.Name, x => x.Name);
            }

            return mb.ToString();
        }
    }
}
