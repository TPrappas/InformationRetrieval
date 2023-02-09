namespace InformationRetrieval
{
    public static class Constants
    {
        #region File Paths

        /// <summary>
        /// The folder path
        /// </summary>
        public const string FolderPath = @"D:\CEID\Ανάκτηση";

        /// <summary>
        /// The books' CSV file path
        /// </summary>
        public const string BooksFilePath = $@"{FolderPath}/BX-Books.csv";
        
        /// <summary>
        /// The book ratings' CSV file path
        /// </summary>
        public const string BookRatingsFilePath = $@"{FolderPath}/BX-Book-Ratings.csv";

        /// <summary>
        /// The users' CSV file path
        /// </summary>
        public const string UsersFilePath = $@"{FolderPath}/BX-Users.csv";

        #endregion

        #region Indices

        /// <summary>
        /// The books index
        /// </summary>
        public const string BooksIndex = "books";

        /// <summary>
        /// The book ratings index
        /// </summary>
        public const string RatingsIndex = "ratings";

        /// <summary>
        /// The users index
        /// </summary>
        public const string UsersIndex = "users";


        #endregion
    }
}
