using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;


namespace BehanceBot
{
    internal class SubscriberBot : Bot
    {
        int folow_counter;
        public SubscriberBot(Writer Cons, FileReaderWriter fileReader) : base(Cons, fileReader)
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
                string xpath = UserXpath + i + @"]";
                Сhrome.Scroll(xpath);
                if (!ParseAndFollowing(xpath, i, limit)) return;
                Thread.Sleep(300);
            }
            Cons.WriteLine($"{Name}: Stop.");
        }

        internal bool ParseAndFollowing(string xpath, int i, int follow_max_count)
        {
            try
            {
                string s_like = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span")).Text;
                int like_persona = ParsToInt(s_like);

                string s_num_views = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text;
                int num_views = ParsToInt(s_num_views);
                string button_text = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

                IWebElement Element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
                string name_persona = Element.Text;
                string url_persona = Element.GetAttribute("href");

                if (like_persona < 100 && like_persona > 5 && num_views < 999 && num_views > 20 && button_text == "Подписаться")
                {
                    Cons.WriteLine($"{i}) {like_persona} {num_views} {name_persona} Подписка № {folow_counter}");

                    Сhrome.ClickButtonXPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]");
                    folow_counter++;
                }

                if (num_views > 10000)
                {
                    Cons.WriteLine($"{i}) {name_persona} Add for subscribe.");
                    FileReader.WriteUrltoFile(url_persona + @"/followers");
                }

                if (folow_counter >= follow_max_count)
                {
                    Cons.WriteLine($"{Name}: Follow limit. End work");
                    return false;
                }

            }
            catch
            {
                Cons.WriteLine($"Error parsing account.");
                return false;
            }

            return true;
        }

    }

}
