namespace InformationRetrieval
{
    /// <summary>
    /// A user's average rating per country andd age group
    /// </summary>
    public class AverageRatingPerGroup
    {
        #region Public Properties

        /// <summary>
        /// The GenZ rating
        /// </summary>
        public double GenZRating { get; set; }

        /// <summary>
        /// The Millenial rating
        /// </summary>
        public double MillenialRating { get; set; }

        /// <summary>
        /// The GenX rating
        /// </summary>
        public double GenXRating { get; set; }

        /// <summary>
        /// The boomer rating
        /// </summary>
        public double BoomerRating { get; set; }

        /// <summary>
        /// The bot rating
        /// </summary>
        public double BotRating { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AverageRatingPerGroup() : base()
        {

        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AverageRatingPerGroup(List<double> values) : base()
        {
            _ = values ?? throw new ArgumentNullException();

            if (values.Count != 5)
                throw new ArgumentOutOfRangeException();

            GenZRating = values[0];
            MillenialRating = values[1];
            GenXRating = values[2];
            BoomerRating = values[3];
            BotRating = values[4];
        }

        #endregion
    }
}