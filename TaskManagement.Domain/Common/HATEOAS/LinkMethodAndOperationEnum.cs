namespace TaskManagement.Domain.Common.HATEOAS
{
    public enum LinkMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// Defines the various operations that can be performed on a resource in a HATEOAS compliant API.
    /// This enumeration is used to specify the type of link operation associated with a resource.
    /// </summary>
    public enum LinkOperation
    {
        /// <summary>
        /// Represents the HTTP GET operation, typically used to retrieve all resources.
        /// </summary>
        List,


        /// <summary>
        /// Represents the HTTP GET operation, typically used to retrieve a resource.
        /// </summary>
        Get,

        /// <summary>
        /// Represents the HTTP POST operation, typically used to create a new resource.
        /// </summary>
        Create,

        /// <summary>
        /// Represents the HTTP PUT operation, typically used to update an existing resource.
        /// </summary>
        Update,

        /// <summary>
        /// Represents the HTTP DELETE operation, typically used to remove a resource.
        /// </summary>
        Delete,

        /// <summary>
        /// Represents the operation to retrieve the next page of resources in a paginated response.
        /// </summary>
        Next,

        /// <summary>
        /// Represents the operation to retrieve the previous page of resources in a paginated response.
        /// </summary>
        Previous,

        /// <summary>
        /// Represents the operation to retrieve the first page of resources in a paginated response.
        /// </summary>
        First,

        /// <summary>
        /// Represents the operation to retrieve the last page of resources in a paginated response.
        /// </summary>
        Last,

        /// <summary>
        /// Represents the operation to retrieve the other pages of resources in a paginated response.
        /// </summary>
        OtherPages
    }
}
