using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayMe.Server.Extensions
{
    public static class EnumerableOfTExtension
    {
        public static IEnumerable<T> Random<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
