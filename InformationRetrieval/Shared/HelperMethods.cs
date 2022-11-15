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
            text = text.Replace("['", "\"").Replace("']", "\"");
            File.WriteAllText(filePath, text);

            var streamReader = new StreamReader(filePath);
            
            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            var data = csvReader.GetRecords<T>().ToList();

            Console.WriteLine("Test 1 completed successfully");

            csvReader.Dispose();
            streamReader.Dispose();

            return data;
        }

    }
}
