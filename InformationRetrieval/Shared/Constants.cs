namespace InformationRetrieval
{
    public static class Constants
    {
        #region File path

        public const string FolderPath = @"D:/CEID/Ανάκτηση";

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
    }
}
