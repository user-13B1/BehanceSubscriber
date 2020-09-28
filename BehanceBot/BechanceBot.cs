using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;




namespace BehanceBot
{

    public partial class BehBotForm : Form
    {
        private readonly Writer console;
        private readonly FileReader fileReader;
        private readonly List<Bot> bots;


        public BehBotForm()
        {
            InitializeComponent();
           
            textBoxLogin.Text = Properties.Settings.Default.login;
            textBoxPassword.Text = Properties.Settings.Default.password;
            
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1600, 100);

            fileReader = new FileReader();
            console = new Writer(this, fileReader);    // Активируем лог консоль
            bots = new List<Bot>(3);
        }

        private void BehBotForm_Load(object sender, EventArgs e)
        {
            Task.Run(() => Launch(new FollowingBot(console, fileReader), 200));
            Task.Run(() => Launch(new LikeBot(console, fileReader), 200));
            Task.Run(() => Launch(new UnFollowingBot(console, fileReader), 220));
            Task.Run(() => TimerEnd(TimeSpan.FromHours(3)));
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
            if(bots.Count==0)
            {
                console.WriteLine("All bot stoped. Application closed.");
                Thread.Sleep(TimeSpan.FromSeconds(5));
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

            Thread.Sleep(TimeSpan.FromSeconds(5));
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

