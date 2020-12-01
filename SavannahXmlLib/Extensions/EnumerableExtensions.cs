using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SavannahXmlLib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable source, Func<TSource, TResult> selector)
        {
            var converted = source.OfType<TSource>();
            foreach (var item in converted)
            {
                yield return selector(item);
            }
        }

        public static IEnumerable<TResult> ToGeneric<TResult>(this IEnumerable source)
        {
            if (source == null)
                return null;
            return source.OfType<TResult>();
        }
    }
}
