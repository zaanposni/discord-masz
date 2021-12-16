using Microsoft.AspNetCore.StaticFiles;
using System.Security.Cryptography;
using System.Text;

namespace MASZ.Services
{
    public class FilesHandler
    {
        public string GetContentType(string path)
        {
            if (!File.Exists(path))
            {
                return "application/octet-stream";
            }

            new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
            return contentType ?? "application/octet-stream";
        }

        public FileInfo[] GetFilesByDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            return new DirectoryInfo(directory).GetFiles();
        }

        public byte[] ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public async Task<string> SaveFile(IFormFile file, string directory)
        {
            Directory.CreateDirectory(directory);

            var uniqueFileName = GetUniqueFileName(file);
            var filePath = Path.Combine(directory, uniqueFileName);
            await file.CopyToAsync(new FileStream(filePath, FileMode.Create));
            return uniqueFileName;
        }

        public void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || str[i] >= 'A' && str[i] <= 'z'
                        || str[i] == '.' || str[i] == '_')
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }

        private string GetUniqueFileName(IFormFile file)
        {
            string fileName = Path.GetFileName(file.FileName);
            return GetSHA1Hash(file)
                    + "_"
                    + Guid.NewGuid().ToString()[..8]
                    + "_"
                    + RemoveSpecialCharacters(Path.GetFileNameWithoutExtension(fileName))
                    + RemoveSpecialCharacters(Path.GetExtension(fileName));
        }

        private static string GetSHA1Hash(IFormFile file)
        {
            // get stream from file then convert it to a MemoryStream
            MemoryStream stream = new();
            file.OpenReadStream().CopyTo(stream);
            // compute md5 hash of the file's byte array.
            byte[] bytes = SHA1.Create().ComputeHash(stream.ToArray());
            stream.Close();
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }
    }
}