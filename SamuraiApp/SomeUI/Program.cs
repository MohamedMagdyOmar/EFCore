using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeUI
{
    class Program
    {
        // rather than initiating a new context for every one of my methods, we will class wide context that i instantiate in startup
        private static SamuraiContext _context = new SamuraiContext();

        // note we are running one method at a time, so each time we run one of these methods, i am working with a fresh instance of the "context".
        // if you used it for multiple operations, it will keep track of all of the entites used in each of those operations.
        static void Main(string[] args)
        {
            //InsertSamurai();
            //QueringSimplaeData();
            //MoreQueries();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            //QueryAndUpdateBattle_Disconnected();
            //InsertNewPkFkGraph();
            //InsertNewPkFkGraphMultipleChildren();
            //AddChildToExistingObjectWhileTracked();
            //AddChildToExistingObjectWhileNotTracked();
            AddChildToExistingObjectWhileNotTracked(1);
            Console.ReadLine();
        }

        private static void AddChildToExistingObjectWhileNotTracked(int samuraiId)
        {
            // no need to retrieve the samurai object, we just need its id to be used as foreign key when we are adding a quote for this samurai
            var quote = new Quote
            {
                Text = "Now Palestine is freed",
                SamuraiId = samuraiId
            };
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void AddChildToExistingObjectWhileNotTracked()
        {
            // here we have disconnected scenario, where initially we have context that track the samurai
            // then we have newContext, that is not tracking the samurai retreived object and it did not know anything about it.
            // so in this case you have to set the foreign key in quote by yourself (note in "AddChildToExistingObjectWhileTracked" you did not set the foregin key because the object is already tracked)
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(new Quote { Text = "I am Happy" });
            using (var newContext = new SamuraiContext())
            {
                // below line is WRONG, because newContext did not know anything about samurai object, it is not tracking it
                // and when you try to execute below line DB will throw an exception because there is already a samurai with this id
                // to solve this problem, look at the above method
                // newContext.Samurais.Add(samurai);
            }
        }

        private static void AddChildToExistingObjectWhileTracked()
        {
            // here we are retreiving a samurai from database, and then adding new quote to it.
            // here in this case the context is still tracking the samurai, so it knows that i added a new quotes for the tracked samurai.
            // also in quote, we did not set the foreign key value of the samurai, because the context is already tracking it
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(new Quote { Text = "I am Happy" });
            _context.SaveChanges();
        }

        private static void InsertNewPkFkGraphMultipleChildren()
        {
            var samurai = new Samurai
            {
                Name = "Mohamed Omar",
                Quotes = new List<Quote> { 
                    new Quote { Text = "I've Come To Save You" },
                    new Quote { Text = "Palestine will be freed one day" }
                }
            };

            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewPkFkGraph()
        {
            // note that the relation between Samurai and Quotes are 1 to many
            // when you check the logs, you will find that there are 2 inserts, the first one is the insert to the "Samurai" table, and get the Id
            // then another Insert into the "Quotes" table and it will use previous retreieved id as the foreign key
            // so the insert is done in 2 steps not a batch insert
            var samurai = new Samurai
            {
                Name = "Mohamed Omar",
                Quotes = new List<Quote> { new Quote { Text = "I've Come To Save You" } }
            };

            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Mohamed");
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void DeleteWhileNotTracked()
        {
            // disconnected scenario
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Mohamed"));
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Remove(samurai);
                newContext.SaveChanges();
            }
        }

        private static void DeleteMany()
        {
            var samurais = _context.Samurais.Where(s => s.Name.Contains("Mohamed"));
            _context.Samurais.RemoveRange(samurais);
            _context.SaveChanges();
        }

        private static void DeleteUsingId(int samuraiId)
        {
            // first you query the database using "Find" to retrieve the object so you can pass it into remove method
            // it is not good because there are 2 trips to the database
            var samurai = _context.Samurais.Find(samuraiId);
            _context.Remove(samurai);
            _context.SaveChanges();

            // there is alternate method is to call a stored procedure and pass a parameter
            // _context.Database.ExecuteSqlCommand("exec DeleteById {0}", samuraiId)
            // or you can use also DbSet.FromSql()
        }

        private static void InsertBattle()
        {
            _context.Battles.Add(new Battle { Name = "Battle Of Hettin", StartDate = new DateTime(1560, 05, 01), EndDate = new DateTime(1560, 06, 15) });
            _context.SaveChanges();
        }
        private static void QueryAndUpdateBattle_Disconnected()
        {
            // here we are emulating a "disconnected scenario" where we are requesting a batlle in one context "below one" and modify it,
            // then use a brand new context "newContextInstance" to push the changes to the database
            var battle = _context.Battles.FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);

            using (var newContextInstance = new SamuraiContext())
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += " Mohamed";
            _context.Samurais.Add(new Samurai { Name = "Omar" });
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(s => s.Name += " Mohamed");
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name = "Mohamed " + samurai.Name;
            _context.SaveChanges();
        }

        private static void MoreQueries()
        {
            var name = "mohamed";
            var samurai1 = _context.Samurais.Where(s => s.Name == "Julie").ToList();
            var samurai2 = _context.Samurais.Where(s => s.Name == name).ToList();
            var samurai3 = _context.Samurais.FirstOrDefault(s => s.Name == name);

            // retrieving an object by using its key value. it is not a LINQ method, it is a "Dbset" method that will execute right a way.
            // it has a great benefit in that if the object with that is already in the memory, and being tracked by "DbContext", EF will not waste time to execute the query on the database, and it will return the object that it is already tracking.
            var samurai4 = _context.Samurais.Find(2);

            var samurai5 = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "m%")).ToList();

            // note: if you are using .Last() or .LastORdEFAULT(), thes methods need to be sorted, because it will construct SQL query with "descending" sort and the "Select top 1"
            var lastSamurai1 = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);

            // if you do not include ordering, EF is not going to construct SQL Query that makes the ordering, and select top 1, so what it does is return the full result of the 
            // query into memory, and then in memorY EF will pick the last item, and if you have alot of results, this will make performance issue.

            // below line is not accepted in EF3
            //var lastSamurai2 = _context.Samurais.LastOrDefault(s => s.Name == name);
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            var smallSamurai = new Samurai { Name = "mohamed" };
            var battle = new Battle{ Name = "Hettin", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

            // below add command makes the "context" track the samurai object
            _context.AddRange(samurai, smallSamurai, battle);

            // internal workflow is as follows:
            //1- Examine each tracked object
            //2- Read state of object
            //3- workout sql commands(construct correct sql statements)
            //4- execute each sql command in the database
            //5- capture any returned data
            _context.SaveChanges();

        }

        private static void QueringSimplaeData()
        {

            // will retrieve all samurais and insert them in the memory
            var allSamurais = _context.Samurais.ToList();

            //this is another solution, but it will keep the connection to the database opened untill we retrieve all the data
            foreach (var samurai in _context.Samurais)
            {
                Console.WriteLine(samurai.Name);
            }

        }
    }
}
