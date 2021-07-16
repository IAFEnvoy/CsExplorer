using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsExplorer
{
   
    /// <summary>
    /// 文件夹
    /// </summary>
    public class FileClass:BaseInfo
    {
        private string fileExtends;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtends
        {
            get { return fileExtends; }
            set { fileExtends = value; }
        }


        private string fileExtendsName;

        /// <summary>
        /// 文件扩展的名称叫什么
        /// </summary>
        public string FileExtendsName
        {
            get { return fileExtendsName; }
            set { fileExtendsName = value; }
        }

        private double fileMax = 0;

        /// <summary>
        /// 文件大小
        /// </summary>
        public double FileMax
        {
            get { return fileMax; }
            set { fileMax = value; }
        }




    }
}
