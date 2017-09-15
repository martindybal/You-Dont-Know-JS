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
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var projectFolderFullPath = appFolder.ClosestParent("You-Dont-Know-JS").FullName;
            var bookInputFolderFullPath = Path.Combine(projectFolderFullPath, bookFolder);

            var mdFilesInBookFolder = Directory.GetFiles(bookInputFolderFullPath, "*.md");

            //==false is a small hack, because false (0) is before true (1) and this is not desirable in this case.
            var bookFiles = mdFilesInBookFolder.Select(fileName => new FileInfo(fileName))
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

            var bookOutputFolderFullPath = Path.Combine(projectFolderFullPath, "_generated_books", bookFolder);
            var bookFullPath = Path.Combine(bookOutputFolderFullPath, $"_{bookFolder}.md");
            File.WriteAllText(bookFullPath, bookContent.ToString());

            var imagesFilesInBookFolder = Directory.EnumerateFiles(bookInputFolderFullPath)
                                                   .Select(fileName => new FileInfo(fileName))
                                                   .Where(file => file.Extension == "png" ||
                                                                  file.Extension == "jpg" ||
                                                                  file.Extension == "jpeg");

            foreach (var imageFile in imagesFilesInBookFolder)
            {
                File.Copy(imageFile.FullName, Path.Combine(bookInputFolderFullPath, imageFile.Name));
            }
        }
    }
}

