using System;
using System.Collections.Generic;

namespace dsEngine
{
    /// <summary>
    /// Stores invoice data for a dealer
    /// </summary>
    internal class Invoice
    {
        private Dealer dealer;
        SortedDictionary<DateTime, SortedSet<Vehicle>> workOrders;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dealer">The dealer to bill</param>
        public Invoice(Dealer dealer)
        {
            this.dealer = dealer;
            workOrders = new SortedDictionary<DateTime, SortedSet<Vehicle>>();
        }

        /// <summary>
        /// Add a vehicle to a workorder
        /// </summary>
        /// <param name="d">The date of the workorder</param>
        /// <param name="v">The vehicle to add</param>
        /// <exception cref="ArgumentException">Vehicle already exists on specified workorder</exception>
        public void addVehicle(DateTime d, Vehicle v)
        {
            // If there is no workorder for the given date, create one
            if (!workOrders.ContainsKey(d))
            {
                workOrders.Add(d, new SortedSet<Vehicle>());
            }

            // Add the vehicle to the workorder
            // If the vehicle already exists on the workorder, throw exception
            if (workOrders[d].Add(v) == false)
            {
                throw new ArgumentException($"{dealer.Name}: {v} has already been billed on {d.Month}/{d.Day}/{d.Year}");
            }
        }
    }
}
