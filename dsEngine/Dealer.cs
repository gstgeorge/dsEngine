using GenericParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dsEngine
{
    /// <summary>
    /// Stores dealer and monthly charges for invoicing.
    /// </summary>
    internal class Dealer : IComparable<Dealer>
    {
        // Private fields.
        private string _name;

        #region Properties

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
        public SortedDictionary<DateTime, List<Vehicle>> WorkOrders { get; private set; } = new SortedDictionary<DateTime, List<Vehicle>>();

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
        public string FileName { get => _name.Replace(' ', '_').ToLower(); }

        #endregion

        #region Methods

        /// <summary>
        /// Parse one or more files, adding each vehicle to the dealer's work orders
        /// </summary>
        /// <param name="paths">The paths of the files to process</param>
        public static void ImportVehiclesFromFile(params string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    // Process the file based on the filetype
                    switch (Path.GetExtension(path).ToLower())
                    {
                        case ".csv":
                            ParseCSV(path);
                            MessageBox.Show($"Processed {path}");
                            break;
                        
                        // TODO: add xlsx support

                        default: // File is not of a supported format.
                            throw new InvalidDataException("File is an invalid type.");
                    }
                }

                // File is either an invalid filetype, or unable to be recognized as belonging to a supported data source.
                catch (InvalidDataException ide)
                {
                    string errorMsg = "Unable to import from " + Path.GetFileName(path) + "." + Environment.NewLine +
                        ide.Message + Environment.NewLine + Environment.NewLine + "Continue processing any remaining files?";

                    if (MessageBox.Show(errorMsg, "ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        continue;
                    }
                    else return;
                }

                // Dealer does not exist in the dealer directory.
                // TODO: Prompt user to add dealer to the dealer directory and attempt to re-process this file
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Dealer not found. Skipping.");
                    continue;
                }

                // Vehicle already exists on this work order.
                // TODO: Do something with this.
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }

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
        private void Save()
        {
            File.WriteAllText(Settings.DEALER_SETTINGS_PATH + FileName + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }


        /// <summary>
        /// Add a vehicle to the work order of a specified dealer.
        /// </summary>
        /// <param name="dealerName">The dealer's name.</param>
        /// <param name="date">The date of the workorder.</param>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <exception cref="InvalidOperationException">Dealer does not exist in the dealer directory.</exception>
        /// <exception cref="ArgumentException">Vehicle already exists on specified workorder.</exception>
        private static void AddVehicleToWorkOrder(string dealerName, DateTime date, Vehicle vehicle)
        {
            // Lookup dealer in dealer directory
            var dealer = DealerDirectory.FirstOrDefault(x => x.Name == dealerName);

            // If dealer does not exist in the dealer directory, throw an exception.
            if (dealer == null)
            {
                throw new InvalidOperationException("Dealer configuration not found.");
            }

            // If there is no work order for the given date, create one.
            if (!dealer.WorkOrders.ContainsKey(date))
            {
                dealer.WorkOrders.Add(date, new List<Vehicle>());
            }

            // Add the vehicle to the workorder
            // If the vehicle already exists on the workorder, throw exception
            if (dealer.WorkOrders[date].Contains(vehicle))
            {
                throw new ArgumentException($"{dealer.Name}: {vehicle} has already been billed on {date.Month}/{date.Day}/{date.Year}");
            }
            else dealer.WorkOrders[date].Add(vehicle);
        }

        // Implement IComparable interface
        public int CompareTo(Dealer other)
        {
            return Name.CompareTo(other.Name);
        }

        /// <summary>
        /// Process a CSV file of vehicle data, adding each vehicle to the dealer's work orders.
        /// </summary>
        /// <param name="path">The path of the file to process.</param>
        /// <exception cref="ArgumentException">Vehicle already exists on specified workorder.</exception>
        /// <exception cref="InvalidDataException">The file is not formatted in a recognizable manner.</exception>
        /// <exception cref="InvalidOperationException">Dealer does not exist in the dealer directory.</exception>
        private static void ParseCSV(string path)
        {
            DataTable dt = new DataTable();

            // Parse file as DataTable
            using (GenericParserAdapter parser = new GenericParserAdapter(path)) // TODO: handle file in use SYSTEM.IO.Exception
            {
                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                parser.SkipStartingDataRows = 0;
                parser.MaxBufferSize = 4096;
                parser.MaxRows = 1000;
                dt = parser.GetDataTable();
            }

            // Identify the provider based on the number of columns, and set the appropiate column names.
            string vin, stock, condition, year, make, model, date;

            switch (dt.Columns.Count)
            {
                case 21: // Autouplink Tech

                    vin = "VIN";
                    stock = "Stock Number";
                    condition = "Vehicle Stock Type";
                    year = "Model Year";
                    make = "Make";
                    model = "Model";
                    date = "Service Date/Time";
                    break;

                default:
                    throw new InvalidDataException("Data source is unsupported or cannot be determined.");
            }

            // Process each vehicle
            foreach (DataRow row in dt.Rows)
            {
                Vehicle v = new Vehicle(
                    row[vin].ToString(),
                    row[stock].ToString(),
                    (Vehicle.Condition)Enum.Parse(typeof(Vehicle.Condition), row[condition].ToString()),
                    Convert.ToUInt16(row[year]),
                    row[make].ToString(),
                    row[model].ToString()
                    );

                DateTime d = DateTime.Parse(row[date].ToString());

                try
                {
                    AddVehicleToWorkOrder(row["Dealer Name"].ToString(), new DateTime(d.Year, d.Month, d.Day), v);
                }
                catch (InvalidOperationException ioex)
                {
                    throw ioex;
                }
                catch (ArgumentException aex)
                {
                    throw aex;
                }
            }
        }

        #endregion

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
