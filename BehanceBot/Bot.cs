using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Runtime.InteropServices.WindowsRuntime;
using SeleniumLib;

namespace BehanceBot
{
    internal abstract class Bot
    {
        internal readonly Chrome chrome;
        internal readonly Writer console;
        internal readonly FileReader fileReader;
        internal string Name { get; set; }
        internal string user_xpath;

        public Bot(Writer Cons, FileReader fileReader)
        {
            this.console = Cons;
            this.fileReader = fileReader;
            chrome = new Chrome();
            chrome.SetWindowSize(1280, 1000);
            user_xpath = "//*[@id='app']/div/div/div[18]/div/div[1]/div/div[2]/div/div[1]/ul/li[";
        }


        internal bool IsBlock()
        {
            string xpath = $@"/html/body/div[26]/div/div[2]/a";
            if (chrome.IsElementPage(By.XPath(xpath)))
            {
                console.WriteLine($"Limit error.");
                return true;
            }

            return false;
        }

        internal bool Autorize(string v1, string v2)
        {
            int timeSleep = 2000;
#if DEBUG
            timeSleep = timeSleep / 10;
#endif

            console.WriteLine($"{Name}: authorization.");                                 //Ввод логина и пароля. 
            chrome.OpenUrl(@"https://www.behance.net/search");
            
            //---Вход---
            string xpath = "//button[@class='Btn-button-BGn Btn-base-M-O Btn-normal-hI4 js-adobeid-signin PrimaryNav-a11yButton-2Cl']";
            IWebElement Element = chrome.FindWebElement(By.XPath(xpath));
            Element.Click();
            Thread.Sleep(timeSleep);
           
            chrome.SendKeysXPath(@".//input[@id='EmailPage-EmailField']", v1);
            Thread.Sleep(timeSleep);
            chrome.ClickButtonXPath(@".//*[@id='EmailForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            chrome.SendKeysXPath(@".//input[@id='PasswordPage-PasswordField']", v2);
            Thread.Sleep(timeSleep);
            chrome.ClickButtonXPath(@".//*[@id='PasswordForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            //открываем подписки
            if (!chrome.ClickButtonXPath(@"./html/body/nav/ul[2]/li[4]/a"))
            {
                console.WriteLine($"{Name}: Error аutorized.");
                return false;
            }

            chrome.ClickButtonXPath(@".//*[@id='app']/div/div/div[1]/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[3]/td[2]/a");
            console.WriteLine($"{Name}: аutorized.");
            return true;
        }

        internal void OpenRandomPage()
        {
            string s_rand_page = fileReader.GetRandomUrl();
            console.WriteLine($"Open random page {s_rand_page}");
            chrome.OpenUrl(s_rand_page);
        }

        internal int ParsToInt(string text)
        {
            if (!Int32.TryParse(text, out _))
            {
                text = text.Replace(" ", string.Empty);
                text = text.Replace("тыс.", "000");
                if (text.IndexOf(',') > 0)
                {
                    text = text.Replace(",", string.Empty);
                    text = text.Substring(0, text.Length - 1);
                }
            }

            if (!Int32.TryParse(text, out int num))
                console.WriteLine($"Ошибка парсинга строки в число - {text}");

            return num;
        }

        internal virtual void Start(int limit) { }

        internal void Close()
        {
            console.WriteLine($"{Name}: Close.");
            chrome.Quit();
        }
    }


    internal class FollowingBot : Bot
    {
        int folow_counter;
        public FollowingBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
        {
            Name = "FollowingBot";
        }

        internal override void Start(int limit)
        {
            
            folow_counter = 0;
            OpenRandomPage();

            for (int i = 3; i < 3000; i++)
            {
                if (IsBlock())
                    return;
                string xpath = user_xpath + i + @"]";
                console.WriteLine($"{Name}: Scroll.");
                chrome.Scroll(xpath);
                if (!ParseAndFollowing(xpath, i, limit)) return;
                Thread.Sleep(300);
            }
            console.WriteLine($"{Name}: Stop.");
        }

        internal bool ParseAndFollowing(string xpath, int i, int follow_max_count)
        {
           
            try
            {
                string s_like = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span")).Text;
                int like_persona = ParsToInt(s_like);

                string s_num_views = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text;
                int num_views = ParsToInt(s_num_views);
                string button_text = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

                IWebElement Element = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
                string name_persona = Element.Text;
                string url_persona = Element.GetAttribute("href");

                if (like_persona < 100 && like_persona > 5 && num_views < 999 && num_views > 20 && button_text == "Подписаться")
                {
                    console.WriteLine($"{i}) {like_persona} {num_views} {name_persona} Подписка № {folow_counter}");

                    chrome.ClickButtonXPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]");
                    folow_counter++;
                }

                if (num_views > 20000)
                {
                    console.WriteLine($"{i}) {name_persona} Добавлен для подписок.");
                    fileReader.WriteUrltoFile(url_persona + @"/followers");
                }

                if (folow_counter >= follow_max_count)
                {
                    console.WriteLine($"Выполнено.Завершение работы.");
                    return false;
                }


            }
            catch
            {
                console.WriteLine($"Ошибка парсинга аккаунтов для подписки.");
                return false;
            }


            return true;
        }

    }

    internal class UnFollowingBot : Bot
    {

        public UnFollowingBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
        {
            Name = "UnFollowingBot";
        }

        internal override void Start(int limit)
        {
            int subs_count = OpenMySubs();
            if (subs_count <= 500)
            {
                console.WriteLine("Количество подписок не достаточно для начала отписки.");
                return;
            }

            for (int i = 2; i < subs_count; i++) //Скролим вниз
            {
                
                string xpath = user_xpath + i + "]";
                chrome.Scroll(xpath);
                // Cons.WriteLine(i, false);
            }

            for (int i = subs_count - 3; i > 0; i--) //Цикл отписки
            {
                if (IsBlock())
                    return;

                string xpath = user_xpath + i + "]";
                chrome.Scroll(xpath);
                Thread.Sleep(3000);
                Unsubscribe(i);
                limit--;
                if (limit <= 0)
                    return;
            }

        }


        internal int OpenMySubs()
        {
            console.WriteLine("Open my portfolio page");
            chrome.OpenUrl(@"https://www.behance.net/balakir/projects");
            string mySubs_xPath = @"//*[@id='site-content']/div/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[4]/td[2]/a";

            int subs_count = ParsToInt(chrome.FindWebElement(By.XPath(mySubs_xPath)).Text);
            console.WriteLine($"Number of our subscriptions {subs_count}");
            chrome.ClickButtonXPath(mySubs_xPath);
            return subs_count;
        }

        internal void Unsubscribe(int j)
        {
            j += 2;
            console.WriteLine($"Unsubscribe: {j}", false);
            string btn_1 = user_xpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[2]";
            string btn_2 = user_xpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[3]";
            IWebElement elem_button_1 = chrome.FindWebElement(By.XPath(btn_1));
            IWebElement elem_button_2 = chrome.FindWebElement(By.XPath(btn_2));

            if (elem_button_1.Displayed)
            {
                chrome.ClickButtonXPath(btn_1);
                return;
            }
            if (elem_button_2.Displayed)
            {
                chrome.ClickButtonXPath(btn_2);
                return;
            }
            console.WriteLine($"Account: {j}, error unsubscribe");
        }
    }

    internal class LikeBot : Bot
    {

        public LikeBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
        {
            Name = "LikeBot";
        }

        internal override void Start(int limit)
        {
            int like_counter = 0;
            for (int j = 0; j < 30; j++)
            {
                OpenRandomPage();
                for (int i = 3; i < 3000; i++)
                {
                    string xpathUser = user_xpath + i + "]";

                    if (like_counter >= limit)
                        return;

                    if (IsBlock())
                        return;

                    if (!chrome.Scroll(xpathUser))
                    {
                        console.WriteLine("End following list");
                        break;
                    }

                    if (CheckUser(xpathUser, out string userUrl))
                    {
                        LikePhoto(userUrl);
                        like_counter++;
                        console.WriteLine($"Like! Number of likes = {like_counter}");
                    }

                    Thread.Sleep(200);
                }
            }

            bool CheckUser(string xpath, out string userUrl)
            {
                int like_persona = ParsToInt(chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span")).Text);
                int num_views = ParsToInt(chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text);
                string button_text = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

                IWebElement Element = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
                userUrl = Element.GetAttribute("href");

                if (like_persona < 100 && like_persona > 10 && num_views < 999 && num_views > 20 && button_text == "Подписаться")
                    return true;

                return false;
            }

            void LikePhoto(string userUrl)
            {
                chrome.OpenUrlNewTab(userUrl);
                                                                                                                              
                IWebElement Element_photo = chrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));
                string url_photo = Element_photo.GetAttribute("href");
                chrome.OpenUrl(url_photo);

                Thread.Sleep(300);

                if (!chrome.ClickButtonXPath(@"//div[.='Оценить']"))
                {
                    console.WriteLine($"Error like");
                }

                Thread.Sleep(500);
                chrome.CloseAndReturnTab();
            }

        }
    }
}