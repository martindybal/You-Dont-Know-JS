using System.IO;

namespace BookGenerator.App
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo ClosestParent(this DirectoryInfo folder, string parentFolderName)
        {
            var closestFolder = folder.ClosestParentOrDefault(parentFolderName); ;
            if (closestFolder == null)
            {
                throw new DirectoryNotFoundException($"Directory {parentFolderName} wasn't found in {folder.FullName}");
            }
            return closestFolder;
        }

        public static DirectoryInfo ClosestParentOrDefault(this DirectoryInfo folder, string parentFolderName)
        {
            var parentFolder = folder.Parent;
            if (parentFolder == null)
            {
                return null;
            }
            if (parentFolder.Name == parentFolderName)
            {
                return parentFolder;
            }
            return parentFolder.ClosestParent(parentFolderName);
        }
    }
}