using SharesonServer.Model.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharesonServer.Model
{
    public static class All_Images
    {
        public static List<FoldersAndFiles> AllFiles = new List<FoldersAndFiles>();
        
        public static string GetRandom(string pathToFolder)
        {
            Random random = new Random();
            string itemToReturn ="";

            var instance = AllFiles.Where(f => pathToFolder.Contains(f.DirectoryPath)).FirstOrDefault();

            itemToReturn = instance.Files[random.Next(0, instance.Files.Length)];
            return itemToReturn;

        }
    }
}
