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
            var books = HelperMethods.ImportFromCSV<Book>(BooksFilePath);
            var ratings = HelperMethods.ImportFromCSV<BookRating>(BookRatingsFilePath);
            // books
            var Books = new List<Book>();

            // foreach book
            // select all rating where book id = book.Id
            // book.ratings = select
            
        }
    }
}