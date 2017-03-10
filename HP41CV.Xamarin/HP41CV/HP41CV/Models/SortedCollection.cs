using System;
using System.Collections.ObjectModel;

namespace HP41CV.Models
{
    public class SortedCollection<T> : Collection<T>
        where T : IComparable
    {
        public SortedCollection() : base()
        {
            indexSorted = new Collection<int>();
        }
        private readonly Collection<int> indexSorted;

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            for (int i = 0; i < indexSorted.Count; i++)
                if (indexSorted[i] >= index)
                    indexSorted[i]++;
            for (int i = 0; i < indexSorted.Count; i++)
                if (base[indexSorted[i]].CompareTo(item) > 0)
                {
                    indexSorted.Insert(i, index);
                    return;
                }
            indexSorted.Add(index);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            indexSorted.Clear();
        }

        public T GetSortedAt(int index)
        {
            return base[indexSorted[index]];
        }
    }
}
