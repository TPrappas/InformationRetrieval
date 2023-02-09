using CsvHelper;

using Nest;

using System.Globalization;
using System.Linq.Expressions;

namespace InformationRetrieval
{
    public static class HelperMethods
    {
        /// <summary>
        /// Imports from a CSV file from the given <paramref name="filePath"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IEnumerable<T> ImportFromCSV<T>(string filePath)
            where T : class
        {
            var text = File.ReadAllText(filePath);
            text = text.Replace("\"['", "\"").Replace("']\"", "\"").Replace("['", "\"").Replace("']", "\"");
            File.WriteAllText(filePath, text);

            var streamReader = new StreamReader(filePath);
            
            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            var data = csvReader.GetRecords<T>().ToList();

            csvReader.Dispose();
            streamReader.Dispose();

            return data;
        }

        /// <summary>
        /// Creates a new index in the given <paramref name="client"/> with name the given <paramref name="name"/>
        /// </summary>
        /// <typeparam name="T">The type of the items in the index</typeparam>
        /// <param name="name">The name</param>
        /// <param name="client">The client</param>
        /// <returns></returns>
        public static async Task<bool> CreateIndexAsync<T>(string name, ElasticClient client)
            where T : class
        {
            var indexSettings = new IndexSettings()
            {
                NumberOfReplicas = 1,
                NumberOfShards = 1
            };
            var response = await client.Indices.CreateAsync(
                name,
                index => index.InitializeUsing(new IndexState()
                {
                    Settings = indexSettings
                }).Map<T>(p => p.AutoMap())
            );

            var updateResponse = await client.Indices.UpdateSettingsAsync(name, s => s
                .IndexSettings(i => i.Setting(UpdatableIndexSettings.MaxResultWindow, 1000000)));

            if (!response.IsValid)
                Console.WriteLine($"Unsuccessful creation of index '{name}'! The index probably already exists.");
            else
                Console.WriteLine($"Successful creation of index '{name}'!");

            return response.IsValid;
        }

        /// <summary>
        /// Index the data if the response is valid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="data"></param>
        public static async void IndexData<T>(ElasticClient client, string indexName, T data)
            where T : class
        {
            var response = await client.IndexAsync(data, request => request.Index(indexName));

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error


                // Return
                return;
            }

            Console.WriteLine($"Index document with ID {response.Id} succeeded.");
        }

        /// <summary>
        /// Get the data with a specific id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="id"></param>
        public static async Task<T?> GetDataAsync<T>(ElasticClient client, string indexName, int id)
            where T : class
        {
            var response = await client.GetAsync<T>(id, idx => idx.Index(indexName));

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error


                // Return
                return null;
            }

            return response.Source;
        }

        /// <summary>
        /// Search the data that contain similar words
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <param name=""></param>
        public static async Task<IEnumerable<T>> SearchDataAsync<T, TValue>(ElasticClient client, string indexName, int from, int size, Expression<Func<T, TValue>>? term = null, TValue? value = null)
            where T : class
            where TValue : class
        {
            var response = await client.SearchAsync<T>(s => s
                .Index(indexName)
                .From(from)
                .Size(size)
                .Query(q =>
                    q.Match(m =>
                        m.Field(term).Query(value?.ToString())
                    )
                )
            );

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error

                // Return
                return Enumerable.Empty<T>();
            }

            return response.Documents;
        }

        /// <summary>
        /// Search the data that contain similar words
        /// </summary>
        /// <typeparam name="TValue">The value</typeparam>
        /// <param name="client">The client</param>
        /// <param name="indexName">The index name</param>
        /// <param name="from">Starting index</param>
        /// <param name="size">The size</param>
        /// <param name="term">The term</param>
        /// <param name="userId">The user id</param>
        /// <param name="value">The value</param>
        public static async Task<IEnumerable<Book>> SearchUserBookRatingsAsync<TValue>(ElasticClient client, int userId, string indexName, int from, int size, Expression<Func<Book, TValue>> term, TValue value)
            where TValue : class
        {
            var response = await client.SearchAsync<Book>(s => s
                .Index(indexName)
                .From(from)
                .Size(size)
                .Query(q =>
                    q.Match(m =>
                        m.Field(term).Query(value.ToString())) &&
                    q.Term("ratings.userId", userId)
                )
            );
            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error
                Console.WriteLine(response.OriginalException.Message);
                // Return
                return Enumerable.Empty<Book>();
            }

            var books = response.Documents.OrderByDescending(x => x.Title).ToList();

            var percentage = (int)Math.Ceiling(books.Count() * 0.1);

            return books.GetRange(0, percentage);
        }

        /// <summary>
        /// Update the data with the specific id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="data"></param>
        /// <param name=""></param>
        public static async void UpdateData<T>(ElasticClient client, string indexName, DocumentPath<T> id, T data)
            where T : class
        {
            var response = await client.UpdateAsync<T>(id, u => u
               .Index(indexName)
               .Doc(data)
            );

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error


                // Return
                return;
            }

            Console.WriteLine("Update document succeeded.");
        }

        /// <summary>
        /// Delete the data with the specific id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="id"></param>
        public static async void DeleteData<T,TValue>(ElasticClient client, string indexName, Expression<Func<T, TValue>> term, TValue id)
            where T : class
        {
            var response = await client.DeleteByQueryAsync<T>(x => x.Index(indexName)
                                                                    .Query(y => y.Term(term, id)));

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error


                // Return
                return;
            }

            Console.WriteLine("Delete document succeeded.");
        }

        /// <summary>
        /// Bulk insert the given data
        /// </summary>
        /// <typeparam name="T">Τhe type</typeparam>
        /// <param name="client">The client</param>
        /// <param name="indexName">The index name</param>
        /// <param name="data">The data</param>
        public static async Task<IEnumerable<BulkResponseItemBase>> BulkDataAsync<T>(ElasticClient client, string indexName, List<T> data)
            where T : class
        {
            var startIndex = 0;
            var tasks = new List<Task<BulkResponse>>();
            
            // While the start index is not the last...
            while(startIndex <= data.Count - 1 - 300)
            {
                // Add to the task list a new bulk for the specidfied index for 300 records
                tasks.Add(client.BulkAsync(x => x.Index(indexName).CreateMany<T>(data.GetRange(startIndex, 300))));
                // Increament the index by 300
                startIndex += 300;
            }

            // Excecute the tasks
            var results = await Task.WhenAll(tasks);

            // Return the items
            return results.SelectMany(x => x.Items);
        }
    }
}
