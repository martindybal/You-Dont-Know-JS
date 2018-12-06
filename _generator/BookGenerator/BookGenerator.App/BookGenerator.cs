using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BookGenerator.App
{
    class BookGenerator
    {
        public void GenerateAllBook()
        {
            var booksFolder = new[] { "Up & Going", "Scope & Closures", "this & Object Prototypes", "Types & Grammar", "Async & Performance", "ES6 & Beyond" };
            foreach (var bookFolder in booksFolder)
            {
                GenerateBook(bookFolder);
            }
        }

        private void GenerateBook(string bookFolder)
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

            GenerateMarkdown(bookFolder, bookFiles, projectFolderFullPath, bookInputFolderFullPath);
        }

        private static void GenerateMarkdown(string bookFolder, IOrderedEnumerable<FileInfo> bookFiles, string projectFolderFullPath,
            string bookInputFolderFullPath)
        {
            var bookContent = new StringBuilder();
            foreach (var bookFile in bookFiles)
            {
                if (bookFile.Name == "toc.md")
                {
                    bookContent.Append(GenerateTableOfContent(bookFile));
                }
                else
                {
                    bookContent.Append(File.ReadAllText(bookFile.FullName));
                }
            }

            var bookOutputFolderFullPath = Path.Combine(projectFolderFullPath, "_generated_books", bookFolder);
            var bookFullPath = Path.Combine(bookOutputFolderFullPath, $"_{bookFolder}.md");

            SaveBook(bookFullPath, bookContent.ToString());
            CopyImagesToBookFolder(bookInputFolderFullPath, bookOutputFolderFullPath);
        }

        private static string GenerateTableOfContent(FileInfo bookFile)
        {
            var sb = new StringBuilder();
            foreach (var line in File.ReadAllLines(bookFile.FullName))
            {
                var parts = line.Split('*');
                if (parts.Length == 2)
                {
                    sb.AppendLine($"{parts[0]}* [{parts[1].Trim()}](#{parts[1].Trim().ToLower().Replace(":", string.Empty).Replace("?", string.Empty).Replace(' ', '-')})");
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }

        private static void CreateDirectoryIfNotExist(string bookFullPath)
        {
            FileInfo bookFile = new FileInfo(bookFullPath);
            bookFile.Directory.Create(); // If the directory already exists, this method does nothing.
        }

        private static void SaveBook(string bookFullPath, string bookContent)
        {
            CreateDirectoryIfNotExist(bookFullPath);
            File.WriteAllText(bookFullPath, bookContent);
        }

        private static void CopyImagesToBookFolder(string inputFolderFullPath, string outputFolderFullPath)
        {
            var imageExtensions = new[] { ".png", ".jpg", ".jpeg" };

            var imagesFilesInBookFolder = Directory.EnumerateFiles(inputFolderFullPath)
                .Select(fileName => new FileInfo(fileName))
                .Where(file => imageExtensions.Contains(file.Extension));

            foreach (var imageFile in imagesFilesInBookFolder)
            {
                File.Copy(imageFile.FullName, Path.Combine(outputFolderFullPath, imageFile.Name), true);
            }
        }
    }
}