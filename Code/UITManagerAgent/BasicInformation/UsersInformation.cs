namespace UITManagerAgent.BasicInformation;

/// <summary>
///     Represents a collection of user information.
///     Inherits from the <see cref="Information" /> class.
/// </summary>
public class UsersInformation : Information {
    /// <summary>
    /// Users main fields.
    /// </summary>
    public InnerValue Users { get; set; } = new("Users List", "null");

    /// <summary>
    /// Detailed Users information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new();

    /// <summary>
    /// Returns a Json string representation of the UserInformation.
    /// </summary>
    /// <returns>A Json string that represents the UserInformation.</returns>
    public override string ToJson() {
        //string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}""}}"));
        string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}"",""InformationAgents"":[{string.Join(",", agent.InformationAgents!.Select(innerAgent => $@"{{""Name"":""{innerAgent.Name}"",""Value"":""{innerAgent.Value}"",""Format"":""{innerAgent.Format}""}}"))}]}}"));

        return $@"{{""Name"": ""{Users.Name}"",""Value"": ""{Users.Value}"",""Format"": ""{Users.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
    public class User{
        
        /// <summary>
        /// accessors of the User field
        /// </summary>
        public InnerValue UserName { get; set; } = new("User Name","TEXT");
        public InnerValue UserScope { get; set; } = new("User Scope","TEXT");

        /// <summary>
        /// Return a List that represents information of a User.
        /// </summary>
        /// <returns>A List that represents information of a User.</returns>
        public List<InnerValue> GetList() {
            List<InnerValue> innerValues = new();
            innerValues.Add(UserName);
            innerValues.Add(UserScope);
            return innerValues;
        }
    }
}

