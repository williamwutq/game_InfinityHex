namespace Core
{
    /// <summary>
    /// A class that manages a centralized time reference for a collection of <see cref="TimedObject{Object}"/> instances,
    /// enabling efficient time updates, conversions between absolute and relative time, and expiration checks.
    /// This manager is thread-safe, and it can maintain global time reference consistency across threads.
    /// However, it is advisable to sync behavior of different threads to reduce the possibility of unintended Age calls.
    /// </summary>
    public class TimeReferenceManager
    {
        private readonly int limit;
        private readonly int expire;
        private readonly System.Threading.Lock lockObject = new();
        private int currentTime;
        private HandleTimeReferenceReset? timeReferenceResetHandler;
        /// <summary>
        /// Delegate for handling time reference reset events, invoked when the time reference resets to the limit.
        /// Implementation should call <see cref="TimedObject{T}.Age(time)"/> on all 
        /// </summary>
        /// <param name="time">The limit value to which the time reference is reset.</param>
        public delegate void HandleTimeReferenceReset(int time);
        /// <summary>
        /// Sets the handler for time reference reset events.
        /// </summary>
        /// <param name="timeResetHandler">The delegate to invoke when the time reference resets.</param>
        public void SetTimeResetHandler(HandleTimeReferenceReset timeResetHandler)
        {
            this.timeReferenceResetHandler = timeResetHandler;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeReferenceManager"/> class with the specified limit and expiration values.
        /// </summary>
        /// <param name="limit">The maximum time value before the time reference resets.</param>
        /// <param name="expire">The duration after which objects are considered expired.</param>
        public TimeReferenceManager(int limit, int expire)
        {
            this.limit = limit;
            this.expire = expire;
            this.currentTime = 0;
        }
        /// <summary>
        /// Decrements the current time reference. If the time reaches 0, it resets to the limit and invokes the reset handler.
        /// Instead of calling <see cref="TimedObject{T}.Age(time)"/> on every single object, large collection of <see cref="TimedObject{Object}"/>
        /// can be aged by calling this method on the manager.
        /// </summary>
        public void Age()
        {
            lock (lockObject)
            {
                if (currentTime == 0)
                {
                    timeReferenceResetHandler?.Invoke(limit);
                    currentTime = limit;
                }
                else currentTime--;
            }
        }
        /// <summary>
        /// Resets the current time reference to the limit value.
        /// </summary>
        public void Reset()
        {
            lock (lockObject)
            {
                currentTime = limit;
            }
        }
        /// <summary>
        /// Gets the current time reference.
        /// </summary>
        /// <returns>The current time reference value.</returns>
        public int GetTime()
        {
            lock (lockObject)
            {
                return currentTime;
            }
        }
        /// <summary>
        /// Converts an absolute time to a relative time by subtracting the current time reference.
        /// </summary>
        /// <param name="absoluteTime">The absolute time to convert.</param>
        /// <returns>The relative time value.</returns>
        public int ToRelative(int absoluteTime)
        {
            lock (lockObject)
            {
                return absoluteTime - currentTime;
            }
        }
        /// <summary>
        /// Converts a relative time to an absolute time by adding the current time reference.
        /// </summary>
        /// <param name="relativeTime">The relative time to convert.</param>
        /// <returns>The absolute time value.</returns>
        public int ToAbsolute(int relativeTime)
        {
            lock (lockObject)
            {
                return relativeTime + currentTime;
            }
        }
        /// <summary>
        /// Converts the time of a <see cref="TimedObject{T}"/> from absolute to relative by subtracting the current time reference.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{T}"/> with an absolute time value.</param>
        /// <exception cref="ArgumentNullException">Thrown if absoluteTimedObject is null.</exception>
        public void ToRelative<T>(TimedObject<T> absoluteTimedObject)
        {
            System.ArgumentNullException.ThrowIfNull(absoluteTimedObject);
            absoluteTimedObject.Age(-GetTime());
        }
        /// <summary>
        /// Converts the time of a <see cref="TimedObject{T}"/> from relative to absolute by adding the current time reference.
        /// </summary>
        /// <param name="relativeTimedObject">The <see cref="TimedObject{T}"/> with a relative time value.</param>
        /// <exception cref="ArgumentNullException">Thrown if relativeTimedObject is null.</exception>
        public void ToAbsolute<T>(TimedObject<T> relativeTimedObject)
        {
            System.ArgumentNullException.ThrowIfNull(relativeTimedObject);
            relativeTimedObject.Age(GetTime());
        }
        /// <summary>
        /// Creates a new <see cref="TimedObject{T}"/> with the current absolute time.
        /// Instead of calling <see cref="TimedObject{T}"/> constructor, large collection of TimedObject
        /// should call this construction method instead.
        /// </summary>
        /// <param name="obj">The object to store in the <see cref="TimedObject{T}"/>.</param>
        /// <returns>A new <see cref="TimedObject{T}"/> with the current absolute time.</returns>
        public TimedObject<T> ConstructAbsoluteTimedObject<T>(T obj)
        {
            return new TimedObject<T>(obj, GetTime());
        }
        /// <summary>
        /// Creates a new <see cref="TimedObject{T}"/> with an absolute time based on the current time reference plus a relative time.
        /// Instead of calling <see cref="TimedObject{T}"/> constructor, large collection of TimedObject
        /// should call this construction method instead.
        /// </summary>
        /// <param name="obj">The object to store in the <see cref="TimedObject{T}"/>.</param>
        /// <param name="relativeTime">The relative time to add to the current time reference.</param>
        /// <returns>A new <see cref="TimedObject{T}"/> with the calculated absolute time.</returns>
        public TimedObject<T> ConstructAbsoluteTimedObject<T>(T obj, int relativeTime)
        {
            return new TimedObject<T>(obj, GetTime() + relativeTime);
        }
        /// <summary>
        /// Sets the time of a <see cref="TimedObject{T}"/> to the current absolute time, effectively renewing it.
        /// Instead of calling <see cref="TimedObject{T}.Renew()"/> to renew objects, large collection of TimedObject
        /// should call this renew method instead.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{T}"/> to renew.</param>
        /// <exception cref="ArgumentNullException">Thrown if absoluteTimedObject is null.</exception>
        public void Renew<T>(TimedObject<T> absoluteTimedObject)
        {
            System.ArgumentNullException.ThrowIfNull(absoluteTimedObject);
            absoluteTimedObject.SetTime(GetTime());
        }
        /// <summary>
        /// Sets the time of a <see cref="TimedObject{T}"/> to an absolute time based on the current time reference plus a relative time.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{T}"/> to update.</param>
        /// <param name="relativeTime">The relative time to add to the current time reference.</param>
        /// <exception cref="ArgumentNullException">Thrown if absoluteTimedObject is null.</exception>
        public void SetRelativeTime<T>(TimedObject<T> absoluteTimedObject, int relativeTime)
        {
            System.ArgumentNullException.ThrowIfNull(absoluteTimedObject);
            absoluteTimedObject.SetTime(GetTime() + relativeTime);
        }
        /// <summary>
        /// Gets the relative expiration threshold.
        /// </summary>
        /// <returns>The expiration duration in relative time.</returns>
        public int GetRelativeExpire()
        {
            lock (lockObject)
            {
                return expire;
            }
        }
        /// <summary>
        /// Gets the absolute expiration threshold based on the current time reference.
        /// </summary>
        /// <returns>The expiration time in absolute terms.</returns>
        public int GetAbsoluteExpire()
        {
            lock (lockObject)
            {
                return expire + currentTime;
            }
        }
        /// <summary>
        /// Determines whether an absolute time has expired based on the expiration threshold.
        /// </summary>
        /// <param name="absoluteTime">The absolute time to check.</param>
        /// <returns>True if the time has expired; otherwise, false.</returns>
        public bool IsExpired(int absoluteTime)
        {
            return absoluteTime > GetAbsoluteExpire();
        }
        /// <summary>
        /// Determines whether a <see cref="TimedObject{T}"/> has expired based on its absolute time.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{T}"/> to check.</param>
        /// <returns>True if the object has expired or is null; otherwise, false.</returns>
        public bool IsExpired<T>(TimedObject<T> absoluteTimedObject)
        {
            return absoluteTimedObject == null || absoluteTimedObject.GetTime() > GetAbsoluteExpire();
        }
        /// <summary>
        /// Returns a string that represents the current state of the TimeReferenceManager.
        /// </summary>
        /// <returns>A string containing the limit, expire, and current time values.</returns>
        public override string ToString()
        {
            lock (lockObject)
            {
                return $"TimeReferenceManager(time={currentTime}/{limit}, expire={expire})";
            }
        }
    }
}