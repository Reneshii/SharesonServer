using SharesonServer.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharesonServer.Repository.SupportFunctions
{
    public static class All_Images
    {
        public static List<FoldersAndFiles> ImagesData = new List<FoldersAndFiles>();

        public static string GetRandom(string pathToFolder)
        {
            Random random = new Random();
            string itemToReturn ="";

            var instance = ImagesData.Where(f => pathToFolder.Contains(f.DirectoryPath)).FirstOrDefault();

            itemToReturn = instance.Files[random.Next(0, instance.Files.Length)];

            //foreach (var item in UsedFiles)
            //{
            //    AllFiles.Remove(AllFiles.Where(f => f.Files.Contains(item)).FirstOrDefault());
            //}
            
            return itemToReturn;

        }
    }
}
