namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Null touch strategy that does nothing.
    /// </summary>
    public class NullCollectionItemTouchedStrategy:ICollectionItemTouchedStrategy
    {
        public static NullCollectionItemTouchedStrategy Instance => new NullCollectionItemTouchedStrategy();

        public void IndexReferenced(int index)
        {
            // do nothing
        }

        public void CountReferenced()
        {
            // do nothing
        }
    }
}