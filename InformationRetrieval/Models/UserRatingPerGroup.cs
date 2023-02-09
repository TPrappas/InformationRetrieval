
namespace InformationRetrieval
{
    /// <summary>
    /// The book ratings per user
    /// </summary>
    public class UserRatingPerGroup
    {
        #region Public Properties

        /// <summary>
        /// The user id
        /// </summary>
        public int Id { get; set; }

        public AverageRatingPerGroup? RatingPerGroup { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserRatingPerGroup() : base()
        {

        }

        #endregion
    }
}