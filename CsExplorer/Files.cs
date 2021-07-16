using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsExplorer
{
    class Files
    {
        public static string Getminsize(long size)
        {
            double si = size;
            string[] s = { "B", "KB", "MB", "GB", "TB", "PB" };
            int o = 0;
            while (si >= 1024)
            {
                si /= 1024;
                o++;
            }
            return Math.Round(si, 2).ToString() + s[o];
        }
    }
}
