namespace game_InfinityHex.UI
{
    /// <summary>
    /// Manages a collection of textual hints, allowing dynamic addition and randomized retrieval.
    /// This manager is thread-safe and can be shared across data processing and graphics threads.
    /// </summary>
    public class HintsManager
    {
        private readonly System.Collections.Generic.List<string> Hints;
        private int Index;
        private readonly System.Random Random;
        private readonly System.Threading.Lock Lock;
        /// <summary>
        /// Initializes a new instance of the <see cref="HintsManager"/> class.
        /// If initial hints are provided, the starting index is randomly chosen.
        /// </summary>
        /// <param name="hints">An optional collection of initial hints. If null, an empty hint list is initialized.</param>
        public HintsManager(System.Collections.Generic.IEnumerable<string>? hints = null)
        {
            Random = new();
            Lock = new();
            if (hints == null)
            {
                Hints = [];
                Index = 0;
            }
            else
            {
                Hints = [.. hints];
                Index = Hints.Count > 0 ? Random.Next(Hints.Count) : 0;
            }
        }
        /// Adds a new hint to the internal collection.
        /// </summary>
        /// <param name="hint">The hint text to add. If null or empty, the hint is ignored.</param>
        public void AddHint(string hint)
        {
            if (!string.IsNullOrEmpty(hint))
            {
                lock (Lock)
                {
                    Hints.Add(hint);
                }
            }
        }
        /// <summary>
        /// Retrieves a hint from the collection.
        /// <para>
        /// Return according to the number of hints currently in the <see cref="HintsManager"/>.
        /// If no hints exist, a single space character (" ") is always returned.
        /// If only one hint exists, it is always returned.
        /// For multiple hints, an non-repeated hint is always returned.
        /// </para>
        /// This method ensures if there are more than one hint, consecutive calls to it will return different hints.
        /// </summary>
        /// <returns>
        /// A hint string randomly selected from the list. If no hints are available, returns a single space character (" ").
        /// </returns>
        public string GetHint()
        {
            lock (Lock)
            {
                int count = Hints.Count;
                if (count == 0)
                {
                    return " ";
                }
                if (count == 1)
                {
                    return Hints[0];
                }
                else
                {
                    Index = (Index + Random.Next(1, count)) % count;
                    return Hints[Index];
                }
            }
        }
    }
}