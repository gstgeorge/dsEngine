using System;

namespace dsEngine
{
    /// <summary>
    /// Stores vehicle data
    /// </summary>
    internal class Vehicle : IComparable<Vehicle>
    {
        public enum Condition {NEW, USED};

        private string vin;
        private string stock;
        private UInt32? refno;
        private Condition cond;
        private short year;
        private string make;
        private string model;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vin">The vehicle's VIN number</param>
        /// <param name="stock">The vehicle's stock number</param>
        /// <param name="condition">The vehicle's condition</param>
        /// <param name="year">The vehicle's model year</param>
        /// <param name="make">The vehicle's manufacturer</param>
        /// <param name="model">The vehicle's model name</param>
        /// <param name="refno">An optional reference number</param>
        public Vehicle(string vin, string stock, Condition condition, short year, string make, string model, uint? refno = null)
        {
            this.vin = vin;
            this.stock = stock;
            this.refno = refno;
            this.cond = condition;
            this.year = year;
            this.make = make;
            this.model = model;
        }

        // Properties
        public string Vin { get => vin; }
        public string Stock { get => stock; }
        public UInt32? Refno { get => refno; }
        public Condition Cond { get => cond; }
        public string Description { get => $"{year} {make} {model}"; }

        // Override ToString
        public override string ToString()
        {
            return $"{stock} ({Description})";
        }

        // Implement IComparable interface
        public int CompareTo(Vehicle other)
        {
            if (other == null) { throw new ArgumentNullException(); }

            if (vin.Equals(other.Vin))
            {
                return 0;
            }

            return stock.CompareTo(other.stock);
        }
    }
}
