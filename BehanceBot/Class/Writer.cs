using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace BehanceBot
{
    internal class Writer
    {
        private readonly BehBotForm Form;
        private readonly TextWriter _writer;
        private readonly FileReader file_reader;


        public Writer(BehBotForm Form, FileReader file_reader)
        {
            this.Form = Form;
            
            this.file_reader = file_reader;
            _writer = new TextBoxStreamWriter(Form.txtConsole1);
            Console.SetOut(_writer);   // Перенаправляем выходной поток консоли   
           
        }
       
        public void WriteLine(object s,bool log = true)
        {
            if (Form.InvokeRequired)
            {
               // Form.BeginInvoke(new Action(() => { Console.Write("{0:T} ", DateTime.Now); }));
                Form.BeginInvoke(new Action(() => { Console.WriteLine(s); }));
            }
            else
            {
               // Console.Write("{0:T} ", DateTime.Now);
                Console.WriteLine(s);
            }

            //Запись сообщения  в лог файл
            if (file_reader != null && log)
                file_reader.AddBufferMessage(s);
        }
    }
}