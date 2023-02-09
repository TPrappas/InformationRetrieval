using CsvHelper.Configuration.Attributes;

using Nest;

using System.Diagnostics.Metrics;

#nullable disable

namespace InformationRetrieval
{
    /// <summary>
    /// The class describes a row of the book's ratings CSV
    /// <summary>
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class BookRating
    {
        #region Private Members

        private string mBookId;

        #endregion

        #region Public Properties

        /// <summary>
        /// The unique id
        /// </summary>
        public string Id { get; }

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
        public string BookId
        {
            get => mBookId;

            set => mBookId = value?.Trim();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>
        public BookRating()
        {
            Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"B:{BookId}   U:{UserId}   R:{Rating}";
        }

        #endregion
    }
}
