using System.Text.Json;
namespace UITManagerAgent.BasicInformation;

/// <summary>
///     Represents a collection of user information.
///     Inherits from the <see cref="Information" /> class.
/// </summary>
public class UsersInformation : Information {
    /// <summary>
    ///     List of collected usernames.
    /// </summary>
    private List<User> _usersList = new();

    public class User {
        private string? _name;
        private string? _formatName = "TEXT";
        private string? _scope;
        private string? _formatScope = "TEXT";
        
        public User() {
        }
        
        public User(string? name, string? scope) {
            _name = name;   
            _scope = scope;
        }

        /// <summary>
        /// accessors of th Name field
        /// </summary>
        public string? Name {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Gets or sets the format of the username information
        /// </summary>
        /// <value>
        /// A string representing the format of username.
        /// </value>
        public string? FormatName {
            get => _formatName;
            set => _formatName = value;
        }

        /// <summary>
        /// accessors of th Scope field
        /// </summary>
        public string? Scope {
            get => _scope;
            set => _scope = value;
        }
        
        
        /// <summary>
        /// Gets or sets the format of the user scope information
        /// </summary>
        /// <value>
        /// A string representing the format of user scope.
        /// </value>
        public string? FormatScope {
            get => _formatScope;
            set => _formatScope = value;
        }
        
        /// <summary>
        /// Returns a Json string representation of the user information
        /// </summary>
        /// <returns>A Json string that represents the user information .</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this);
        }
    }
    
    /// <summary>
    /// accessors of th usersList field
    /// </summary>
    public List<User> UsersList {
        get => _usersList;
        set => _usersList = value;
    }

    /// <summary>
    ///     Returns a string representation of the list of usernames.
    /// </summary>
    /// <returns>
    ///     A comma-separated string of usernames.
    /// </returns>
    public override string ToString() {
        return $"{string.Join(", ", UsersList)}";
    }

    /// <summary>
    /// Returns a Json string representation of the users
    /// </summary>
    /// <returns>A Json string that represents the users.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}