using System;
using System.IO;

namespace SharesonServer.Model
{
    public static class TotalImages
    {
        public static string[] TotaltemsInFoldersAnime;
        public static string[] TotaltemsInFoldersReal;
        public static string[] TotaltemsInFoldersMemy;

        public static string GetRandom(string pathToFolder)
        {
            Random random = new Random();

            if(pathToFolder.Contains("Anime"))
            {
                return Path.GetFileName(TotaltemsInFoldersAnime[random.Next(0, TotaltemsInFoldersAnime.Length)]);
            }
            else if(pathToFolder.Contains("Memy"))
            {
                return Path.GetFileName(TotaltemsInFoldersMemy[random.Next(0, TotaltemsInFoldersMemy.Length)]);
            }
            else if(pathToFolder.Contains("Real"))
            {
                return Path.GetFileName(TotaltemsInFoldersReal[random.Next(0, TotaltemsInFoldersReal.Length)]);
            }
            else
            {
                return null;
            }
        }
    }
}
