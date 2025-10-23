namespace NetBuild.Domain.Managers
{
    public interface IBlobStorageManager
    {
        Task<byte[]?> DownloadAsync(string path, string fileName);
        Task<string> UploadAsync(string path, string fileName, byte[] data);
        Task<IEnumerable<string>> GetFilesAsync(string[] containerPath);
    }
}
