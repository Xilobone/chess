using System.Numerics;
using System.Reflection.PortableExecutable;

namespace counters
{
    /// <summary>
    /// Represents a counter object which can be used to easily keep track of statistics,
    /// remembers previously stored values
    /// </summary>
    /// <typeparam name="T">The number type of the counter</typeparam>
    public class Counter<T> : ICounter where T : struct, INumber<T>
    {
        private string name;
        private string unit;
        private T value;

        private List<T> history;

        /// <summary>
        /// Creates a new counter with a specified name
        /// </summary>
        /// <param name="name">The name of the counter, used when displaying an overview</param>
        public Counter(string name) : this(name, "") { }

        /// <summary>
        /// Creates a new counter with a specified name
        /// </summary>
        /// <param name="name">The name of the counter, used when displaying an overview</param>
        /// <param name="unit">The unit of the coutner, used when displaying an overview</param>
        public Counter(string name, string unit)
        {
            this.name = name;
            this.unit = unit;
            value = default;

            history = new List<T>();
        }
        
        /// <summary>
        /// Increases the value of the counter by one
        /// </summary>
        public void Increment()
        {
            value += T.One;
        }

        /// <summary>
        /// Increases the value of the counter by the specified amount
        /// </summary>
        /// <param name="amount">The amount to increase the counter by</param>
        public void Increment(T amount)
        {
            value += amount;
        }

        /// <summary>
        /// Decreases the value of the counter by one
        /// </summary>
        public void Decrement()
        {
            value -= T.One;
        }

        /// <summary>
        /// Decreases the value of the counter by the specified amount
        /// </summary>
        /// <param name="amount">The amount to decrease the counter by</param>
        public void Decrement(T amount)
        {
            value -= amount;
        }

        /// <summary>
        /// Sets the value of the counter to be a specified value
        /// </summary>
        /// <param name="value">The value to set the counter to</param>
        public void Set(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Resets the value of the counter and adds it to its history
        /// </summary>
        public void Reset()
        {
            history.Add(value);
            value = default;
        }

        /// <summary>
        /// Removes all stored information of the counter, clears the history and sets the value to zero
        /// </summary>
        public void FullReset()
        {
            history.Clear();
            value = default;
        }

        /// <summary>
        /// Displays the value to the console
        /// </summary>
        public void DisplayValue()
        {
            Console.WriteLine($"{name}: {value}{unit}");
        }

        /// <summary>
        /// Displays an overview of the values in the history
        /// </summary>
        public void DisplayOverview()
        {
            if (history.Count == 0)
            {
                Console.WriteLine($"{name}: no values to display");
                return;
            }

            T min = history[0];
            T max = history[0];
            T count = T.CreateChecked(history.Count);
            T sum = T.Zero;

            foreach(T v in history)
            {
                if (v < min) min = v;
                if (v > max) max = v;

                sum += v;
            }

            T avg = sum / count;

            T stdDevSum = T.Zero;

            foreach(T v in history)
            {
                stdDevSum += (v - avg) * (v - avg);
            }

            T stdDev = T.Zero;
            if (count > T.One) stdDev = Sqrt(stdDevSum / (count - T.One));

            Console.WriteLine($"{name}:");
            Console.WriteLine($"    - count: {count}");
            Console.WriteLine($"    - avg: {avg}{unit}");
            Console.WriteLine($"    - stdDev: {stdDev}{unit}");
            Console.WriteLine($"    - min: {min}{unit}");
            Console.WriteLine($"    - max: {max}{unit}");

        }

        /// <summary>
        /// Calculates the square root of a value
        /// </summary>
        /// <param name="value">The value to calculate the square root of</param>
        /// <returns>The square root of the value</returns>
        private static T Sqrt(T value)
        {
            double v = Convert.ToDouble(value);
            double sqrt = Math.Sqrt(v);
            return T.CreateChecked(sqrt);
        }
        


    }
}