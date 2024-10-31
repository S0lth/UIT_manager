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
public class DomainNameInformation : Information
{
    private string _domainName;

    /// <summary>
    /// Retrieves the domain name.
    /// </summary>
    /// <returns>A string containing the domain name.</returns>
    public string GetDomainName()
    {
        return _domainName;
    }

    /// <summary>
    /// Sets the domain name to a specified value.
    /// </summary>
    /// <param name="domainName">A string containing the domain name to set.</param>
    public void SetDomainName(string domainName)
    {
        _domainName = domainName;
    }

    /// <summary>
    /// Returns a string representation of the domain name.
    /// </summary>
    /// <returns>A string that represents the domain name, prefixed with "Domain name : ".</returns>
    public override string ToString()
    {
        return "Domain name : " + _domainName;
    }

}
