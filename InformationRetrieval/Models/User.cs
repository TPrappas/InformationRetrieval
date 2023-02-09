using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Nest;

#nullable disable

namespace InformationRetrieval
{
    /// <summary>
    /// The class describes a row of the user's CSV
    /// <summary>
    
    public class User
    {
        #region Public Properties

        /// <summary>
        /// The id
        /// </summary>
        [Name("uid")]
        public int Id { get; set; }

        /// <summary>
        /// The location
        /// </summary>
        [Name("location")]
        public string Location { get; set; }

        /// <summary>
        /// The age
        /// </summary>
        [Name("age")]
        public float? Age { get; set; }

        /// <summary>
        /// The city
        /// </summary>
        public string City
        {
            get
            {
                var city = Location.Split(',')[0];
                return city.Trim() ?? string.Empty;
            }
        }

        /// <summary>
        /// The region
        /// </summary>
        public string Region
        {
            get
            {
                var region = Location.Split(',')[1];
                return region.Trim() ?? string.Empty;
            }
        }

        /// <summary>
        /// The country
        /// </summary>
        public string Country
        {
            get
            {
                var country = Location.Split(',')[2];
                return country.Trim() ?? string.Empty;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>

        public User()
        {

        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Age + " " + Country;
        }

        #endregion
    }

    public class UserRatings : User
    {
        #region Public Properties

        /// <summary>
        /// The book ratings of the user
        /// </summary>
        public IEnumerable<BookRating> Ratings { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserRatings() : base()
        {

        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserRatings(User user, IEnumerable<BookRating> ratings) : base()
        {
            Id = user.Id;
            Location = user.Location;   
            Age = user.Age;

            Ratings = ratings;
        }

        #endregion
    }
}

