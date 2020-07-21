using Shareson.Support;
using System.Windows.Media.Imaging;

namespace SharesonServer.Model
{
    public class StoredFiles : Property_Changed
    {
        public string Name { get; set; }
        public double SizeInBytes { get; set; }
        public double SizeInMegabytes { get; set; }
        public BitmapImage BitmapImages { get; set; }
    }
}
