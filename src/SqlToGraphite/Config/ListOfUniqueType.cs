using System.Collections.Generic;
using System.Linq;

namespace SqlToGraphite.Config
{
    public class ListOfUniqueType<T> : ICollection<T>
    {
        private readonly List<T> innerList = new List<T>();

        public void Add(T item)
        {
            this.DoesTypeExistAlready(item);
            this.innerList.Add(item);
        }

        private void DoesTypeExistAlready(T item)
        {
            if (this.innerList.Any(i => i.GetType() == item.GetType()))
            {
                throw new CannotAddAnotherInstanceOfTypeException(string.Format("Cannot add another instance of {0}", item.GetType().Name));
            }
        }

        public void Clear()
        {
            this.innerList.Clear();
        }

        public bool Contains(T item)
        {
            return this.innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return this.innerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.innerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}