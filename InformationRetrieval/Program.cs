using System;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.IO;

using static InformationRetrieval.Constants;

namespace InformationRetrieval
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var books = HelperMethods.ImportFromCSV<Book>(BooksFilePath).ToList();
            var ratings = HelperMethods.ImportFromCSV<BookRating>(BookRatingsFilePath);

            books.ForEach(book => book.Ratings = ratings.Where(x => x.BookId == book.Id));

            


            Console.ReadLine();
        }
    }
}