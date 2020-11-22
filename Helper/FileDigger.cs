using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var p = html.IndexOf("<p>");
            var slashP = html.IndexOf("</p>");
            return string.Concat(html.Substring(p + 3, slashP), "...");
        }
    }
}
