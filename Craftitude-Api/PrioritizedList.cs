using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craftitude
{
    public class PrioritizedList<PriorityT, ValueT>
        where PriorityT: IComparable
        where ValueT: IComparable
    {
        public void Add(PriorityT priority, ValueT value)
        {
            list.Add(new Tuple<PriorityT, ValueT>(priority, value));
        }

        /*

        public void Remove(ValueT value)
        {
            list.RemoveAll(i => i.Item2.Equals(value));
        }

        public void Clear()
        {
            list.Clear();
        }
        
         */

        private static int CompareEntries(Tuple<PriorityT, ValueT> a, Tuple<PriorityT, ValueT> b)
        {
            return a.Item1.CompareTo(b.Item1);
        }

        public IEnumerable<ValueT> GetSortedList()
        {
            list.Sort(CompareEntries);
            return list.Select(a => a.Item2);
        }

        private List<Tuple<PriorityT, ValueT>> list = new List<Tuple<PriorityT, ValueT>>();
    }
}
