using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
            Save();
        }


        /// <summary>
        /// The dealer's business name.
        /// </summary>
        public string Name 
        {
            get => _name;
            set
            {
                if (value == null || value == String.Empty)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                else
                {
                    _name = value;
                }
            }
        }

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
        /// Write the dealer settings to disk at <see cref="Settings.DEALER_SETTINGS_PATH"/>.
        /// </summary>
        public void Save()
        {
            File.WriteAllText(Settings.DEALER_SETTINGS_PATH + Name + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
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
