using CsvHelper.Configuration.Attributes;

using Nest;

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
        public Guid Id { get; }

        /// <summary>
        /// The rating
        /// </summary>
        [Name("rating")]
        public double Rating { get; set; }

        /// <summary>
        /// The user's id
        /// </summary>
        [Name("uid")]
        public int UserId { get; set; }

        /// <summary>
        /// The book's id
        /// </summary>
        [Name("isbn")]
        public string BookId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>
        public BookRating()
        {
            Id = Guid.NewGuid();
        }

        #endregion
    }
}
