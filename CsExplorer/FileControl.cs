using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CsExplorer
{
    /// <summary>
    /// 文件操作
    /// </summary>
    public class FileControl
    {

       /// <summary>
        /// 看此目标下面是否有此文件
       /// </summary>
       /// <param name="fc">目录</param>
       /// <param name="CopyFile">要粘贴的文件类</param>
       /// <returns></returns>
        public static bool isExists(DirectoryClass fc,FileClass CopyFile)
        {
            bool isExist = false; //默认不存在此记录

            DirectoryInfo dirs = new DirectoryInfo(fc.FullName);
            if (dirs.GetDirectories().Where(i => i.Name == CopyFile.Name).Count() > 0)  //代表有记录
            {
                isExist = true;
            }

            return isExist;
        }


        /// <summary>
        /// 粘贴文件到文件夹中
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="CopyFile"></param>
        /// <returns></returns>
        public static bool PasterFile(DirectoryClass fc, FileClass CopyFile)
        {
            bool isPaster = false;  //文件粘贴不成功

            string destFileName = fc.FullName + "\\" + CopyFile.Name;
            try
            {
                File.Copy(CopyFile.FullName, destFileName);
                isPaster = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "文件粘贴错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return isPaster;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="CopyFile"></param>
        /// <returns></returns>
        public static bool DeleteFile(FileClass CopyFile)
        {
            bool isDelete = false;

            try
            {
                File.Delete(CopyFile.FullName);
                isDelete = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "删除文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isDelete;
        }

        /// <summary>
        /// 修改文件名称
        /// </summary>
        /// <param name="file">当前选择的文件</param>
        /// <param name="afterName">要修改的名称</param>
        /// <returns></returns>
        public static bool RenameFile(FileClass file, string afterName)
        {
            bool isRename = false;

            string[] oldFileName = file.FullName.Split('\\');

            string newFileName = string.Join("\\", oldFileName, 0, oldFileName.Length - 1) + "\\" + afterName;

            try
            {
                File.Move(file.FullName, newFileName);
                isRename = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "文件更名失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isRename;
        }
    }
}
