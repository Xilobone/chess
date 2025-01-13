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
        private const string BASE_FILE_PATH = "logs/counter/";

        private string name;
        private string unit;
        private T value;

        private T count;
        private T min;
        private T max;
        private T avg;
        private T stdDev;

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

            //calculate statistics
            min = history[0];
            max = history[0];
            count = T.CreateChecked(history.Count);
            T sum = T.Zero;

            foreach (T v in history)
            {
                if (v < min) min = v;
                if (v > max) max = v;

                sum += v;
            }

            avg = sum / count;

            T stdDevSum = T.Zero;

            foreach (T v in history)
            {
                stdDevSum += (v - avg) * (v - avg);
            }

            stdDev = T.Zero;
            if (count > T.One) stdDev = Sqrt(stdDevSum / (count - T.One));
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
            DisplayOverview(false);
        }

        /// <summary>
        /// Displays an overview of the values in the history
        /// </summary>
        /// <param name="showComparison">wether to show the comparison with the same named counter in the logs/counter directory</param>
        public void DisplayOverview(bool showComparison)
        {
            if (history.Count == 0)
            {
                Console.WriteLine($"{name}: no values to display");
            }

            string overview = $"{name}:\n";

            if (showComparison)
            {
                Counter<T> comparison = read<T>(name);

         
                double countDiv = getFractionDifference(count, comparison.count);
                double avgDiv = getFractionDifference(avg, comparison.avg);
                double stdDevDiv = getFractionDifference(stdDev, comparison.stdDev);
                double minDiv = getFractionDifference(min, comparison.min);
                double maxDiv = getFractionDifference(max, comparison.max);

                overview += $"    - count: {count} ({countDiv * 100}%)\n";
                overview += $"    - avg: {avg}{unit} ({avgDiv * 100}%)\n";
                overview += $"    - stdDev: {stdDev}{unit} ({stdDevDiv * 100}%)\n";
                overview += $"    - min: {min}{unit} ({minDiv * 100}%)\n";
                overview += $"    - max: {max}{unit} ({maxDiv * 100}%)";

                Console.WriteLine(overview);
                return;
            }

            overview += $"    - count: {count}\n";
            overview += $"    - avg: {avg}{unit}\n";
            overview += $"    - stdDev: {stdDev}{unit}\n";
            overview += $"    - min: {min}{unit}\n";
            overview += $"    - max: {max}{unit}";

            Console.WriteLine(overview);

        }

        //writes the history of the counter to a .cntr file
        public void write()
        {
            string path = $"{BASE_FILE_PATH}{name}.cntr";
            foreach (T value in history)
            {
                File.AppendAllText(path, value.ToString() + "\n");
            }
        }

        /// <summary>
        /// Reads a .cntr file and recreates the counter
        /// </summary>
        /// <typeparam name="S">The type of counter to create</typeparam>
        /// <param name="name">The name of the file to read</param>
        /// <returns>A counter with the same history as the file</returns>
        public static Counter<S> read<S>(string name) where S : struct, INumber<S>
        {
            string path = $"{BASE_FILE_PATH}{name}.cntr";
            string[] lines = File.ReadAllLines(path);

            Counter<S> counter = new Counter<S>(name);

            foreach (string line in lines)
            {
                S value = S.Parse(line, null);

                counter.Set(value);
                counter.Reset();
            }

            return counter;
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

        private static double getFractionDifference(T value1, T value2) {
            if (value2 == T.Zero) return 0;

            return (Convert.ToDouble(value1) - Convert.ToDouble(value2)) / Convert.ToDouble(value2);
        }



    }
}