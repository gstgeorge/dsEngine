using System;

namespace dsEngine
{
    /// <summary>
    /// Stores vehicle data for use in creating a line item on a work summary report.
    /// </summary>
    internal class Vehicle : IComparable<Vehicle>
    {
        /// <summary>
        /// The vehicle's condition.
        /// </summary>
        public enum Condition { USED, NEW };


        // Private fields.
        private string _vin;
        private string _stock;
        private UInt32? _refno;
        private Condition _cond;
        private short _year;
        private string _make;
        private string _model;


        /// <summary>
        /// Initializes a new instance of the Vehicle class.
        /// </summary>
        /// <param name="vin">The vehicle's VIN number.</param>
        /// <param name="stock">The vehicle's stock number.</param>
        /// <param name="condition">The vehicle's condition.</param>
        /// <param name="year">The vehicle's model year.</param>
        /// <param name="make">The vehicle's manufacturer.</param>
        /// <param name="model">The vehicle's model name.</param>
        /// <param name="refno">An optional reference number.</param>
        public Vehicle(string vin, string stock, Condition condition, short year, string make, string model, uint? refno = null)
        {
            _vin = vin;
            _stock = stock;
            _refno = refno;
            _cond = condition;
            _year = year;
            _make = make;
            _model = model;
        }


        /// <summary>
        /// The vehicle's VIN number.
        /// </summary>
        public string Vin { get => _vin; }

        /// <summary>
        /// The vehicle's stock number.
        /// </summary>
        public string Stock { get => _stock; }

        /// <summary>
        /// An optional reference number that may be supplied by the vendor.
        /// </summary>
        public UInt32? Refno { get => _refno; }

        /// <summary>
        /// The vehicle's condition.
        /// </summary>
        public Condition Cond { get => _cond; }

        /// <summary>
        /// The vehicle's line item description to be printed on a work summary report.
        /// </summary>
        public string Description { get => $"{_year} {_make} {_model}"; }


        // Override ToString
        public override string ToString()
        {
            return $"{_stock} ({Description})";
        }

        // Implement IComparable interface
        public int CompareTo(Vehicle other)
        {
            if (other == null) { throw new ArgumentNullException(); }

            // If the VINs match, then the vehicles are equal, regardless of any other attribute.
            if (_vin.Equals(other.Vin))
            {
                return 0;
            }

            // If the vehicles are of like condition, sort based on stock number.
            else if (_cond.Equals(other.Cond))
            {
                return _stock.CompareTo(other._stock);
            }

            // If the vehicles are not of like condition, place the Used vehicle first.
            else return _cond.CompareTo(other._cond);
        }
    }
}
