using System;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.IO;
using static InformationRetrieval.Constants;
using Nest;
using Elasticsearch.Net;
using Elastic.Clients.Elasticsearch;
using System.Diagnostics;
using Microsoft.ML;
using Elastic.Clients.Elasticsearch.Core.GetScriptContext;
using InformationRetrieval.Models;

namespace InformationRetrieval
{
    public class Program
    {
        #region Public Properties

        /// <summary>
        /// The total number of books in the CSV file
        /// </summary>
        public static int TotalBooksInFile { get; set; } = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Program() : base()
        {

        }

        #endregion

        #region Public Methods

        public static void Main(string[] args)
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(pool); 

            var client = new ElasticClient(settings);
            
            // Adds data to elastic search
            AddDataToElasticSearch(client);
            
            // Searches the book titles that contain the specified string and that the user with the specified id has rated them
            SearchBooks(client, 8067, "Of");

            GetUserData();

            // For debugging reasons
            Console.ReadLine();
        }

        /// <summary>
        /// Adds to the elastic search the book and user data from the CSV files
        /// </summary>
        public static async void AddDataToElasticSearch(ElasticClient client)
        {
            // Creates the books index
            var booksIndexResult = await HelperMethods.CreateIndexAsync<Book>(BooksIndex, client);

            // Creates the ratings index (they don't insert but we don't need them)
            var ratingsIndexResult = await HelperMethods.CreateIndexAsync<BookRating>(RatingsIndex, client);

            // Creates the users index
            var usersIndexResult = await HelperMethods.CreateIndexAsync<User>(UsersIndex, client);

            var books = HelperMethods.ImportFromCSV<Book>(BooksFilePath).ToList();

            // Imports the book ratings from the BX-Book-Rating-CSV
            var ratings = HelperMethods.ImportFromCSV<BookRating>(BookRatingsFilePath);

            // Imports the books from the BX-Users-CSV
            var users = HelperMethods.ImportFromCSV<User>(UsersFilePath).ToList();

            books = books.GroupJoin(ratings, x => x.Id, x => x.BookId, (book, ratings) =>
            {
                book.Ratings = ratings.ToList();

                return book;
            }).ToList();
            TotalBooksInFile = books.Count;

            var results = await Task.WhenAll(
                HelperMethods.BulkDataAsync(client, BooksIndex, books.GetRange(0, (int)Math.Floor((double)(books.Count / 3)) - 1)),
                HelperMethods.BulkDataAsync(client, BooksIndex, books.GetRange((int)Math.Floor((double)(books.Count / 3)), (int)Math.Floor((double)(books.Count * 2 / 3)) - 1)),
                HelperMethods.BulkDataAsync(client, BooksIndex, books.GetRange((int)Math.Floor((double)(books.Count * 2 / 3)), books.Count - 1))
            );

            // Bulk insert the ratings (they don't insert but we don't need them)
            var ratingsBulkInsert = await HelperMethods.BulkDataAsync(client, RatingsIndex, ratings.ToList());

            // Bulk inserts the users
            var usersBulkInsert = HelperMethods.BulkDataAsync(client, UsersIndex, users);
        }

        /// <summary>
        /// Searches the book titles that contain the specified string and that the user with the specified id has rated them
        /// </summary>
        /// <param name="client">The client</param>
        /// <param name="userId">The user id</param>
        /// <param name="searchTerm">The search term</param>
        public static async void SearchBooks(ElasticClient client, int userId, string searchTerm)
        { 
            // The books
            var result = await HelperMethods.SearchUserBookRatingsAsync(client, userId, BooksIndex, 0, 10000, x => x.Title, searchTerm);
        }
        
        /// <summary>
        /// Gets the user data and trains with K-means
        /// </summary>
        public static void GetUserData()
        {
            // Imports the book ratings from the BX-Book-Rating-CSV
            var ratings = HelperMethods.ImportFromCSV<BookRating>(BookRatingsFilePath);

            // Imports the books from the BX-Users-CSV
            var users = HelperMethods.ImportFromCSV<User>(UsersFilePath).ToList();


            // Declare a dictionary that will contain the user id and ratings per category
            var userStats = new List<UserRatingPerGroup>();

            var userRatings = users.GroupJoin(ratings, x => x.Id, y => y.UserId, (user, ratings) => 
                new UserRatings(user, ratings.Where(x => x.UserId == user.Id && x.Rating > 0).ToList()))
                .OrderBy(x => x.Id).ToList();

            var usersByCountry = userRatings.GroupBy(x => x.Country).OrderBy(g => g.Key).ToList();

            // For each group with users by country...
            foreach (var group in usersByCountry)
            {
                var genZ = group.Where(x => x.Age >= 0 && x.Age < 27 && x.Ratings.Count() > 0).ToList();
                var millenials = group.Where(x => x.Age >= 27 && x.Age < 43 && x.Ratings.Count() > 0).ToList();
                var genX = group.Where(x => x.Age >= 43 && x.Age < 59 && x.Ratings.Count() > 0).ToList();
                var boomers = group.Where(x => x.Age >= 59 && x.Ratings.Count() > 0).ToList();
                var bots = group.Where(x => x.Age is null && x.Ratings.Count() > 0).ToList();

                // Gets the ratings of the generation and calculates for each the average
                var genZAverageRating = genZ.Select(x => x.Ratings.Average(r => r.Rating));
                
                // For each user creates and adds a new model that has the user id and the average rating of their generation
                genZ.ForEach(x => userStats.Add(new UserRatingPerGroup() 
                {
                    RatingPerGroup = new AverageRatingPerGroup(new List<double>() { genZAverageRating.Average(x => x), 0, 0, 0, 0 }),
                    Id = x.Id
                }));
                
                // Gets the ratings of the generation and calculates for each the average
                var millenialsAverageRating = millenials.Select(x => x.Ratings.Average(r => r.Rating));
                
                // For each user creates and adds a new model that has the user id and the average rating of their generation
                millenials.ForEach(x => userStats.Add(new UserRatingPerGroup()
                {
                    RatingPerGroup = new AverageRatingPerGroup(new List<double>() { 0, millenialsAverageRating.Average(x => x), 0, 0, 0 }),
                    Id = x.Id
                }));

                // Gets the ratings of the generation and calculates for each the average
                var genXAverageRating = genX.Select(x => x.Ratings.Average(r => r.Rating));
                
                // For each user creates and adds a new model that has the user id and the average rating of their generation
                genX.ForEach(x => userStats.Add(new UserRatingPerGroup()
                {
                    RatingPerGroup = new AverageRatingPerGroup(new List<double>() { 0, 0, genXAverageRating.Average(x => x), 0, 0 }),
                    Id = x.Id
                }));

                // Gets the ratings of the generation and calculates for each the average
                var boomersAverageRating = boomers.Select(x => x.Ratings.Average(r => r.Rating));
                
                // For each user creates and adds a new model that has the user id and the average rating of their generation
                boomers.ForEach(x => userStats.Add(new UserRatingPerGroup()
                {
                    RatingPerGroup = new AverageRatingPerGroup(new List<double>() { 0, 0, 0, boomersAverageRating.Average(x => x), 0 }),
                    Id = x.Id
                }));

                // Gets the ratings of the generation and calculates for each the average
                var botsAverageRating = bots.Select(x => x.Ratings.Average(r => r.Rating));

                // For each user creates and adds a new model that has the user id and the average rating of their generation
                bots.ForEach(x => userStats.Add(new UserRatingPerGroup()
                {
                    RatingPerGroup = new AverageRatingPerGroup(new List<double>() { 0, 0, 0, 0, botsAverageRating.Average(x => x)}),
                    Id = x.Id
                }));
            }

            // Declare the features column names
            var featureColumnName = "Features";

            //Initialize a new machine learning context object and set the seed
            var machineLearningContext = new MLContext(0);

            // Initialize the number of clusters
            var numberOfClusters = 4;

            // Get the training data
            var averageRatingsTrainingData = machineLearningContext.Data.LoadFromEnumerable(userStats.Select(x => x.RatingPerGroup).ToList());

            // Get the column names
            var propertyNames = typeof(AverageRatingPerGroup).GetProperties().Select(x => x.Name).ToArray();

            // Initialize the k-means trainer
            var kMeansTrainer = machineLearningContext.Transforms.Concatenate(featureColumnName, propertyNames)
                .Append(machineLearningContext.Clustering.Trainers.KMeans(featureColumnName, numberOfClusters: numberOfClusters));

            // Train the model
            var trainedAverageRatingsModel = kMeansTrainer.Fit(averageRatingsTrainingData);

            // Run the model on the same data set
            var transformedAverageRatingsData = trainedAverageRatingsModel.Transform(averageRatingsTrainingData);

            // Get the predictions
            var predictions = machineLearningContext.Data.CreateEnumerable<AverageRatingPerGroupPrediction>(transformedAverageRatingsData, false).ToList();

            // Declare a dictionary for the cluster and the average ratings per category
            var clusteredUsersPerCluster = new List<List<int>>();

            // Declare a dictionary for the cluster and the average ratings per movie
            var averageMovieRatingPerCluster = new Dictionary<uint, List<double>>();
        }

        #endregion
    }
}