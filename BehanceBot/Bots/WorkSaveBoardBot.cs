using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Drawing.Text;
using System.Threading;


namespace BehanceBot
{
    internal class WorkSaveBoardBot : Bot
    {
        static int imageAddBoard_counter;

        public WorkSaveBoardBot(Writer Cons, DBmanager db) : base(Cons, db)
        {
            Name = "WorkSaveBoard";
        }

        internal override void Start(int limit)
        {
            for (int j = 0; j < 100; j++)
            {
                if (!OpenRandomFollowerPage())
                {
                    Cons.WriteLine($"Open random page error.");
                    return;
                }

                for (int i = 3; i < 3000; i++)
                {
                    string xpathNextUser = UserXpath + "/li[" + i + "]";

                    if (imageAddBoard_counter >= limit)
                        return;

                    if (IsBlock())
                        return;

                    if (!Сhrome.Scroll(xpathNextUser))
                        break;

                    if (CheckUser(xpathNextUser, out string userUrl, out _, out _, out _))
                    {
                        AddImageToBoard(userUrl);
                        db.AddUser(userUrl,0,0,0);
                        imageAddBoard_counter++;
                        Cons.WriteLine($"{Name}. Count = {imageAddBoard_counter}");
                    }

                    Thread.Sleep(100);
                }
                Cons.WriteLine("End following list");
            }
           
        }

        private void AddImageToBoard(string userUrl)
        {
            Сhrome.OpenUrlNewTab(userUrl);
            IWebElement Element_photo = Сhrome.FindWebElement(By.XPath("//div[@class = 'e2e-Work']/div/div[1]/div[1]/div/div/div[2]/a"));

            if (Element_photo == null)
            {
                Cons.WriteLine($"AddImageToBoard. v2 {userUrl}");
                Сhrome.CloseAndReturnTab();
                return;
            }
                                                            
            string url_photo = Element_photo.GetAttribute("href");
            Сhrome.OpenUrl(url_photo);

            Thread.Sleep(1000);
            if (!Сhrome.ClickButtonXPath("//div[. = 'Сохранить']"))
            {
                Cons.WriteLine($"Error open board.");
                Сhrome.CloseAndReturnTab();
                return;
            }

            Thread.Sleep(500);
            if (Сhrome.FindWebElement(By.XPath("//*[contains(text(), 'Новая доска настроени')]")) == null)
            {
                if (!Сhrome.ClickButtonXPath("//div[. = 'Сохранить']"))
                {
                    Cons.WriteLine($"Error check open board.");
                    Сhrome.CloseAndReturnTab();
                    return;
                }
            }

            Thread.Sleep(200);
            if (!Сhrome.ClickButtonXPath($"//li[. ='{GetRandomNameBoard()}']"))
            {
                Cons.WriteLine($"Error select board.");
            }

            Thread.Sleep(600);
            if (!Сhrome.ClickButtonXPath("//button[. ='Сохранить']"))
            {
                Cons.WriteLine($"Error save image to board.");
            }

            Thread.Sleep(3000);
            Сhrome.CloseAndReturnTab();
        }


        private string GetRandomNameBoard()
        {
            Random rnd = new Random();
            switch (rnd.Next(3))
            {
                case 0: return "Idea";
                case 1: return "GoodWork";
                case 2: return "Interesting";
                default: throw new ArgumentException("Недопустимый код операции");
            }
        }

    }
}
