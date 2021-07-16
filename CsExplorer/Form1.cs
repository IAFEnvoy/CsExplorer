using BinaryObject;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CsExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string startpath;
        List<string> history = new List<string>();
        bool showhide;
        private int Getfileicon(string filename)
        {
            string[] s = filename.Split('.');
            if (s.Length == 1) return 1;
            string houzhuiming=string.Empty;
            foreach(string s1 in s) houzhuiming = s1;
            for(int i = 0; i < Icon.Images.Count; i++)
            {
                if (Icon.Images.Keys[i] == houzhuiming) return i;
            }
            string fileName = "tmp." + houzhuiming;
            File.Create(fileName).Close();
            Image img = System.Drawing.Icon.ExtractAssociatedIcon(fileName).ToBitmap();
            File.Delete(fileName);
            Icon.Images.Add(houzhuiming, img);
            return Icon.Images.Count - 1;
        }
        private ListViewItem SetFile(FileInfo file)
        {
            Color c = Color.Black;
            if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                c = Color.Gray;
            ListViewItem l = new ListViewItem(file.Name, Getfileicon(file.FullName));
            l.SubItems.Add(file.LastWriteTime.ToString());
            l.SubItems.Add(file.Extension);
            l.SubItems.Add(Files.Getminsize(file.Length));
            l.ForeColor = c;
            return l;
        }
        private ListViewItem SetDir(DirectoryInfo dir)
        {
            Color c = Color.Black;
            if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                c = Color.Gray;
            ListViewItem l = new ListViewItem(dir.Name, 0);
            l.ForeColor = c;
            return l;
        }
        private void Setlist(string path)
        {
            listView1.Items.Clear();
            try
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                DirectoryInfo[] dir = directory.GetDirectories();
                FileInfo[] fil = directory.GetFiles();
                foreach (DirectoryInfo d in dir)
                {
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden||showhide==true)
                        listView1.Items.Add(SetDir(d));
                }
                foreach (FileInfo f in fil)
                {
                    if ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden || showhide == true)
                        listView1.Items.Add(SetFile(f));
                }
            }
            catch(Exception e)
            {
                if (e.Message.Contains("访问被拒绝"))
                {
                    string message = "无法访问" + e.Message.Replace("对", "").Replace("的访问被拒绝", "") + "\n\n拒绝访问。";
                    MessageBox.Show(message, "位置不可用", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    pictureBox1_Click(new object(), new EventArgs());
                }
                else MessageBox.Show(e.Message);
            }
        }
        private bool Istrueorfalse(string s)
        {
            if (s == "false") return false;
            else return true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(Application.StartupPath + @"\Appconfig.xml", settings);
            xmlDoc.Load(reader);
            XmlNode xn = xmlDoc.SelectSingleNode("Explorer");
            XmlNodeList xnl = xn.ChildNodes;

            XmlElement xe = (XmlElement)xnl[0];
            XmlNodeList xnl0 = xe.ChildNodes;
            startpath= xnl0.Item(0).InnerText;

            xe = (XmlElement)xnl[1];
            xnl0 = xe.ChildNodes;
            showhide = Istrueorfalse(xnl0.Item(0).InnerText);

            xe = (XmlElement)xnl[2];
            xnl0 = xe.ChildNodes;
            SetView(xnl0.Item(0).InnerText);

            xe = (XmlElement)xnl[3];
            xnl0 = xe.ChildNodes;
            byte[] b= Convert.FromBase64String(xnl0.Item(0).InnerText);
            Font font = (Font)Binary.DeserializeBinary(b);
            menuStrip1.Font = font;
            textBox1.Font = font;
            treeView1.Font = font;
            listView1.Font = font;
            listuse.Font = font;

            reader.Close();

            Form1_SizeChanged(sender, e);
            Setlist(startpath);
            textBox1.Text = startpath;
            history.Add(textBox1.Text);
            FillTreeView();
        }
        #region  填充左边树型的菜单

        /// <summary>
        /// 填充左边树形菜单（只填两级)
        /// </summary>
        private void FillTreeView()
        {
            //此电脑
            #region 添加此电脑

            DirectoryClass directoryInfo = new DirectoryClass();
            directoryInfo.FullName = startpath;

            DirectoryClass myCom = new DirectoryClass();
            myCom.Name = "此电脑";
            myCom.IsDirectory = false;
            myCom.IsFixDriver = true;

            TreeNode myComputer = new TreeNode(myCom.Name, 0, 0);
            myComputer.Tag = myCom;
            #endregion

            //添加磁盘
            GetComputerDriver(myComputer);
            treeView1.Nodes.Add(myComputer);

        }
        /// <summary>
        /// 添加磁盘
        /// </summary>
        /// <param name="myComputer"></param>
        private void GetComputerDriver(TreeNode myComputer)
        {
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                if (di.DriveType == DriveType.Fixed)  //只添加固定磁盘,如:C
                {
                    DirectoryClass cdInfo = new DirectoryClass();
                    cdInfo.Name = di.Name;
                    cdInfo.FullName = di.Name;
                    cdInfo.IsDirectory = true;
                    cdInfo.IsFixDriver = true;

                    TreeNode cdNode = new TreeNode(string.Format("{0}({1})", di.VolumeLabel, di.Name),1,1);
                    cdNode.Tag = cdInfo;

                    //加载一级目录
                    GetDirectory(cdInfo, cdNode);

                    myComputer.Nodes.Add(cdNode);
                }
            }
        }
        /// <summary>
        /// 加载一级目录,并把文件和文件夹加到树形菜单里面去
        /// </summary>
        /// <param name="info">类名</param>
        /// <param name="pNode">父节点</param>
        private void GetDirectory(DirectoryClass info, TreeNode pNode)
        {
            pNode.Nodes.Clear(); //清空下面所有的记录

            DirectoryInfo dirInfo = new DirectoryInfo(info.FullName);

            //加载到树型菜单中
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                #region 加载文件夹信息

                string[] fileAttrites = File.GetAttributes(dir.FullName).ToString().Split(',');
                //如果属性大于0的就说明有问题,只要属性里面有hidden就不要显示出来

                bool isHideen = false; //是不是隐藏文件

                foreach (string cAttrites in fileAttrites)
                {
                    if (cAttrites.Equals("hidden", StringComparison.InvariantCultureIgnoreCase) || cAttrites.Equals("readonly", StringComparison.InvariantCultureIgnoreCase))
                    {
                        isHideen = true;
                        break;
                    }
                }

                #region  如果不是隐藏文件夹，就显示出来

                if (!isHideen)
                {
                    DirectoryClass childDirectory = new DirectoryClass();
                    childDirectory.Name = dir.Name;
                    childDirectory.FullName = dir.FullName;
                    childDirectory.IsDirectory = true;
                    childDirectory.CreateTime1 = dir.LastWriteTime.ToString("yyyy/MM/dd hh:MM");

                    TreeNode childNode = new TreeNode(childDirectory.Name,2,3);
                    childNode.Tag = childDirectory;

                    GetSecondDirectory(childDirectory, childNode);
                    pNode.Nodes.Add(childNode);
                }

                #endregion

                #endregion
            }
        }
        /// <summary>
        /// 加载二级菜单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pNode"></param>
        private void GetSecondDirectory(DirectoryClass info, TreeNode pNode)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(info.FullName);

            //加载到树型菜单中,只判断是否有子文件夹，如果有，就显示一个+号
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                TreeNode newNode = new TreeNode();
                pNode.Nodes.Add(newNode);
            }
        }
        #endregion

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox2.Location = new Point(Width - pictureBox2.Width - 25, pictureBox2.Location.Y);
            textBox1.Size = new Size(Width - 80, textBox1.Height);
            splitContainer1.Size = new Size(Width - 30, Height - 90);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.SelectedItems[0].ImageIndex == 0)//为文件夹
                {
                    textBox1.Text += listView1.SelectedItems[0].Text + @"\";
                    Setlist(textBox1.Text);
                    history.Add(textBox1.Text);
                }
                else if (listView1.SelectedItems[0].ImageIndex == 2)//为驱动器
                {

                    textBox1.Text = (string)listView1.SelectedItems[0].Tag;
                    Setlist(textBox1.Text);
                    history.Add(textBox1.Text);
                }
                else//为文件
                {
                    Process.Start(textBox1.Text + listView1.SelectedItems[0].Text);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Setlist(textBox1.Text);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (history.Count > 1)
            {
                history.RemoveAt(history.Count - 1);
                if(history[history.Count - 1] == "此电脑") SetDri();
                else Setlist(history[history.Count - 1]);
                textBox1.Text = history[history.Count - 1];
            }
            
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            //得到文件夹
            DirectoryClass getDirectory = (DirectoryClass)e.Node.Tag; //把节点转换为DirecoryClass

            if (!getDirectory.IsFixDriver)  //如果不是固定磁盘, 如，我的电脑这样就不行
            {
                //再得到下面两级的资料  
                GetDirectory(getDirectory, e.Node);
            }
        }
        void SetDri()
        {
            listView1.Items.Clear();
            foreach(DriveInfo dri in DriveInfo.GetDrives())
            {
                if (dri.IsReady)
                {
                    ListViewItem l = new ListViewItem(dri.VolumeLabel+"("+dri.Name+")", 2);
                    l.SubItems.Add(dri.DriveFormat);
                    l.SubItems.Add(Files.Getminsize(dri.AvailableFreeSpace));
                    l.SubItems.Add(Files.Getminsize(dri.TotalSize));
                    l.Tag = dri.Name;
                    listView1.Items.Add(l);
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DirectoryClass dc = (DirectoryClass)treeView1.SelectedNode.Tag;
            if (dc.FullName == null)
            {
                SetDri();
                textBox1.Text = "此电脑";
                history.Add("此电脑");
            }
            else
            {
                Setlist(dc.FullName);
                textBox1.Text = dc.FullName;
                history.Add(textBox1.Text);
            }
        }
        private void SetView(string type)
        {
            toolStripMenuItem1.Checked = false;
            toolStripMenuItem2.Checked = false;
            toolStripMenuItem3.Checked = false;
            toolStripMenuItem4.Checked = false;
            toolStripMenuItem5.Checked = false;
            switch (type)
            {
                case "largeicon":{
                        listView1.View = View.LargeIcon;
                        toolStripMenuItem1.Checked = true;
                        break;
                    }
                case "smallicon":
                    {
                        listView1.View = View.SmallIcon;
                        toolStripMenuItem2.Checked = true;
                        break;
                    }
                case "tile":
                    {
                        listView1.View = View.Tile;
                        toolStripMenuItem4.Checked = true;
                        break;
                    }
                case "details":
                    {
                        listView1.View = View.Details;
                        toolStripMenuItem5.Checked = true;
                        break;
                    }
                case "list":
                    {
                        listView1.View = View.List;
                        toolStripMenuItem3.Checked = true;
                        break;
                    }
                default: { MessageBox.Show("???");break; }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetView("largeicon");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetView("smallicon");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetView("list");
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetView("tile");
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SetView("details");
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            listView1.LabelEdit = true;
            listView1.SelectedItems[0].BeginEdit();
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                string aftername = e.Label;
                Computer MyComputer = new Computer();
                MyComputer.FileSystem.RenameFile(textBox1.Text + @"\" + listView1.SelectedItems[0].Text, aftername);
                Setlist(textBox1.Text);//刷新
            }
            catch { }
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            if (fd.ShowDialog() == DialogResult.Cancel) return;
            Font font = fd.Font;
            menuStrip1.Font = font;
            textBox1.Font = font;
            treeView1.Font = font;
            listView1.Font = font;
            listuse.Font = font;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xmlRoot = xmlDocument.CreateElement("Explorer");
            xmlDocument.AppendChild(xmlRoot);

            XmlComment xmlComment = xmlDocument.CreateComment("启动路径");
            XmlElement xmlChild = xmlDocument.CreateElement("Startuppath");
            xmlChild.AppendChild(xmlComment);
            XmlElement data = xmlDocument.CreateElement("Value");
            data.InnerText= @"C:\";
            xmlChild.AppendChild(data);
            xmlRoot.AppendChild(xmlChild);

            xmlComment = xmlDocument.CreateComment("隐藏项是否显示");
            xmlChild = xmlDocument.CreateElement("showhide");
            xmlChild.AppendChild(xmlComment);
            data = xmlDocument.CreateElement("Value");
            data.InnerText = "false";
            xmlChild.AppendChild(data);
            xmlRoot.AppendChild(xmlChild);

            xmlComment = xmlDocument.CreateComment("启动时的显示方法");
            xmlChild = xmlDocument.CreateElement("starttype");
            xmlChild.AppendChild(xmlComment);
            data = xmlDocument.CreateElement("Value");
            data.InnerText = "details";
            xmlChild.AppendChild(data);
            xmlRoot.AppendChild(xmlChild);

            xmlComment = xmlDocument.CreateComment("字体");
            xmlChild = xmlDocument.CreateElement("Font");
            xmlChild.AppendChild(xmlComment);
            data = xmlDocument.CreateElement("Value");
            data.InnerText = Convert.ToBase64String(Binary.SerializeBinary(listView1.Font));
            xmlChild.AppendChild(data);
            xmlRoot.AppendChild(xmlChild);

            xmlDocument.Save("Appconfig.xml");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "此电脑")
            {
                columnHeader1.Text = "驱动器";
                columnHeader2.Text = "文件系统";
                columnHeader3.Text = "剩余空间";
                columnHeader4.Text = "总空间";
            }
            else
            {
                columnHeader1.Text = "文件名";
                columnHeader2.Text = "修改日期";
                columnHeader3.Text = "类型";
                columnHeader4.Text = "大小";
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            /*
            Form delete = new DeleteDialog();
            if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.SelectedItems[0].ImageIndex == 0)//为文件夹
                {
                    DirectoryInfo dir = new DirectoryInfo(textBox1.Text + listView1.SelectedItems[0].Text + @"\");
                    delete. = Icon.Images[0];
                }
                else if (listView1.SelectedItems[0].ImageIndex == 2)//为驱动器
                {

                }
                else//为文件
                {
                    FileInfo file = new FileInfo(textBox1.Text + listView1.SelectedItems[0].Text + @"\");

                }
            }
            */
        }
    }
}
