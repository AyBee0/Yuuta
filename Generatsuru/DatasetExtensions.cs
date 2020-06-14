using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Generatsuru
{
    public static class DatasetExtensions
    {
        public static void SeedEnumValues<T, TEnum>(this ModelBuilder mb, Func<TEnum, T> converter) 
            where T : class where TEnum : Enum 
            =>
                Enum.GetValues(typeof(TEnum))
                    .Cast<object>()
                    .Select(value => converter((TEnum)value))
                    .ToList()
                    .ForEach(instance => mb.Entity<T>().HasData(instance));
    }
}
