using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Data
{
    public class ConnectedData
    {
        private SamuraiContext _context;

        public ConnectedData()
        {
            _context = new SamuraiContext();
            _context.Database.Migrate();
        }

        public Samurai CreateNewSamurai()
        {
            var samurai = new Samurai { Name = "New Samurai" };
            _context.Samurais.Add(samurai);
            return samurai;
        }

        public ObservableCollection<Samurai> SamuraiListInMemory()
        {
            // return list of Samurai object that are already in memory
            // check if the context is tracking any of the samurai or not
            if(_context.Samurais.Local.Count == 0)
            {
                // if we are not tracking samurai, then query the database to get list of samurais
                _context.Samurais.ToList();
            }

            // we create a copy of these samurais object, this is because we do not need to bind directly those tracked objects.
            // this will create 2 way relationship with the context, so if yoy do add or delete object in the user interface, the context is informed and vice versa
            return _context.Samurais.Local.ToObservableCollection();
        }

        public Samurai LoadSamuraiGraph(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            // Reference let me load a reference navigation property, in other word a property that points to a single object
            // i need to get the secretIdentity for the samurai, and load will go to the database and pull back that data
            _context.Entry(samurai).Reference(s => s.SecretIdentity).Load();
            _context.Entry(samurai).Collection(s => s.Quotes).Load();

            return samurai;
        }
    }
}
