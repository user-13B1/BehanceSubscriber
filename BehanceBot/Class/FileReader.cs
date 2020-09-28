using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace BehanceBot
{

    internal class FileReader
    {
        private readonly string data_path_dir;
        private readonly string message_log_txt_path;
        readonly string urls_path = @"\Urls.txt";
        readonly List<string> buffer_mes_list;

        public FileReader()
        {
            data_path_dir = Directory.GetCurrentDirectory();
            buffer_mes_list = new List<string>();

            if (!Directory.Exists(data_path_dir))
            {
                try
                {
                    Directory.CreateDirectory(data_path_dir); //создаем директорию лога
                }
                catch
                {
                    MessageBox.Show("Ошибка создания директории для лога.", "Ошибка.");
                }
            }

            if (Directory.Exists(data_path_dir))
            {
                
                message_log_txt_path = this.data_path_dir + @"\log_message.txt";
            }

            if (!File.Exists(data_path_dir + urls_path))
            {
                MessageBox.Show("Рабочая директория не найдена.", "Ошибка.");
            }


            Task.Run(() => LogWriteToFile(TimeSpan.FromSeconds(10))); // Таск записи сообщений консоли в файл.

        }


        internal void AddBufferMessage(object s)
        {
            string mess = String.Format("{0:T} ", DateTime.Now) + String.Format($"{s}");
            buffer_mes_list.Add(mess);

        }


        private void LogWriteToFile(TimeSpan ping)
        {
            while (true)
            {
                WriteBufferToFile();
                Thread.Sleep(ping);
            }
        }

        async internal void WriteBufferToFile()
        {
            if (buffer_mes_list.Count == 0)
                return;
            try
            {
                if (Directory.Exists(data_path_dir))
                {
                    using (StreamWriter sw = new StreamWriter(message_log_txt_path, true, Encoding.Default))
                    {
                        var s_mes = String.Join("\n", buffer_mes_list.ToArray());
                        buffer_mes_list.Clear();
                        await sw.WriteLineAsync(s_mes);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка {e.Message}", "Ошибка.");
            }

        }


        internal string GetRandomUrl()
        {
            Random rnd = new Random();
            List<string> urls = ScanFileList();
            if(urls.Count>0)
            {
                int value = rnd.Next(0, urls.Count);
                DeletStringFromFile(value);
                return urls[value];
            }
            else
                return null;
        }

        private void DeletStringFromFile(int value)
        {
            string pathFile = data_path_dir + urls_path;

            string[] readText = File.ReadAllLines(pathFile);
           
            using (StreamWriter file = new StreamWriter(pathFile, false))
            {
                for (int i = 0; i < readText.Length; i++)
                {
                    if (i != value)
                        file.WriteLine(readText[i]);
                }
            }


        }

        public List<string> ScanFileList()
        {
            List<string> arrTag = new List<string>();
            string pathFile = data_path_dir + urls_path;

            using (StreamReader sr = new StreamReader(pathFile, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.Length > 10 && line.Length < 120 && line.IndexOf(' ') == -1)
                    {
                        arrTag.Add(line);
                    }
                }
            }

            if (arrTag.Count < 1)
            {
                return null;
            }
            return arrTag;
        }

        internal void WriteUrltoFile(string url_persona)
        {
            string pathFile = data_path_dir + urls_path;
            try
            {
                using (StreamWriter sw = new StreamWriter(pathFile, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(url_persona);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка {e.Message}", "Ошибка.");
            }

        }
    }
}