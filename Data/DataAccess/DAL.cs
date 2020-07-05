using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataAccess
{
    public abstract class DAL : IDisposable
    {
        protected YuutaDbContext Database { get; } = new YuutaDbContext();
        public void Dispose()
        {
            Database.SaveChanges();
            Database.Dispose();
        }
    }
}
    