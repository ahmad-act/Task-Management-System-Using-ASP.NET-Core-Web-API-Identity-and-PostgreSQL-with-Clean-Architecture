namespace TaskManagement.Domain.Common.JWT
{
    /// <summary>
    /// Represents a JWT (JSON Web Token) payload that contains user identification information.
    /// This class holds the claims associated with the token, such as the user's ID and role.
    /// </summary>
    public class JwtData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// This property holds the user's ID, which is used to identify the user associated with the token.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// This property indicates the user's role, which may be used for authorization purposes.
        /// </summary>
        public string role { get; set; }
    }
}
