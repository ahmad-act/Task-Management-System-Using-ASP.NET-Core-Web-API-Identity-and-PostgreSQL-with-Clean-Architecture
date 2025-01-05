using TaskManagement.Domain.Entities.Base.Basic;

namespace TaskManagement.Domain.Entities
{
    /// <summary>
    /// Represents a physical or mailing address associated with a user in the system.
    /// </summary>
    /// <remarks>
    /// The <c>Address</c> class contains detailed information about a user's address, 
    /// including the type of address, street, city, state, country, and postal code.
    /// </remarks>
    public class Address : BaseBasicEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user associated with this address.
        /// </summary>
        /// <remarks>
        /// This property links the address to a specific user, ensuring that each address 
        /// can be associated with a particular user in the system.
        /// </remarks>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of address.
        /// </summary>
        /// <remarks>
        /// This property defines the nature of the address, such as "Home," "Office," or "Billing".
        /// It helps in categorizing the address based on its intended use.
        /// </remarks>
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the street information for the address.
        /// </summary>
        /// <remarks>
        /// This property typically contains details like the street name, number, and any 
        /// additional location details (e.g., apartment number or suite).
        /// </remarks>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the city where the address is located.
        /// </summary>
        /// <remarks>
        /// Provides the city name for the address, allowing for geographical identification 
        /// within the country.
        /// </remarks>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state or province where the address is located.
        /// </summary>
        /// <remarks>
        /// This property defines the state or administrative region, offering another level 
        /// of location detail.
        /// </remarks>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country of the address.
        /// </summary>
        /// <remarks>
        /// Specifies the country where the address is located, which is essential for 
        /// identifying international addresses.
        /// </remarks>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal or ZIP code for the address.
        /// </summary>
        /// <remarks>
        /// This property allows for postal code identification, which aids in precise 
        /// geographical and mail delivery purposes.
        /// </remarks>
        public string PostalCode { get; set; }
    }
}
