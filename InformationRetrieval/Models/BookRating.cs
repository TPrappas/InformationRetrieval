using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using System;

#nullable disable

namespace InformationRetrieval
{
    /// <summary>
    /// The class describes a row of the book's ratings CSV
    /// <summary>
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class BookRating
    {
        #region Public Properties

        /// <summary>
        /// The unique id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The rating
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// The user's id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The book's id
        /// </summary>
        public string BookId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>

        public BookRating()
        {

        }

        #endregion
    }
}
