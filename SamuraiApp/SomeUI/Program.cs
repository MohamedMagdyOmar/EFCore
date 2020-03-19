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
            MoreQueries();
            Console.ReadLine();
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
