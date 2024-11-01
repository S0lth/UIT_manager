using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UITManagerAgent.DataCollectors;


namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about a domain name.
/// </summary>
public class DomainNameInformation : Information {
    private string _domainName = "";

    /// <summary>
    /// accessors of the domainName field
    /// </summary>
    public string DomainName {
        get { return _domainName; }
        set { _domainName = value ; }
    }

    /// <summary>
    /// Returns a string representation of the domain name.
    /// </summary>
    /// <returns>A string that represents the domain name, prefixed with "Domain name : ".</returns>
    public override string ToString() {
        return "Domain name : " + _domainName;
    }

}
