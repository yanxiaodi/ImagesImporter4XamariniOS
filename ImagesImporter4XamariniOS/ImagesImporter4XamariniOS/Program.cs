using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImagesImporter4XamariniOS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World! Welcome to ImagesImporter4iOS. Please input the image folder path:");
                var path = Console.ReadLine();
                var currentDirectory = GetDirectory(path);
                var images = GetImages(currentDirectory);

                Console.WriteLine("Please input the Asset Catalog Name:");
                var assetCatalogName = Console.ReadLine();

                GenerateResources(assetCatalogName, currentDirectory, images);
                Console.WriteLine($"Completed! Please copy the {assetCatalogName}.xcassets folder into your root folder of your iOS project, and copy the content of the csproject.txt into your *.csproj file.");
                Console.WriteLine("Press Enter to exit.");
                var enter = Console.ReadKey();
                if (enter.Key == ConsoleKey.Enter)
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            
        }

        private static void GenerateResources(string assetCatalogName, DirectoryInfo currentDirectory, List<FileInfo> images)
        {
            var targetDirectory = currentDirectory.CreateSubdirectory($"{assetCatalogName}.xcassets");
            StringBuilder sb = new StringBuilder();
            foreach (var fileInfo in images.AsParallel())
            {
                var imageFolder =
                    targetDirectory.CreateSubdirectory($"{fileInfo.Name.Replace(fileInfo.Extension, "")}.imageset");
                fileInfo.CopyTo(Path.Combine(imageFolder.FullName, fileInfo.Name));
                string content = File.ReadAllText("Contents.json");
                File.WriteAllText(Path.Combine(imageFolder.FullName, "Contents.json"),
                    content.Replace("{filename}", fileInfo.Name));
                sb.Append(
                    $@"<ImageAsset Include=""{assetCatalogName}.xcassets\{fileInfo.Name.Replace(fileInfo.Extension, "")}.imageset\Contents.json"">");
                sb.Append(Environment.NewLine);
                sb.Append(@"<Visible>false</Visible>");
                sb.Append(Environment.NewLine);
                sb.Append(@"</ImageAsset>");
                sb.Append(Environment.NewLine);
                sb.Append(
                    $@"<ImageAsset Include=""{assetCatalogName}.xcassets\{fileInfo.Name.Replace(fileInfo.Extension, "")}.imageset\{fileInfo.Name}"">");
                sb.Append(Environment.NewLine);
                sb.Append(@"<Visible>false</Visible>");
                sb.Append(Environment.NewLine);
                sb.Append(@"</ImageAsset>");
                sb.Append(Environment.NewLine);
            }

            File.WriteAllText(Path.Combine(targetDirectory.FullName, "csproject.txt"), sb.ToString());
        }

        private static DirectoryInfo GetDirectory(string path)
        {

            bool isPathValid = false;
            do
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("No such folder. Please input a valid folder path:");
                }
                else
                {
                    isPathValid = true;
                }
            }
            while (isPathValid == false);
            return new DirectoryInfo(path);
        }

        private static List<FileInfo> GetImages(DirectoryInfo directory)
        {
            var allFiles = directory.GetFiles();
            var images = allFiles.Where(x => x.Extension.ToLower() == ".jpg" || x.Extension.ToLower() == ".png").ToList();
            return images;
        }
    }
}
