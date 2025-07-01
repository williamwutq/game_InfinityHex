namespace Core
{
    /// <summary>
    /// A thread-safe generic class that associates an object of type T with a time value.
    /// The time value can be used to track age or duration, and the class provides methods
    /// to manipulate this time while ensuring thread safety. Implements cloning, equality comparison,
    /// and ordering based on time and object comparison.
    /// </summary>
    /// <typeparam name="T">The type of the object to be stored.</typeparam>
    public class TimedObject<T> : System.ICloneable, System.IEquatable<TimedObject<T>>, System.IComparable<TimedObject<T>>
    {
        private readonly T obj;
        private int timed;
        private readonly System.Threading.Lock lockObject = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedObject{T}"/> class with the specified object
        /// and an initial time of 0.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>
        public TimedObject(T obj)
        {
            this.obj = obj;
            timed = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedObject{T}"/> class with the specified object
        /// and initial time value.
        /// </summary>
        /// <param name="obj">The object to be stored.</param>
        /// <param name="time">The initial time value.</param>
        public TimedObject(T obj, int time)
        {
            this.obj = obj;
            timed = time;
        }
        /// <summary>
        /// Gets the current time value associated with the object in a thread-safe manner.
        /// </summary>
        /// <returns>The current time value.</returns>
        public int GetTime()
        {
            lock (lockObject)
            {
                return timed;
            }
        }
        /// <summary>
        /// Sets the time value associated with the object in a thread-safe manner.
        /// </summary>
        /// <param name="time">The new time value to set.</param>
        public void SetTime(int time)
        {
            lock (lockObject)
            {
                timed = time;
            }
        }
        /// <summary>
        /// Resets the time value to 0 in a thread-safe manner.
        /// </summary>
        public void Renew()
        {
            lock (lockObject)
            {
                timed = 0;
            }
        }
        /// <summary>
        /// Increments the time value by 1 in a thread-safe manner.
        /// </summary>
        public void Age()
        {
            lock (lockObject)
            {
                timed++;
            }
        }
        /// <summary>
        /// Increments the time value by the specified amount in a thread-safe manner.
        /// </summary>
        /// <param name="time">The amount to increment the time value by.</param>
        public void Age(int time)
        {
            lock (lockObject)
            {
                timed += time;
            }
        }
        /// <summary>
        /// Gets the stored object.
        /// </summary>
        /// <returns>The stored object of type T.</returns>
        public T GetObject()
        {
            return obj;
        }
        /// <summary>
        /// Creates a shallow copy of the current <see cref="TimedObject{T}"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TimedObject{T}"/> with the same object reference and time value.</returns>
        public object Clone()
        {
            lock (lockObject)
            {
                return new TimedObject<T>(obj, timed);
            }
        }
        /// <summary>
        /// Determines whether the specified <see cref="TimedObject{T}"/> is equal to the current instance.
        /// Equality is based on both the time value and the stored object.
        /// </summary>
        /// <param name="other">The <see cref="TimedObject{T}"/> to compare with the current instance.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public bool Equals(TimedObject<T>? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            lock (lockObject)
            {
                return timed == other.GetTime() && Equals(obj, other.GetObject());
            }
        }
        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            else return Equals((TimedObject<T>)obj);
        }
        /// <summary>
        /// Returns a hash code for the current <see cref="TimedObject{T}"/> instance.
        /// </summary>
        /// <returns>A hash code combining the time value and the stored object's hash code.</returns>
        public override int GetHashCode()
        {
            lock (lockObject)
            {
                return System.HashCode.Combine(timed, obj);
            }
        }
        /// <summary>
        /// Compares the current <see cref="TimedObject{T}"/> with another instance.
        /// Comparison is first based on the time value. If times are equal and the stored object
        /// implements <see cref="IComparable{T}"/>, the objects are compared.
        /// </summary>
        /// <param name="other">The <see cref="TimedObject{T}"/> to compare with the current instance.</param>
        /// <returns>A value indicating the relative order of the objects being compared.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public int CompareTo(TimedObject<T>? other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            lock (lockObject)
            {
                int timeComparison = timed.CompareTo(other.GetTime());
                if (timeComparison != 0)
                    return timeComparison;

                if (obj is System.IComparable<T> comparable)
                {
                    return comparable.CompareTo(other.GetObject());
                }
                return 0; // If objects are not comparable, consider them equal if times are equal
            }
        }
        /// <summary>
        /// Returns a string that represents the current <see cref="TimedObject{T}"/>.
        /// </summary>
        /// <returns>A string containing the time value and the stored object's string representation.</returns>
        public override string ToString()
        {
            lock (lockObject)
            {
                return $"TimedObject<{typeof(T).Name}>(time={timed}, object={obj})";
            }
        }
    }
}