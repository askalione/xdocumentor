using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XDocumentor.Xml
{
    static class Extensions
    {
        public static XmlComment FindComment(this IEnumerable<XmlComment> docs, string memberName, MemberTypes memberType, string[] paramNames)
        {
            return docs.FirstOrDefault(x => IsComment(x, memberName, memberType, paramNames));
        }

        static bool IsComment(XmlComment comment, string memberName, MemberTypes memberType, string[] paramNames)
        {
            return HasEqualMemberName(comment.MemberName, memberName) &&
                HasEqualMemberType(comment.MemberType, memberType) &&
                HasEqualParameters(comment.Parameters.Select(x => x.Key).ToArray(), paramNames);
        }

        static bool HasEqualMemberName(string commentMemberName, string memberName)
        {
            return commentMemberName == memberName || commentMemberName.StartsWith(memberName + "`");
        }

        static bool HasEqualMemberType(MemberType commentMemberType, MemberTypes memberType)
        {
            return commentMemberType.ToString().Equals(memberType.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        static bool HasEqualParameters(string[] commentParamNames, string[] paramNames)
        {
            return commentParamNames.SequenceEqual(paramNames);
        }
    }
}
