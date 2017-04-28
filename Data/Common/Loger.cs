using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Data.Common
{ /// <summary>
  /// 日志
  /// </summary>
    public static class Loger
    {
        /// <summary>
        /// 路径
        /// </summary>
        private static string filePath;
        /// <summary>
        /// 路径字段
        /// </summary>
        public static string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        /// <summary>
        /// 读取指定的日志到内存缓存中
        /// </summary>
        /// <param name="time"></param>
        /// <param name="Days"></param>
        /// <returns></returns>
        public static List<string> ReadFromLogTxt(DateTime time, int Days)
        {
            List<string> days = new List<string>();
            List<string> logs = new List<string>();
            try
            {
                for (int i = 0; i < Days + 1; i++)
                {
                    days.Add(((Convert.ToInt32(time.ToString("yyyyMMdd")) - i).ToString()));
                }
                foreach (var item in days)
                {
                    try
                    {
                        if (File.Exists(FilePath + @"\LOG\" + item + ".txt"))
                        {
                            string[] value = File.ReadAllLines(FilePath + @"\LOG\" + item + ".txt");
                            logs.AddRange(value);
                        }
                    }
                    catch (Exception ex)
                    {
                        logs.Add(item + ":没有相关日志记录" + ex.ToString());
                        continue;
                    }
                }

            }
            catch (Exception ex)
            {
                AddErrorText("读取日志", ex);
                logs.Add(ex.ToString());
            }
            return logs;
        }
        /// <summary>
        /// 追加到日志的方法。存在文件则追加，没有则创建并追加信息。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string AddLogText(string message)
        {
            HasLogDirectory();//先判断在路径文件夹下是否有LOG文件夹。
            try
            {
                if (!File.Exists(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt")) //判断是否存在日志文件
                {
                    using (
                    FileStream fs1 = new FileStream(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", FileMode.Create, FileAccess.Write))
                    {//创建写入文件
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(DateTime.Now.ToString() + "  " + message + "\r\n");//开始写入值
                    }
                    File.AppendAllText(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", message + "\r\n");
                    return "创建日志成功";
                }
                else //存在则追加
                {
                    File.AppendAllText(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", message + "\r\n");
                    return "成功";
                }
            }
            catch (Exception ex)
            {
                return "追加到日志失败" + ex.ToString();
            }
            // return "追加到日志成功";
        }
        /// <summary>
        /// 异常信息追加到日志
        /// </summary>
        /// <param name="name">异常模块名</param>
        /// <param name="ex">异常</param>
        public static string AddErrorText(string name, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【模块名称】：" + name);
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            sb.AppendLine("***************************************************************");
            string message = sb.ToString();
            HasErrorLogDirectory();//先判断在路径文件夹下是否有LOG文件夹。
            try
            {
                if (!File.Exists(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt")) //判断是否存在日志文件
                {
                    using (
                    FileStream fs1 = new FileStream(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", FileMode.Create, FileAccess.Write))
                    {
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(DateTime.Now.ToString() + "  " + message + "\r\n");//开始写入值
                    }
                    File.AppendAllText(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", message + "\r\n");

                    return "创建日志成功";
                }
                else //存在则追加
                {
                    using (
                    FileStream fs = new FileStream(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", FileMode.Open, FileAccess.Write))
                    {
                        StreamWriter sr = new StreamWriter(fs);
                        sr.BaseStream.Seek(0, SeekOrigin.End);
                        sr.WriteLine(DateTime.Now.ToString() + "  " + message + "\r\n");//开始写入值                   
                    }
                    File.AppendAllText(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt", message + "\r\n");
                    return "追加到日志成功";
                }
            }
            catch
            {
                return "追加到日志失败";
            }
        }
        private static void HasErrorLogDirectory()
        {
            if (Directory.Exists(FilePath + @"\ERRORLOG"))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(FilePath + @"\ERRORLOG");
                return;
            }
        }
        /// <summary>
        /// 是否有日志文件夹 没有则创建
        /// </summary>
        /// <returns></returns>
        public static void HasLogDirectory()
        {
            if (Directory.Exists(FilePath + @"\LOG"))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(FilePath + @"\LOG");
                return;
            }
        }
    }
}

