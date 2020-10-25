using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;


namespace BehanceBot
{
    internal class WorkAddBoardBot : Bot
    {
        public WorkAddBoardBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
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
                    string xpathUser = user_xpath + i + "]";

                    if (imageAddBoard_counter >= limit)
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
                        AddImageToBoard(userUrl);
                        imageAddBoard_counter++;
                        console.WriteLine($"Save image to board. Count = {imageAddBoard_counter}");
                    }

                    Thread.Sleep(100);
                }
            }



            void AddImageToBoard(string userUrl)
            {
                chrome.OpenUrlNewTab(userUrl);

                IWebElement Element_photo = chrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));
                string url_photo = Element_photo.GetAttribute("href");
                chrome.OpenUrl(url_photo);

                Thread.Sleep(600);
                // console.WriteLine("Open board page.");
                if (!chrome.ClickButtonXPath(@"//div[.='Сохранить']"))
                {
                    console.WriteLine($"Error open board.");
                }
                Thread.Sleep(500);

                // console.WriteLine("Select board.");
                if (!chrome.ClickButtonXPath(@"//*[@id='app']/div/div/div[15]/div/div/ul/li"))
                {
                    console.WriteLine($"Error select board.");
                }
                Thread.Sleep(600);

                // console.WriteLine("Save image.");
                if (!chrome.ClickButtonXPath(@"//*[@id='app']/div/div/div[15]/div/div/div[4]/div[2]/button/div/div"))
                {
                    console.WriteLine($"Error save image to board.");
                }
                Thread.Sleep(3000);

                chrome.CloseAndReturnTab();
            }

        }

    }
}
