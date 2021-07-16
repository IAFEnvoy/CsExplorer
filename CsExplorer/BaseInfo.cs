using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsExplorer
{
    /// <summary>
    /// 公共信息类
    /// </summary>
    public class BaseInfo
    {
        private string name;

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string fullName;

        /// <summary>
        /// 文件路径（文件全名称)
        /// </summary>
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

 

       
        private string CreateTime;

        /// <summary>
        /// 修改日期
        /// </summary>
        public string CreateTime1
        {
            get { return CreateTime; }
            set { CreateTime = value; }
        }

        private bool isDirectory;

        /// <summary>
        /// 是不是文件夹
        /// </summary>
        public bool IsDirectory
        {
            get { return isDirectory; }
            set { isDirectory = value; }
        }

    }
}
