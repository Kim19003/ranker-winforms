using System.Collections.Generic;

namespace RankingApp.Other
{
    public enum Change { Up, Down, None, New }

    public class Helpers
    {

    }

    public static class ListExtensions
    {
        public static void Fill<T>(this List<T> list, List<T> items, bool clearBeforeFilling = false)
        {
            if (clearBeforeFilling)
            {
                list.Clear();
            }

            list.AddRange(items);
        }
    }
}
