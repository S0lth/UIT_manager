using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation;
    /// <summary>
    ///     Represents a collection of IP addresses.
    /// </summary>
    public class IpsAddressesInformation : Information {
        private List<string> _ipsList = new();
        /// <summary>
        /// accessors of IpsAddresses List
        /// </summary>
        public List<string> IpsList {
            get => _ipsList;
            set => _ipsList = value;
        }

        /// <summary>
        ///     Returns a string representation of all IP addresses in the list.
        /// </summary>
        /// <returns>A string containing all IP addresses separated by comas.</returns>
        public override string ToString() {
            return $"{string.Join(", ", _ipsList)}";
        }
    }