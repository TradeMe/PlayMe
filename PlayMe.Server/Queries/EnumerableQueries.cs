using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;

namespace PlayMe.Server.Queries
{
    public static class EnumerableQueries
    {
        public static T GetRandomItem<T>(this IEnumerable<T> source) 
            where T:PlayMeObject
        {
            int count=source.Count();
            if (count==0)
                return null;
            int index = new Random().Next(count); // up to count -1     
            return source.ElementAt(index);
        }

        public static IEnumerable<T> GetRandomItems<T>(this IEnumerable<T> source, int numToGet) 
            where T : PlayMeObject
        {
            int sourceSize = source.Count();
            if (sourceSize == 0)
                return null;

            int count = 0;
            var result = new HashSet<T>();
            while(count < numToGet && count < sourceSize)
            {
                if (result.Add(source.GetRandomItem()))
                    count++;
            }

            return result;
        }
    }
}
