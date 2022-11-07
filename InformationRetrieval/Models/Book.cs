using Nest;

#nullable disable

namespace InformationRetrieval
{
    /// <summary>
    /// The class describes a row of the book's CSV
    /// </summary>
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class Book
    {
        #region Public Properties

        /// <summary>
        /// The id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The title
        /// </summary>
        [Text(Similarity = "BM25")]
        public string Title { get; set; }

        /// <summary>
        /// The author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The year of publication
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The publisher
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// The summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The categories
        /// </summary>
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// The ratings
        /// </summary>
        public IEnumerable<BookRating> Ratings { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor 
        /// </summary>

        public Book()
        {

        }

        #endregion
    }
}
