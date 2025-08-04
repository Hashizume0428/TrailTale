using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int size)
    {
        List<T> chunk = new List<T>(size);
        foreach (var item in source)
        {
            chunk.Add(item);
            if (chunk.Count == size)
            {
                yield return new List<T>(chunk);
                chunk.Clear();
            }
        }
        if (chunk.Count > 0)
        {
            yield return new List<T>(chunk);
        }
    }
}

