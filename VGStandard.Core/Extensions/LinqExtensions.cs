using System.Collections.ObjectModel;

namespace VGStandard.Core.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
        int i = 0;
        var splits = from item in list
                     group item by i++ % parts into part
                     select part.AsEnumerable();
        return splits;
    }

    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
    {
        T[] array = null;
        int count = 0;
        foreach (T item in source)
        {
            if (array == null)
            {
                array = new T[size];
            }
            array[count] = item;
            count++;
            if (count == size)
            {
                yield return new ReadOnlyCollection<T>(array);
                array = null;
                count = 0;
            }
        }
        if (array != null)
        {
            Array.Resize(ref array, count);
            yield return new ReadOnlyCollection<T>(array);
        }
    }

    public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            yield break;
        }

        foreach (var item in source)
        {
            yield return item;
        }
    }

    public static IEnumerable<List<object>> PartitionByType(this List<object> values)
    {
        int count = 0;
        return values.Select((o, i) => new
        {
            Object = o,
            Group = (i == 0 || o.GetType() == values[i - 1].GetType()) ? count : ++count
        })
            .GroupBy(x => x.Group)
            .Select(g => g.Select(x => x.Object).ToList());
    }


    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
