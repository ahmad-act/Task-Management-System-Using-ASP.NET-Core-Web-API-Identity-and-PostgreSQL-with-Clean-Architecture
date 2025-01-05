namespace TaskManagement.Domain.Common.HATEOAS
{
    public class Link
    {
        /// <summary>
        /// Gets or sets the HTTP method associated with the link (e.g., GET, POST, PUT, DELETE).
        /// </summary>
        public string Method { get; set; } // HTTP method

        /// <summary>
        /// Gets or sets the URL that the link points to, representing the resource's location.
        /// </summary>
        public string Href { get; set; }  // URL

        /// <summary>
        /// Gets or sets the relation type of the link, indicating the purpose of the link (e.g., Get, Create).
        /// </summary>
        public string Operation { get; set; }  // Relation type
    }
}