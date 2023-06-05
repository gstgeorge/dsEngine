namespace dsEngine
{
    internal partial class Dealer
    {
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
