using System;

namespace dsEngine
{
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

        public string Vin { get; }
        public string Stock { get; }
        public UInt32? Refno { get; }
        public Condition Cond { get; }
        
        public string Description { get => $"{year} {make} {model}"; }

        public int CompareTo(Vehicle other)
        {
            if (other == null) { throw new ArgumentNullException(); }

            return stock.CompareTo(other.stock);
        }
    }
}
