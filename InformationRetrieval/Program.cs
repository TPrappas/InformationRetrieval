using System;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.IO;
using static InformationRetrieval.Constants;
using Nest;
using Elasticsearch.Net;
using Elastic.Clients.Elasticsearch;

namespace InformationRetrieval
{
    public class Tweet
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime PostDate { get; set; }
        public string Message { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IpAddress { get; set; }
    }
    public class GeoIp
    {
        public string CityName { get; set; }
        public string ContinentName { get; set; }
        public string CountryIsoCode { get; set; }
        public GeoLocation Location { get; set; }
        public string RegionName { get; set; }
    }

    public class Program
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(pool);

            var client = new ElasticClient(settings);

            AddDataToElasticSearch(client);

            // For debugging reasons
            Console.ReadLine();
        }

        /// <summary>
        /// Adds to the elastic search the book and user data from the CSV files
        /// </summary>
        public static async void AddDataToElasticSearch(ElasticClient client)
        {
            // Imports the books from the Book-CSV
            var books = HelperMethods.ImportFromCSV<Book>(BooksFilePath).ToList();

            // Imports the book ratings from the Book-Rating-CSV
            var ratings = HelperMethods.ImportFromCSV<BookRating>(BookRatingsFilePath);

            // Matches the book ratings to the book
            books.ForEach(book => book.Ratings = ratings.Where(x => x.BookId == book.Id));
            
            // Bulk inserts the books
            var booksBulkInsert = await HelperMethods.BulkData(client, "books", books);

            // Bulk inserts the book ratings
            var bookRatingsBulkInsert = await HelperMethods.BulkData(client, "ratings", ratings);

            // Imports the books from the Book-CSV
            var users = HelperMethods.ImportFromCSV<User>(UsersFilePath).ToList();

            // Bulk inserts the users
            var usersBulkInsert = await HelperMethods.BulkData(client, "users", books);
        }

        public static async void TestHelpers()
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(pool);
                // configure the client with authentication credentials
                //.BasicAuthentication("username", "12345678");

            var client = new ElasticClient(settings);

            //var booksIndexResult = await HelperMethods.CreateIndex<Book>("books", client);
            //var usersIndexResult = await HelperMethods.CreateIndex<User>("user", client);

            var person = new Person()
            {
                Id = 0,
                FirstName = "Blue",
                LastName = "Magenta",
                IpAddress = "127.0.0.1",
            };

            HelperMethods.IndexData(client, "people", person);

            var people = new List<Person>()
            {
                new Person()
                {
                    Id = 5,
                    FirstName = "AAAAAA",
                    LastName = "Papa",
                    IpAddress= "127.0.0.1",
                },
                new Person()
                {
                    Id = 1,
                    FirstName = "Triantafyllos",
                    LastName = "Prappas",
                    IpAddress= "127.0.0.1",
                },
                new Person()
                {
                    Id = 6,
                    FirstName = "BBBBB",
                    LastName = "Papa",
                    IpAddress= "127.0.0.1",
                }
            };

            var bulk = await HelperMethods.BulkData(client, "people", people);

            HelperMethods.UpdateData(client, "people", people.First(x => x.Id == 1).Id, new Person()
            {
                FirstName = "Orange",
                Id = 1
            });

            //var search = await HelperMethods.SearchData<Person, string>(client, "people", 0, 10, x => x.IpAddress, "127.0.0.1");

            //var get = await HelperMethods.GetData<Person>(client, "people", 4);

            //HelperMethods.DeleteData<Person, int>(client, "people", x => x.Id, 3);

            //var searchAll = await HelperMethods.SearchData<Person, string>(client, "people", 0, 10, x => x.IpAddress, "127.0.0.1");

        }

        public static async void  TestEight()
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(pool);
                // configure the client with authentication credentials
                //.BasicAuthentication("username", "12345678");


            var client = new ElasticClient(settings);
            var tweetsIndexResult = await HelperMethods.CreateIndex<User>("tweets", client);

            var tweet = new Tweet
            {
                Id = 1,
                User = "stevejgordon",
                PostDate = new DateTime(2009, 11, 15),
                Message = "Trying out the client, so far so good?"
            };

            var response = await client.IndexAsync(tweet, request => request.Index("tweets"));

            if (response.IsValid)
            {
                Console.WriteLine($"Index document with ID {response.Id} succeeded.");
            }

            var responseT = await client.GetAsync<Tweet>(1, idx => idx.Index("tweets"));
            var tweetT = responseT.Source;

            var getResponse = await client.SearchAsync<Tweet>(s => s
                .Index("tweets")
                .From(0)
                .Size(10)
                //.Query(q => q
                //    .Term(t => t.User, "stevejgordon")
                //)
            );

            if (getResponse.IsValid)
            {
                var getTweet = getResponse.Documents.FirstOrDefault();
            }

            var updateResponse = await client.UpdateAsync<Tweet>(tweet.Id, u => u
               .Index("tweets")
               .Doc(new Tweet { Message = "Updated title!" }));

            if (updateResponse.IsValid)
            {
                Console.WriteLine("Update document succeeded.");
            }

            var deleteResponse = await client.DeleteByQueryAsync<Tweet>(x => x.Index("tweets").Query(y => y.Term(z => z.Id, 1)));


            if (deleteResponse.IsValid)
            {
                Console.WriteLine("Delete document succeeded.");
            }

            var tweets = new List<Tweet>()
            {
                new Tweet()
                {
                    Id = 7,
                    User = "stevejgordon",
                    PostDate = new DateTime(2009, 11, 15),
                    Message = "Trying out the client, so far so good?"
                },
                new Tweet()
                {
                    Id = 8,
                    User = "stevejgordon",
                    PostDate = new DateTime(2009, 11, 15),
                    Message = "Trying out the client, so far so good?"
                },
                new Tweet()
                {
                    Id = 9,
                    User = "stevejgordon",
                    PostDate = new DateTime(2009, 11, 15),
                    Message = "Trying out the client, so far so good?"
                },
                new Tweet()
                {
                    Id = 10,
                    User = "papapapa",
                    PostDate = new DateTime(2009, 11, 15),
                    Message = "Trying out the client, so far so good?"
                },
            };

            var bulkInsertResponse = await client.BulkAsync(x => x.Index("tweets").CreateMany<Tweet>(tweets));

            if(bulkInsertResponse.IsValid)
            {
                Console.WriteLine("Bulk insert succeeded");
            }

        }
        
        #endregion

    }
}