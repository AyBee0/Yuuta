using DataAccessLayer.Models;
using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccessLayer.DataAccess
{
    public abstract class DAL<E, D> : IBridge<E, D>  where E : class
    {

        private readonly PropertyInfo property;
        protected abstract Expression<Func<YuutaDbContext, DbSet<E>>> Property { get; set; }
        protected abstract Func<E, D> ConvertEntityToDiscordObject { get; }
        protected abstract Func<D, E> ConvertDiscordObjectToEntity { get; }

        public void AddEntity(D obj, bool singleCheck = true)
        {
            using var db = new YuutaDbContext();
            if (((DbSet<E>)property.GetValue(db)).SingleOrDefault(x => ))
            {
                return;
            }
            db.Commands.Add(new YuutaCommand(obj));
            db.SaveChanges();
        }

        public bool Exists(Command obj, out YuutaCommand found)
        {
            found = GetByDObject(obj, false);
            return found != null;
        }

        public YuutaCommand GetByDid(ulong id)
        {
            throw new InvalidOperationException("Discord commands don't have IDs");
        }

        public YuutaCommand GetByDObject(Command obj, bool createIfNew = false)
        {
            using var db = new YuutaDbContext();
            YuutaCommand found = db.Commands.SingleOrDefault(x => x.CommandName == obj.Name);
            if (found == null && createIfNew)
            {
                found = new YuutaCommand(obj);
                db.Commands.Add(found);
            }
            return found;
        }
    }
}
    