using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace dsEngine
{
    /// <summary>
    /// Stores dealer and monthly charges for invoicing.
    /// </summary>
    internal class Dealer : IComparable<Dealer>
    {
        // Private fields.
        private string _name;


        /// <summary>
        /// Initializes a new instance of the Dealer class.
        /// </summary>
        /// <param name="name">The dealer's name.</param>
        public Dealer(string name)
        {
            Name = name;
            Charges = new List<Charge>();
            DealerDirectory.Add(this);
            Save();
        }

        /// <summary>
        /// A collection of dealers which have previously been configured.
        /// </summary>
        public static SortedSet<Dealer> DealerDirectory = new SortedSet<Dealer>();

        /// <summary>
        /// The dealer's business name.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                // If a name has not been entered, throw an exception.
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Name");
                }

                // Remove any excess whitespace
                string cleanValue = Regex.Replace(value, @"\W+", " ").Trim();

                // Check the dealer directory to ensure a dealer by the same name does not already exist.
                var hasDuplicate = DealerDirectory.FirstOrDefault(dealer => dealer.Name == cleanValue);

                // If a duplicate dealer was found, throw an exception.
                if (hasDuplicate != null && hasDuplicate != this)
                {
                    throw new ArgumentException($"A dealer named {cleanValue} already exists.");
                }

                // If this dealer is being renamed, remove the old config file.
                if (!string.IsNullOrEmpty(_name))
                {
                    string delPath = Settings.DEALER_SETTINGS_PATH + FileName + ".json";

                    if (File.Exists(delPath))
                    {
                        File.Delete(delPath);
                    }
                }

                // Assign the new name, and save the dealer config to disk.
                _name = cleanValue;
                Save();
            }
        }

        /// <summary>
        /// Vehicles which have been processed for this dealer.
        /// </summary>
        [JsonIgnore]
        public SortedDictionary<DateTime, List<Vehicle>> WorkOrders { get; private set; }

        /// <summary>
        /// List of monthly charges that should be billed to the dealer.
        /// </summary>
        public List<Charge> Charges { get; set; }

        /// <summary>
        /// Flag to signal whether an invoice should be generated for this dealer.
        /// </summary>
        [JsonIgnore]
        public bool Process { get; set; } = false;

        /// <summary>
        /// Return the dealer's name without any whitespace characters for use as a filename.
        /// </summary>
        private string FileName { get => _name.Replace(' ', '_').ToLower(); }


        /// <summary>
        /// Load saved dealer configs from disk and add them to the dealer directory.
        /// </summary>
        public static void LoadDealers()
        {
            if (Directory.Exists(Settings.DEALER_SETTINGS_PATH))
            {
                foreach (string f in Directory.EnumerateFiles(Settings.DEALER_SETTINGS_PATH))
                {
                    DealerDirectory.Add(JsonConvert.DeserializeObject<Dealer>(File.ReadAllText(f)));
                }
            }
        }

        /// <summary>
        /// Write the dealer settings to disk at <see cref="Settings.DEALER_SETTINGS_PATH"/>.
        /// </summary>
        public void Save()
        {
            File.WriteAllText(Settings.DEALER_SETTINGS_PATH + FileName + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// Add a vehicle to a specified workorder.
        /// </summary>
        /// <param name="d">The date of the workorder.</param>
        /// <param name="v">The vehicle to add.</param>
        /// <exception cref="ArgumentException">Vehicle already exists on specified workorder.</exception>
        public void AddVehicleToWorkOrder(DateTime d, Vehicle v)
        {
            // If there is no work order for the given date, create one.
            if (!WorkOrders.ContainsKey(d))
            {
                WorkOrders.Add(d, new List<Vehicle>());
            }

            // Add the vehicle to the workorder
            // If the vehicle already exists on the workorder, throw exception
            if (WorkOrders[d].Contains(v))
            {
                throw new ArgumentException($"{Name}: {v} has already been billed on {d.Month}/{d.Day}/{d.Year}");
            }
            else WorkOrders[d].Add(v);
        }

        // Implement IComparable interface
        public int CompareTo(Dealer other)
        {
            return Name.CompareTo(other.Name);
        }
            

        /// <summary>
        /// Stores info on monthly charges.
        /// </summary>
        internal class Charge
        {
            /// <summary>
            /// Type of monthly charge.
            /// </summary>
            public enum ChargeType 
            { 
                /// <summary>
                /// Charge should appear as-is. This is a fixed monthly charge.
                /// </summary>
                FIXED, 

                /// <summary>
                /// Charge should be multiplied by the number of Used vehicles processed.
                /// </summary>
                USED, 

                /// <summary>
                /// Charge should be multiplied by the number of New vehicles processed.
                /// </summary>
                NEW, 

                /// <summary>
                /// Charge should be multiplied by the total number of vehicles processed.
                /// </summary>
                VEHICLE 
            }

            /// <summary>
            /// The charge's line item description.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The type of charge.
            /// </summary>
            public ChargeType Type { get; set; }

            /// <summary>
            /// The charge amount.
            /// </summary>
            public double Amount { get; set; }

            /// <summary>
            /// Should the charge be included when an invoice for this dealer is generated?
            /// </summary>
            public bool Enabled { get; set; }
        }
    }
}
