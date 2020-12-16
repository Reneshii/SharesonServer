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
        public static string GetRandom(string pathToFolder, string[] excluded = null)
        {
            string itemToReturn = "";
            Random random = new Random();
            ServerData.TemporaryResources instance = new ServerData.TemporaryResources();
            List<string> temp = new List<string>();

            if(excluded != null && excluded.Length > 0)
            {
                foreach (var item in ImagesData)
                {
                    if(item.Directory.Contains(pathToFolder))
                        instance = (ServerData.TemporaryResources)item.Clone();
                }

                temp = new List<string>(instance.FilesFoundInSharedFolders.ToList());

                foreach (var excludedItem in excluded)
                {
                    temp.RemoveAll(f => f.Contains(excludedItem));
                }

                instance.FilesFoundInSharedFolders = temp.ToArray();
                temp.Clear();
                temp = null;
            }
            else
            {
                instance = ImagesData.Where(f => pathToFolder.Contains(f.Directory)).FirstOrDefault();
            }

            itemToReturn = instance.FilesFoundInSharedFolders[random.Next(0, instance.FilesFoundInSharedFolders.Length)];
            
            return itemToReturn;

        }
    }
}
