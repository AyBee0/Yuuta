using System;
using System.Collections.Generic;
using System.Text;

namespace Commands {
    public static class Attributes {

        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
        public sealed class StaffCommands : Attribute {
            // See the attribute guidelines at 
            //  http://go.microsoft.com/fwlink/?LinkId=85236
            readonly bool Staff;

            // This is a positional argument
            public StaffCommands() {
                Staff = true;
            }

            public bool IsStaff
            {
                get { return Staff; }
            }

            // This is a named argument
            public int NamedInt { get; set; }
        }

    }
}
