using System;
using masz.Models;

namespace masz.Events
{
    public class FileUploadedEventArgs : EventArgs
    {
        private FileInfo _fileInfo;

        public FileUploadedEventArgs(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public FileInfo GetFileInfo()
        {
            return _fileInfo;
        }
    }
}