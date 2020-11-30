using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.Linq.Mapping;
using System.Runtime.InteropServices.WindowsRuntime;
using LiteDB;
using System.Linq;
using System.IO;
using StackExchange.Redis;


namespace BehanceBot
{
    public partial class BehBotForm : Form
    {
        private readonly Writer console;
        private readonly FileReaderWriter fileReader;
        private readonly List<Bot> bots;
        List<Process> ProcessChromeDriver;
        DBmanager db;
        static bool editFlag;
       
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
            Task.Run(() => Timer(TimeSpan.FromHours(4)));
            db = new DBmanager(console);
            Task.Run(() => Launch(new SubscriberBot(console, fileReader, db), 200));
            Task.Run(() => Launch(new LikeBot(console, fileReader, db), 200));
            Task.Run(() => Launch(new UnsubscribeBot(console, fileReader, db), 250));
            Task.Run(() => Launch(new WorkSaveBoardBot(console, fileReader, db), 5600));

        }


        private void Launch(Bot bot,int limit)
        {
            bots.Add(bot);
            bot.Autorize(textBoxLogin.Text, textBoxPassword.Text);
            bot.Start(limit);
            CloseBot(bot);
        }

        private void CloseBot(Bot bot)
        {
            bot.Close();
            bots.Remove(bot);
            if(bots.Count == 0)
            {
                console.WriteLine("All bot stoped. Application closed.");
                Thread.Sleep(TimeSpan.FromSeconds(10));
                CloseProgram();
            }
        }

        private void Timer(TimeSpan timeSpan)
        {
            ProcessChromeDriver = new List<Process>();
            List<Process> processChromeDriverOld = Process.GetProcessesByName("chromedriver").ToList();
            Thread.Sleep(TimeSpan.FromSeconds(10));
            List<Process> ProcessChromeDriverNew = Process.GetProcessesByName("chromedriver").ToList();

            
            foreach (Process proces in ProcessChromeDriverNew)
                if(processChromeDriverOld.Find(item => item.Id == proces.Id)==null)
                    ProcessChromeDriver.Add(proces);

            Thread.Sleep(timeSpan);
            console.WriteLine("Timer End. Application closed.");
            foreach (Bot bot in bots)
                bot.Close();
            Thread.Sleep(TimeSpan.FromSeconds(10));

            CloseProgram();
        }

        private void CloseProgram()
        {
            db.CloseBase();
            foreach (Process proces in ProcessChromeDriver)
            {
                if (proces != null)
                    proces.CloseMainWindow();
            }
            Properties.Settings.Default.Save();
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
        private void BehBotForm_FormClosed(object sender, FormClosedEventArgs e) => CloseProgram();
        

        private void EditButton_Click(object sender, EventArgs e)
        {
            if(editFlag)
            {
                textBoxLogin.ReadOnly = true;
                textBoxPassword.ReadOnly = true;
                textBoxPassword.UseSystemPasswordChar = true;
            }
            else
            {
                textBoxLogin.ReadOnly = false;
                textBoxPassword.ReadOnly = false;
                textBoxPassword.UseSystemPasswordChar = false;
            }
            editFlag = !editFlag;

        }
    }
}

