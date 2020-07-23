using DSharpPlus.CommandsNext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InteractivityHelpers.Entities
{
    public class InteractivityOperation : ICollection<Interaction>
    {
        private readonly List<Interaction> parsers = new List<Interaction>();
        public int Count => parsers.Count;

        public bool IsReadOnly => false;

        public void Add(Interaction item) => parsers.Add(item);

        public void Clear() => parsers.Clear();

        public bool Contains(Interaction item) => parsers.Contains(item);

        public void CopyTo(Interaction[] array, int arrayIndex) => parsers.CopyTo(array, arrayIndex);

        public IEnumerator<Interaction> GetEnumerator() => parsers.GetEnumerator();

        public bool Remove(Interaction item) => parsers.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public async Task<List<dynamic>> Execute(CommandContext ctx, TimeSpan? timeout = null)
        {
            var tracker = new InteractivityEventTracker(ctx, timeout);
            List<dynamic> objs = new List<dynamic>();
            foreach (Interaction interaction in parsers)
            {
                var obj = await tracker.DoInteractionAsync(interaction);
                objs.Add(obj);
            }
            return objs; 
        }

    }
}
