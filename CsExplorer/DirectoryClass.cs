using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsExplorer
{
    /// <summary>
    /// 文件夹类
    /// </summary>
    public class DirectoryClass:BaseInfo
    {
        private bool isFixDriver = false;

        /// <summary>
        /// 是不是固定磁盘,如 C:  D:  E:
        /// </summary>
        public bool IsFixDriver
        {
            get { return isFixDriver; }
            set { isFixDriver = value; }
        }

        
        private double maxLength;

        /// <summary>
        /// 总大小
        /// </summary>
        public double MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; }
        }

        private double freePrice;

        /// <summary>
        /// 可用空间
        /// </summary>
        public double FreePrice
        {
            get { return freePrice; }
            set { freePrice = value; }
        }
    }
}
