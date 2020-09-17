using System;
using System.Collections.Generic;

namespace SavannahXmlLib.Extensions
{
    public static class LinkedListExtensions
    {
        public static LinkedListNode<T> Find<T>(this LinkedList<T> list, T target, IEqualityComparer<T> compare)
        {
            var node = list.First;
            while (node != null)
            {
                if (compare.Equals(node.Value, target))
                    return node;

                node = node.Next;
            }
            return null;
        }
    }
}
