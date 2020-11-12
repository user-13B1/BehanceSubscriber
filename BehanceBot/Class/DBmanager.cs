using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Data;
using System.Data.Linq.Mapping;
using System.Runtime.InteropServices.WindowsRuntime;
using LiteDB;


namespace BehanceBot
{
    class DBmanager
    {
        readonly Writer cons;
        private LiteDatabase db;
        ILiteCollection<BehanceUsers> collection
        public DBmanager(Writer cons)
        {
            this.cons = cons;
            db = new LiteDatabase(Directory.GetCurrentDirectory() + @"\Data.db");
            collection = db.GetCollection<BehanceUsers>("ParseBehanceUsers");
        }

        internal void AddForSubsList(string url)
        {
            var newUser = new BehanceUsers
            {
                Url = url,
                Date = DateTime.Now,
                donorsubs = 1,
                formerfriend = 0,
                friend = 0
            };
            collection.Insert(newUser);
        }

        internal void AddToFriend(string url)
        {
            var newUser = new BehanceUsers
            {
                Url = url,
                Date = DateTime.Now,
                donorsubs = 0,
                formerfriend = 0,
                friend = 1
            };
            collection.Insert(newUser);
        }


        internal bool IsRepeat(string url)
        {
            ILiteCollection<BehanceUsers> collection = db.GetCollection<BehanceUsers>("ParseBehanceUsers");

            var query = collection.Query()
                .Where(u => u.Url == url);

            if (query.Count() != 0)
            {
                cons.WriteLine($"Repeat account.");
                return true;
            }
            else
                return false;


        }


        [Table(Name = "ParseInstagramUsers")]
        public class BehanceUsers
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }
            public string Url { get; set; }
            public DateTime Date { get; set; }
            public Byte donorsubs { get; set; }
            public Byte formerfriend { get; set; }
            public Byte friend { get; set; }
      
        }

    
    }
}

//var results = col.Query()
//.Where(x => x.Name.StartsWith("J"))
//.OrderBy(x => x.Name)
//.Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
//.Limit(10)
//.ToList();