using DSharpPlus.CommandsNext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static InteractivityHelpers.InteractivityEventTracker;

namespace InteractivityHelpers.Entities
{
    public class InteractivityOperation<T> : ICollection<Interaction<T>> where T : ResultEntity, new()
    {
        private readonly List<Interaction<T>> map = new List<Interaction<T>>();

        public TimeSpan? OperationDefaultTimeOutOverride { get; private set; }
        public InteractivityOperation(TimeSpan? operationDefaultTimeoutOverride = null)
        {
            this.OperationDefaultTimeOutOverride = operationDefaultTimeoutOverride;
        }

        public int Count => map.Count;

        public bool IsReadOnly => false;

        public void Add(Interaction<T> obj)
        {
            this.map.Add(obj);
        }

        public void Clear() => map.Clear();

        public bool Contains(Interaction<T> item) => map.Contains(item);

        public void CopyTo(Interaction<T>[] array, int arrayIndex) => map.CopyTo(array, arrayIndex);


        public bool Remove(Interaction<T> item) => map.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Interaction<T>> GetEnumerator() => map.GetEnumerator();


        public async Task<OperationResult<T>> ExecuteAsync(CommandContext ctx)
        {
            OperationResult<T> operationResult = new OperationResult<T>();
            try
            {
                var tracker = new InteractivityEventTracker(ctx, OperationDefaultTimeOutOverride);
                foreach (Interaction<T> interaction in map)
                {
                    object obj;
                    try
                    {
                        obj = await tracker.DoInteractionAsync(interaction);
                        if (tracker.Status != InteractivityStatus.OK)
                        {
                            operationResult.Status = tracker.Status;
                            break;
                        }
                        operationResult.Add(obj, interaction.PropertyMap);
                    }
                    catch (Exception e) // TODO Remove
                    {
                        throw;
                    }
                    //operationResult.Add(tracker.Status == InteractivityStatus.OK ? obj : null, interaction.PropertyMap);
                }
            }
            finally
            {
                InteractionAttribute.UserInteractiveChannels.Remove((ctx.User.Id, ctx.Channel.Id));
            }
            return (operationResult);
        }
    }
}
