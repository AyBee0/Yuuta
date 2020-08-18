using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractivityHelpers.Entities
{
    public abstract class ResultEntity
    {
        public InteractivityStatus Status { get; internal set; }
    }
}
