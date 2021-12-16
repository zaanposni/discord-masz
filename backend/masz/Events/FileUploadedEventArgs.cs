namespace MASZ.Events
{
    public class FileUploadedEventArgs : EventArgs
    {
        private readonly Models.FileInfo _fileInfo;

        public FileUploadedEventArgs(Models.FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public Models.FileInfo GetFileInfo()
        {
            return _fileInfo;
        }
    }
}