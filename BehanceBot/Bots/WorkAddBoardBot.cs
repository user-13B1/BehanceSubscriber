using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Drawing.Text;
using System.Threading;


namespace BehanceBot
{
    internal class WorkAddBoardBot : Bot
    {
        public WorkAddBoardBot(Writer Cons, FileReaderWriter fileReader, DBmanager db) : base(Cons, fileReader,db)
        {
            Name = "WorkAddBoardBot";
        }

        internal override void Start(int limit)
        {
            int imageAddBoard_counter = 0;

            for (int j = 0; j < 30; j++)
            {
                OpenRandomPage();

                for (int i = 3; i < 3000; i++)
                {
                    string xpathUser = UserXpath + i + "]";

                    if (imageAddBoard_counter >= limit)
                        return;

                    if (IsBlock())
                        return;

                    if (!Сhrome.Scroll(xpathUser))
                    {
                        Cons.WriteLine("End following list");
                        break;
                    }

                    if (CheckUser(xpathUser, out string userUrl))
                    {
                        AddImageToBoard(userUrl);
                        imageAddBoard_counter++;
                        Cons.WriteLine($"Save image to board. Count = {imageAddBoard_counter}");
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private void AddImageToBoard(string userUrl)
        {



            Сhrome.OpenUrlNewTab(userUrl);

            IWebElement Element_photo = Сhrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));

            if (Element_photo == null)
                Element_photo = Сhrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div[2]/a"));

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
            //*[@id="app"]/div/div/div[16]/div/div/div[4]/div[1]/text()
           
            if (Сhrome.FindWebElement(By.XPath("//*[contains(text(), 'Новая доска настроени')]")) == null)
            {
                if (!Сhrome.ClickButtonXPath("//div[. = 'Сохранить']"))
                {
                    Cons.WriteLine($"Error check open board.");
                    Сhrome.CloseAndReturnTab();
                    return;
                }
            }


            if (!Сhrome.ClickButtonXPath($"//li[. ='{GetRandomNameBoard()}']"))
            {
                Cons.WriteLine($"Error select board.");
            }
            Thread.Sleep(600);

            // console.WriteLine("Save image.");

            if (!Сhrome.ClickButtonXPath("//button[. ='Сохранить']"))
            // if (!Сhrome.ClickButtonXPath(@"//*[@id='app']/div/div/div[16]/div/div/div[4]/div[2]/button/div/div"))
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
