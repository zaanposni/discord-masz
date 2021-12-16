
using System.Net.Mime;

namespace MASZ.Models
{
    public class UploadedFile
    {
        public string Name { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public ContentDisposition ContentDisposition { get; set; }
    }
}
