﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace DotNet.Utilities
{
    /// <summary>
    ///	FileUtil
    /// 文件帮助类
    /// 
    /// 修改记录
    /// 
    ///		2015.03.22 版本：1.4    JiRiGaLa 异常数据记录更多信息。
    ///		2012.05.03 版本：1.3    Pcsky增加一个读取文本文件内容的方法(GetTextFileContent)
    ///		2011.07.31 版本：1.2    Sunplay增加一个删除文件的方法(DeleteFile)。
    ///		2011.07.31 版本：1.1    Sunplay增加一个获取文件大小的方法(GetFileSize)。
    ///		2010.07.10 版本：1.0	JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.03.22</date>
    /// </author> 
    /// </summary>
    public class FileUtil
    {
        // 播放音乐
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);

        public const int SND_FILENAME = 0x00020000;

        public const int SND_ASYNC = 0x0001;

        public static void PlaySound(string file)
        {
            if (File.Exists(file))
            {
                PlaySound(file, 0, SND_ASYNC | SND_FILENAME);
            }
        }

        public static bool ThumbnailCallback() { return false; }

        /// <summary>
        /// 获取压缩的图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="width">宽度</param>
        /// <returns>自动压缩后的图片</returns>
        public static Bitmap GetThumbnailImageFromFile(string fileName, int maxHeightWidth = 0)
        {
            Image image = Image.FromFile(fileName);
            int height = image.Height;
            int width = image.Width;
            if (maxHeightWidth != 0)
            {
                if (image.Width >= image.Height)
                {
                    width = maxHeightWidth;
                    height = (maxHeightWidth * image.Height) / image.Width;
                }
                else
                {
                    height = maxHeightWidth;
                    width = (maxHeightWidth * image.Width) / image.Height;
                }
            }
            return new Bitmap(image.GetThumbnailImage(width, height, ThumbnailCallback, IntPtr.Zero));
        }

        #region public static string GetFriendlyFileSize(double fileSize) 有善的文件大小现实方式
        /// <summary>
        /// 有善的文件大小现实方式
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns>现实方式</returns>
        public static string GetFriendlyFileSize(double fileSize)
        {
            string result = string.Empty;
            if (fileSize < 1024)
            {
                result = fileSize.ToString("F1") + "Byte";
            }
            else
            {
                fileSize = fileSize / 1024;
                if (fileSize < 1024)
                {
                    result = fileSize.ToString("F1") + "KB";
                }
                else
                {
                    fileSize = fileSize / 1024;
                    if (fileSize < 1024)
                    {
                        result = fileSize.ToString("F1") + "M";
                    }
                    else
                    {
                        fileSize = fileSize / 1024;
                        result = fileSize.ToString("F1") + "GB";
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>字节</returns>
        public static byte[] GetFile(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            byte[] file = binaryReader.ReadBytes(((int)fileStream.Length));
            binaryReader.Close();
            fileStream.Close();
            return file;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="File">文件</param>
        /// <param name="fileName">文件名</param>
        public static void SaveFile(byte[] file, string fileName)
        {
            string directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            fileStream.Write(file, 0, file.Length);
            fileStream.Close();
        }

        public static byte[] ImageToByte(Image Image)
        {
            MemoryStream memoryStream = new MemoryStream();
            Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
            byte[] file = memoryStream.GetBuffer();
            memoryStream.Close();
            return file;
        }

        public static Image ByteToImage(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = new System.IO.MemoryStream(buffer);
            Image image = Image.FromStream(memoryStream);
            memoryStream.Close();
            return image;
        }

        public string FileName = "Log.txt";

        #region public static void WriteException(Exception ex, string fileName = null) 写入异常情况
        /// <summary>
        /// 写入异常情况
        /// </summary>
        /// <param name="ex">异常</param>
        public static void WriteException(Exception ex, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = BaseSystemInfo.StartupPath + "/Error/Error" + DateTime.Now.ToString(BaseSystemInfo.DateFormat) + ".txt";
            }
            WriteMessage(ex.Message, fileName);
        }
        #endregion

        #region static void WriteException(BaseUserInfo userInfo, Exception ex) 写入异常情况
        /// <summary>
        /// 写入异常情况
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="Exception">异常</param>
        /// <param name="fileName">文件名</param>
        public static void WriteException(BaseUserInfo userInfo, Exception ex, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                if (!string.IsNullOrEmpty(userInfo.UserName))
                {
                    fileName = @"Error/Error" + DateTime.Now.ToString(BaseSystemInfo.DateFormat) + userInfo.UserName + ".txt";
                }
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "Error/Error.txt";
            }

            string message = ex.Source
                + System.Environment.NewLine + ex.StackTrace
                + System.Environment.NewLine + ex.Source
                + System.Environment.NewLine + ex.TargetSite
                + System.Environment.NewLine + ex.Message;

            WriteMessage("错误消息:" + message + ", 错误堆栈:" + ex.StackTrace, fileName);
        }
        #endregion

        /// <summary>
        /// 写文件，目录没有存在会自动创建
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="fileName">文件名</param>
        public static void WriteMessage(string message, string fileName = "Log.txt")
        {
            // 将异常信息写入本地文件中
            string writerFileName = fileName;
            // 将异常信息写入本地文件中
            if (!File.Exists(writerFileName))
            {
                string directoryName = System.IO.Path.GetDirectoryName(fileName);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
                FileStream fileStream = new FileStream(writerFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fileStream.Close();
            }
            using (StreamWriter streamWriter = new StreamWriter(writerFileName, true, Encoding.Default))
            {
                // streamWriter.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " " + message);
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }

        /// <summary>
        /// 向创建二进制文件，并向其中写入二进制信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="message">文件文本内容</param>
        public static void WriteBinaryFile(string fileName, string message)
        {
            Console.WriteLine("写入二进制文件信息开始。");
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;
            try
            {
                // 首先判断，文件是否已经存在
                if (File.Exists(fileName))
                {
                    // 如果文件已经存在，那么删除掉.
                    File.Delete(fileName);
                }
                // 注意第2个参数：
                // FileMode.Create 指定操作系统应创建新文件。如果文件已存在，它将被覆盖。
                fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                binaryWriter = new BinaryWriter(fileStream);

                // 写入测试数据.
                // bw.Write(0x20);
                // bw.Write(1024.567d);
                // bw.Write(1024);

                // 注意，二进制写入 字符串信息的时候
                // 带长度前缀的字符串通过在字符串前面放置一个包含该字符串长度的字节或单词，来表示该字符串的长度。
                // 此方法首先将字符串长度作为一个四字节无符号整数写入，然后将这些字符写入流中。

                // 这里将先写入一个 0x07， 然后再写入 abcdefg
                // bw.Write("abcdefg");
                byte[] binaryBytes = System.Text.Encoding.UTF8.GetBytes(message);
                // binaryWriter.Write(binaryBytes);
                binaryWriter.Write(binaryBytes);

                // 关闭文件.
                binaryWriter.Close();
                fileStream.Close();

                binaryWriter = null;
                fileStream = null;
            }
            catch
            {
                // Console.WriteLine("在写入文件的过程中，发生了异常！");
                // Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                if (binaryWriter != null)
                {
                    try
                    {
                        binaryWriter.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
                if (fileStream != null)
                {
                    try
                    {
                        fileStream.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }
            // Console.WriteLine("写入二进制文件信息结束。");
        }
        
        /// <summary>
        /// 测试向从二进制文件中读取数据，并显示到终端上.
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public static string ReadBinaryFile(string fileName)
        {
            string message = string.Empty;
            // Console.WriteLine("读取二进制文件信息开始。");
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                // 首先判断，文件是否已经存在
                if (!File.Exists(fileName))
                {
                    // 如果文件不存在，那么提示无法读取！
                    // Console.WriteLine("二进制文件{0}不存在！", fileName);
                    return string.Empty;
                }


                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

                // int a = br.ReadInt32();
                // double b = br.ReadDouble();
                // int c = br.ReadInt32();
                // byte len = br.ReadByte();
                // char[] d = br.ReadChars(len);

                // Console.WriteLine("第一个数据:{0}", a);
                // Console.WriteLine("第二个数据:{0}", b);
                // Console.WriteLine("第三个数据:{0}", c);
                // Console.WriteLine("第四个数据 (长度为{0}):", len);
                // foreach (char ch in d)
                // {
                //    Console.Write(ch);
                // }
                // Console.WriteLine();
                //完整的读取文件类容需要获取文件的长度
                int count = (int)fs.Length;
                byte[] buffer = new byte[count];
                br.Read(buffer, 0, buffer.Length);
                message = Encoding.Default.GetString(buffer);
               // message = br.ReadString();
                
                // 读取完毕，关闭.
                br.Close();
                fs.Close();

                br = null;
                fs = null;
            }
            catch
            {
                // Console.WriteLine("在读取文件的过程中，发生了异常！");
                // Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }

                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }
            // Console.WriteLine("读取二进制文件信息结束。");
            return message;
        }

        /// <summary>
        /// 删除文件 
        /// </summary>
        /// <param name="fileName">文件全路径</param>
        /// <returns>bool 是否删除成功</returns>
        public static bool DeleteFile(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                if (File.GetAttributes(fileName) == FileAttributes.Normal)
                {
                    File.Delete(fileName);
                }
                else
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                    File.Delete(fileName);
                }
                return true;
            }
            else
            {
                // 文件不存在
                return false;
            }
        }

        /// <summary>
        /// 根据文件名，得到文件的大小，单位分别是GB/MB/KB
        /// </summary>
        /// <param name="FileFullPath">文件名</param>
        /// <returns>返回文件大小</returns>
        public static string GetFileSize(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                FileInfo fileinfo = new FileInfo(fileName);
                long fl = fileinfo.Length;
                if (fl > 1024 * 1024 * 1024)
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / (1024 * 1024 * 1024), 2)) + " GB";
                }
                else if (fl > 1024 * 1024)
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / (1024 * 1024), 2)) + " MB";
                }
                else
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / 1024, 2)) + " KB";
                }
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// 读取文件文件内容
        /// 文本格式必须为utf-8
        /// (ansi格式容易产生乱码)
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string GetTextFileContent(string fileName)
        {
            StreamReader streamReader = new StreamReader(fileName, Encoding.GetEncoding("utf-8"));
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// 根据文件后缀来获取MIME类型字符串
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static string GetMimeType(string extension)
        {
            string mime = string.Empty;
            extension = extension.ToLower();
            switch (extension)
            {
                case ".avi": mime = "video/x-msvideo"; break;
                case ".bin":
                case ".exe":
                case ".msi":
                case ".dll":
                case ".class": mime = "application/octet-stream"; break;
                case ".csv": mime = "text/comma-separated-values"; break;
                case ".html":
                case ".htm":
                case ".shtml": mime = "text/html"; break;
                case ".css": mime = "text/css"; break;
                case ".js": mime = "text/javascript"; break;
                case ".doc":
                case ".dot":
                case ".docx": mime = "application/msword"; break;
                case ".xla":
                case ".xls":
                case ".xlsx": mime = "application/msexcel"; break;
                case ".ppt":
                case ".pptx": mime = "application/mspowerpoint"; break;
                case ".gz": mime = "application/gzip"; break;
                case ".gif": mime = "image/gif"; break;
                case ".bmp": mime = "image/bmp"; break;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                case ".png": mime = "image/jpeg"; break;
                case ".mpeg":
                case ".mpg":
                case ".mpe":
                case ".wmv": mime = "video/mpeg"; break;
                case ".mp3":
                case ".wma": mime = "audio/mpeg"; break;
                case ".pdf": mime = "application/pdf"; break;
                case ".rar": mime = "application/octet-stream"; break;
                case ".txt": mime = "text/plain"; break;
                case ".7z":
                case ".z": mime = "application/x-compress"; break;
                case ".zip": mime = "application/x-zip-compressed"; break;
                default:
                    mime = "application/octet-stream";
                    break;
            }
            return mime;
        }
    }
}