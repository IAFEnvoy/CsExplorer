using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialogs
{
    public partial class DeleteDialog : Form
    {
        public DeleteDialog()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设置文件名
        /// </summary>
        public static string Filename = "";
        /// <summary>
        /// 设置文件类型（空字符为不显示）
        /// </summary>
        public static string Filetype = "";
        /// <summary>
        /// 设置文件大小（空字符为不显示）
        /// </summary>
        public static string Filesize = "";
        /// <summary>
        /// 设置文件修改日期（空字符为不显示）
        /// </summary>
        public static string Filedate = "";
        /// <summary>
        /// 获取或设置图标
        /// </summary>
        public static Image FileIcon = Properties.Resource1.BlankFile;
        /// <summary>
        /// 附加信息（最多三条）
        /// </summary>
        public static string[] AddInformation=new string[5];
        public bool ReturnValue = false;

        private void DeleteDialog_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = FileIcon;
            string text = Filename;
            int cnt = 0;
            if (Filetype != "") { text += "\n项目类型：" + Filetype; cnt++; }
            if (Filesize != "") { text += "\n大小：" + Filesize; cnt++; }
            if (Filedate != "") { text += "\n修改日期：" + Filedate; cnt++; }
            foreach(string s in AddInformation)
            {
                if (cnt >= 3) break;
                if (s == "" || s == null) break;
                text += "\n" + s;
                cnt++;
            }
            label1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue = true;
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReturnValue = false;
            Hide();
        }
    }
}
