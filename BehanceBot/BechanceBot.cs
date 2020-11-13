using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.Linq.Mapping;
using System.Runtime.InteropServices.WindowsRuntime;
using LiteDB;
using System.Linq;
using System.IO;


namespace BehanceBot
{
    public partial class BehBotForm : Form
    {
        private readonly Writer console;
        private readonly FileReaderWriter fileReader;
        private readonly List<Bot> bots;

        public BehBotForm()
        {
            InitializeComponent();
           
            textBoxLogin.Text = Properties.Settings.Default.login;
            textBoxPassword.Text = Properties.Settings.Default.password;
            
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1600, 100);

            fileReader = new FileReaderWriter();
            console = new Writer(this, fileReader);
            bots = new List<Bot>();
        }

        private void BehBotForm_Load(object sender, EventArgs e)
        {
            DBmanager db = new DBmanager(console);
            Task.Run(() => Launch(new SubscriberBot(console, fileReader, db), 200));
            Task.Run(() => Launch(new LikeBot(console, fileReader, db), 200));
            Task.Run(() => Launch(new UnsubscribeBot(console, fileReader,  db), 310));
            for (int i = 0; i < 2; i++)
                Task.Run(() => Launch(new WorkSaveBoardBot(console, fileReader,  db), 300));
            Task.Run(() => TimerEnd(TimeSpan.FromHours(2)));
        }


        private void Launch(Bot bot,int limit)
        {
            bots.Add(bot);
            bot.Autorize(textBoxLogin.Text, textBoxPassword.Text);
            bot.Start(limit);
            Close(bot);
        }

        private void Close(Bot bot)
        {
            bot.Close();
            bots.Remove(bot);
            if(bots.Count == 0)
            {
                console.WriteLine("All bot stoped. Application closed.");
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Application.Exit();
            }
        }

        private void TimerEnd(TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
            foreach (Bot bot in bots)
            {
                bot.Close();
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
            console.WriteLine("Timer went off. Application closed.");
            Application.Exit();
        }

        private void TextBoxLogin_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = textBoxLogin.Text;
        }
        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.password = textBoxPassword.Text;
        }
        private void BehBotForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}

