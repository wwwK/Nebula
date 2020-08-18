using System;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Core.Extensions
{
    public static class ListExts
    {
        public static IEnumerator<T> SelectMany<T>(this IList list, Predicate<object> predicate)
        {
            foreach (object obj in list)
            {
                if (obj is T t && predicate(obj))
                    yield return t;
            }
        }
    }
}