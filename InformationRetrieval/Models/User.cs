using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public int Id { get; set; }

        /// <summary>
        /// The location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The age
        /// </summary>
        public float? Age { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>

        public User()
        {

        }

        #endregion
    }
}

