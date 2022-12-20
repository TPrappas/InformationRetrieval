using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using CsvHelper;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using CsvHelper.Configuration;
using Elastic.Clients.Elasticsearch;
using System.Linq.Expressions;
using System.Collections;

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
        public static async Task<T> GetData<T>(ElasticClient client, string indexName, int id)
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
        public static async Task<IEnumerable<T>> SearchData<T, TValue>(ElasticClient client, string indexName, int from, int size, Expression<Func<T, TValue>> term, TValue value)
            where T : class
            where TValue : class
        {
            var response = await client.SearchAsync<T>(s => s
                .Index(indexName)
                .From(from)
                .Size(size)
                .Query(q => q
                    .Term(term, value)
                )
            );

            // If the response is not valid...
            if (!response.IsValid)
            {
                // Show the error


                // Return
                return null;
            }

            return response.Documents;
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
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        /// <param name="data"></param>
        public static async Task<IEnumerable<BulkResponseItemBase>> BulkData<T>(ElasticClient client, string indexName, IEnumerable<T> data)
            where T : class
        {
            var response = await client.BulkAsync(x => x.Index(indexName).CreateMany<T>(data));

            //// If the response is not valid...
            //if (!response.IsValid)
            //{
            //    // Show the error


            //    // Return
            //    return null;
            //}

            return response.Items;
        }
    }
}
