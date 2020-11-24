using SharesonServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharesonServer.Repository.SupportFunctions
{
    public static class All_Images
    {
        public static List<ServerData.TemporaryResources> ImagesData = new List<ServerData.TemporaryResources>();

        public static void UpdateImagesList(string directory, string file)
        {
            List<string> newFilesList = new List<string>();
            var ifExist = ImagesData.Where(f => string.Equals(f.Directory, directory)).FirstOrDefault();
            if(ifExist == null)
            {
                newFilesList.Add(file);
                ImagesData.Add(new ServerData.TemporaryResources()
                {
                    Directory = directory,
                    FilesFoundInSharedFolders = newFilesList.ToArray(), 
                });
            }
            else
            {
                foreach (var item in ifExist.FilesFoundInSharedFolders)
                {
                    newFilesList.Add(item);
                }
                newFilesList.Add(file);

                ImagesData.Add(new ServerData.TemporaryResources()
                {
                    Directory = directory,
                    FilesFoundInSharedFolders = newFilesList.ToArray(),
                });
            }
        }
        public static string GetRandom(string pathToFolder)
        {
            Random random = new Random();
            string itemToReturn ="";

            var instance = ImagesData.Where(f => pathToFolder.Contains(f.Directory)).FirstOrDefault();

            itemToReturn = instance.FilesFoundInSharedFolders[random.Next(0, instance.FilesFoundInSharedFolders.Length)];

            //foreach (var item in UsedFiles)
            //{
            //    AllFiles.Remove(AllFiles.Where(f => f.Files.Contains(item)).FirstOrDefault());
            //}
            
            return itemToReturn;

        }
    }
}
