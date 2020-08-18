using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Event
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public ulong CreatedByDid { get; set; }
        public bool Ongoing { get; set; }
    }
}
