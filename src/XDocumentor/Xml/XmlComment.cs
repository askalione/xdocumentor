using System;
using System.Collections.Generic;
using System.Text;

namespace XDocumentor.Xml
{
    class XmlComment
    {
        public MemberType MemberType { get; set; }
        public string ClassName { get; set; }
        public string MemberName { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public string Returns { get; set; }

        public override string ToString()
        {
            return MemberType + ":" + ClassName + "." + MemberName;
        }
    }
}
