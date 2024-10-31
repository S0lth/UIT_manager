using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UITManagerAgent.DataCollectors;


namespace UITManagerAgent.BasicInformation;

public class DomainNameInformation : Information
{
    private string _domainName;


    public string GetDomainName()
    {
        return _domainName;
    }

    public void SetDomainName(string domainName)
    {
        _domainName = domainName;
    }

    public override string ToString()
    {
        return "Nom de domaine : " + _domainName;
    }

}
