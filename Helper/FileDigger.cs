using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace K1_Static_Website.Helper
{
    public static class FileDigger
    {
        internal static string GetPostTitle(string filename)
        {
            return filename.Substring(0, filename.Length - 3);
        }

        internal static string GetSummary(string html)
        {
            XmlDataDocument x = new XmlDataDocument();
            x.LoadXml(string.Concat("<div> ", html, "</div>"));
            var p = x.GetElementsByTagName("p");

            return string.Concat(p[0].InnerText, "...");
        }
    }
}
