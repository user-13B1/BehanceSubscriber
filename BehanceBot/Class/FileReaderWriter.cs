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

    internal class FileReaderWriter
    {
        private readonly string dataPathDir;
        private readonly string message_log_txt_path;
        readonly string urls_path = @"\Urls.txt";
        readonly List<string> buffer_mes_list;

        public FileReaderWriter()
        {
            dataPathDir = Directory.GetCurrentDirectory();
            buffer_mes_list = new List<string>();

            if (!Directory.Exists(dataPathDir))
            {
                try
                {
                    Directory.CreateDirectory(dataPathDir); //создаем директорию лога
                }
                catch
                {
                    MessageBox.Show("Ошибка создания директории для лога.", "Ошибка.");
                }
            }

            if (Directory.Exists(dataPathDir))
            {
                
                message_log_txt_path = this.dataPathDir + @"\log_message.txt";
            }

            if (!File.Exists(dataPathDir + urls_path))
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
                if (Directory.Exists(dataPathDir))
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

    }
}