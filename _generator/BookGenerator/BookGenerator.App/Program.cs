using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookGenerator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            Task.Run(async () =>
            {
                var bookGenerator = new BookGenerator();
                await bookGenerator.GenerateAllBook();
            }).Wait();
        }
    }

    class BookGenerator
    {
        public async Task GenerateAllBook()
        {
            var booksFolder = new[] { "Up & Going", "Scope & Closures", "this & Object Prototypes", "Types & Grammar", "Async & Performance", "ES6 & Beyond" };
            foreach (var bookFolder in booksFolder)
            {
                await GenerateBook(bookFolder);
            }
        }

        private async Task GenerateBook(string bookFolder)
        {
            //TODO remove ..\ ... 
            var bookFolderFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bookFolder);

            var filesInBookFolder = Directory.GetFiles(bookFolderFullPath, "*.md");

            //==false is a small hack, because false (0) is before true (1) and this is not desirable in this case.
            var bookFiles = filesInBookFolder.Select(fileName => new FileInfo(fileName))
                                             .OrderBy(f => f.Name == "README.md" == false)
                                             .ThenBy(f => f.Name == "toc.md" == false)
                                             .ThenBy(f => f.Name.StartsWith("ch") == false)
                                             .ThenBy(f => f.Name.StartsWith("Ap") == false)
                                             .ThenBy(f => f.Name == "foreword.md" == false)
                                             .ThenBy(f => f.Name)
                                             ;

            var bookContent = new StringBuilder();
            foreach (var bookFile in bookFiles)
            {
                bookContent.Append(File.ReadAllText(bookFile.FullName));
            }

            File.WriteAllText(Path.Combine(bookFolderFullPath, bookFolder + ".md"), bookContent.ToString());
        }
    }
}

