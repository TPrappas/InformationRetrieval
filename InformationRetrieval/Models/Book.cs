using CsvHelper.Configuration.Attributes;

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
        [Name("isbn")]
        public string Id { get; set; }

        /// <summary>
        /// The title
        /// </summary>
        [Text(Similarity = "BM25")]
        [Name("book_title")]
        public string Title { get; set; }

        /// <summary>
        /// The author
        /// </summary>
        [Name("book_author")]
        public string Author { get; set; }

        /// <summary>
        /// The year of publication
        /// </summary>
        [Name("year_of_publication")]
        public int Year { get; set; }

        /// <summary>
        /// The publisher
        /// </summary>
        [Name("publisher")]
        public string Publisher { get; set; }

        /// <summary>
        /// The summary
        /// </summary>
        [Name("summary")]
        public string Summary { get; set; }
        
        /// <summary>
        /// The category
        /// </summary>
        [Name("category")]
        public string Category { get; set; }

        /// <summary>
        /// The categories
        /// </summary>
        public IEnumerable<string> Categories 
        {
            get => Category.Split(',');
        }

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
