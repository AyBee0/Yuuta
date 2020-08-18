using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace InteractivityHelpers.Entities
{
    //public class ResultList : ICollection<object>
    //{
    //    private readonly List<object> results;

    //    private int index = 0;

    //    public object GetNext()
    //    {
    //        var result = results[index];
    //        index++;
    //        return result;
    //    }

    //    internal ResultList(List<object> results)
    //    {
    //        this.results = results ?? throw new ArgumentNullException("Results cannot be null");
    //    }

    //    public int Count => results.Count;

    //    public bool IsReadOnly => true;

    //    public void Add(object item) => throw new InvalidOperationException("Cannot modify result list");

    //    public void Clear() => throw new InvalidOperationException("Cannot modify result list");

    //    public bool Contains(object item) => results.Contains(item);

    //    public void CopyTo(object[] array, int arrayIndex)
    //    {
    //        results.CopyTo(array, arrayIndex);
    //    }
    //    public bool Remove(object item) => throw new InvalidOperationException("Cannot modify result list");

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //    public IEnumerator<object> GetEnumerator() => results.GetEnumerator();

    //}
    public class OperationResult<T> where T : ResultEntity, new()
    {
        public T Result { get; private set; }
        public InteractivityStatus Status => Result.Status;
        public OperationResult()
        {
            Result = new T();
        }
        public void Add(object value, Expression<Func<T>> property)
        {
            PropertyInfo propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            propertyInfo.SetValue(Result, value);
        }
    }
}
