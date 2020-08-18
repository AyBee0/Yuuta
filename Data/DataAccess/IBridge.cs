using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataAccess
{
    /// <summary>
    /// Bridge between Discord objects and the DAL
    /// </summary>
    /// <typeparam name="E">EFCore POCO/Entity</typeparam>
    /// <typeparam name="D">Discord Object</typeparam>
    public interface IBridge<E,D>
    {
        void AddEntity(D obj, bool singleCheck = true);
        bool Exists(D obj, out E found);
        E GetByDObject(D obj, bool createIfNew = true);
        E GetByDid(ulong id);

    }
}
