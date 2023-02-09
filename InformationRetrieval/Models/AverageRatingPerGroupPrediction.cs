namespace InformationRetrieval
{
    /// <summary>
    /// The average rating per group prediction
    /// </summary>
    public class AverageRatingPerGroupPrediction : AverageRatingPerGroup
    {
        #region Public Properties

        /// <summary>
        /// Predicted clustrer label from the trainer
        /// </summary>
        public uint PredictedLabel { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AverageRatingPerGroupPrediction() : base()
        {

        }

        #endregion
    }
}