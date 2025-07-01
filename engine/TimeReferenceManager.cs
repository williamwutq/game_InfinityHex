namespace Core
{
    /// <summary>
    /// A class that manages a centralized time reference for a collection of <see cref="TimedObject{object}"/> instances,
    /// enabling efficient time updates, conversions between absolute and relative time, and expiration checks.
    /// </summary>
    public class TimeReferenceManager
    {
        private readonly int limit;
        private readonly int expire;
        private int currentTime;
        private HandleTimeReferenceReset? timeReferenceResetHandler;
        /// <summary>
        /// Delegate for handling time reference reset events, invoked when the time reference resets to the limit.
        /// Implementation should call <see cref="TimedObject{object}.Age(time)"/> on all 
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
        /// Instead of calling <see cref="TimedObject{object}.Age(time)"/> on every single object, large collection of <see cref="TimedObject{object}"/>
        /// can be aged by calling this method on the manager.
        /// </summary>
        public void Age()
        {
            if (currentTime == 0)
            {
                timeReferenceResetHandler?.Invoke(limit);
                currentTime = limit;
            }
            else currentTime--;
        }
        /// <summary>
        /// Resets the current time reference to the limit value.
        /// </summary>
        public void Reset()
        {
            currentTime = limit;
        }
        /// <summary>
        /// Gets the current time reference.
        /// </summary>
        /// <returns>The current time reference value.</returns>
        public int GetTime()
        {
            return currentTime;
        }
        /// <summary>
        /// Converts an absolute time to a relative time by subtracting the current time reference.
        /// </summary>
        /// <param name="absoluteTime">The absolute time to convert.</param>
        /// <returns>The relative time value.</returns>
        public int ToRelative(int absoluteTime)
        {
            return absoluteTime - currentTime;
        }
        /// <summary>
        /// Converts a relative time to an absolute time by adding the current time reference.
        /// </summary>
        /// <param name="relativeTime">The relative time to convert.</param>
        /// <returns>The absolute time value.</returns>
        public int ToAbsolute(int relativeTime)
        {
            return relativeTime + currentTime;
        }
        /// <summary>
        /// Converts the time of a <see cref="TimedObject{object}"/> from absolute to relative by subtracting the current time reference.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{object}"/> with an absolute time value.</param>
        public void ToRelative(TimedObject<object> absoluteTimedObject)
        {
            absoluteTimedObject.Age(-currentTime);
        }
        /// <summary>
        /// Converts the time of a <see cref="TimedObject{object}"/> from relative to absolute by adding the current time reference.
        /// </summary>
        /// <param name="relativeTimedObject">The <see cref="TimedObject{object}"/> with a relative time value.</param>
        public void ToAbsolute(TimedObject<object> relativeTimedObject)
        {
            relativeTimedObject.Age(currentTime);
        }
        /// <summary>
        /// Creates a new <see cref="TimedObject{object}"/> with the current absolute time.
        /// Instead of calling <see cref="TimedObject{object}"/> constructor, large collection of TimedObject
        /// should call this construction method instead.
        /// </summary>
        /// <param name="obj">The object to store in the <see cref="TimedObject{object}"/>.</param>
        /// <returns>A new <see cref="TimedObject{object}"/> with the current absolute time.</returns>
        public TimedObject<object> ConstructAbsoluteTimedObject(object obj)
        {
            return new TimedObject<object>(obj, currentTime);
        }
        /// <summary>
        /// Creates a new <see cref="TimedObject{object}"/> with an absolute time based on the current time reference plus a relative time.
        /// Instead of calling <see cref="TimedObject{object}"/> constructor, large collection of TimedObject
        /// should call this construction method instead.
        /// </summary>
        /// <param name="obj">The object to store in the <see cref="TimedObject{object}"/>.</param>
        /// <param name="relativeTime">The relative time to add to the current time reference.</param>
        /// <returns>A new <see cref="TimedObject{object}"/> with the calculated absolute time.</returns>
        public TimedObject<object> ConstructAbsoluteTimedObject(object obj, int relativeTime)
        {
            return new TimedObject<object>(obj, currentTime + relativeTime);
        }
        /// <summary>
        /// Sets the time of a <see cref="TimedObject{object}"/> to the current absolute time, effectively renewing it.
        /// Instead of calling <see cref="TimedObject{object}.Renew()"/> to renew objects, large collection of TimedObject
        /// should call this renew method instead.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{object}"/> to renew.</param>
        public void Renew(TimedObject<object> absoluteTimedObject)
        {
            absoluteTimedObject.SetTime(currentTime);
        }
        /// <summary>
        /// Sets the time of a <see cref="TimedObject{object}"/> to an absolute time based on the current time reference plus a relative time.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{object}"/> to update.</param>
        /// <param name="relativeTime">The relative time to add to the current time reference.</param>
        public void SetRelativeTime(TimedObject<object> absoluteTimedObject, int relativeTime)
        {
            absoluteTimedObject.SetTime(currentTime + relativeTime);
        }
        /// <summary>
        /// Gets the relative expiration threshold.
        /// </summary>
        /// <returns>The expiration duration in relative time.</returns>
        public int GetRelativeExpire()
        {
            return expire;
        }
        /// <summary>
        /// Gets the absolute expiration threshold based on the current time reference.
        /// </summary>
        /// <returns>The expiration time in absolute terms.</returns>
        public int GetAbsoluteExpire()
        {
            return expire + currentTime;
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
        /// Determines whether a <see cref="TimedObject{object}"/> has expired based on its absolute time.
        /// </summary>
        /// <param name="absoluteTimedObject">The <see cref="TimedObject{object}"/> to check.</param>
        /// <returns>True if the object has expired; otherwise, false.</returns>
        public bool IsExpired(TimedObject<object> absoluteTimedObject)
        {
            return absoluteTimedObject.GetTime() > GetAbsoluteExpire();
        }
    }
}