using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using NetBuild.Domain.Managers;
using NetBuild.Infrastructure.AzureBlobStorage.Configuration;

namespace NetBuild.Infrastructure.AzureBlobStorage
{
    public class AzureBlobStorageManager : IBlobStorageManager
    {
        private readonly BlobServiceClient _blobClient;

        public AzureBlobStorageManager(IOptions<AzureBlobStorageOption> options)
        {
            _blobClient = new BlobServiceClient(options.Value.ConnectionString);
        }

        public async Task<byte[]?> DownloadAsync(string path, string fileName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(path);
            if (!await containerClient.ExistsAsync())
            {
                return null;
            }

            var blobClient = containerClient.GetBlobClient(fileName);
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var result = await blobClient.DownloadStreamingAsync();

            using (var memory = new MemoryStream())
            {
                result.Value.Content.CopyTo(memory);
                memory.Position = 0;

                return memory.ToArray();
            }
        }

        public async Task<string> UploadAsync(string path, string fileName, byte[] data)
        {
            var containerClient = _blobClient.GetBlobContainerClient(path.Split('/')[0]);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            using (var memory = new MemoryStream(data))
            {
                var extension = MimeTypes.GetMimeType(fileName);
                await blobClient.UploadAsync(memory, new BlobHttpHeaders { ContentType = extension });
                
                return blobClient.Uri.ToString();
            }
        }

        public async Task<IEnumerable<string>> GetFilesAsync(string[] containerPath)
        {
            var files = new List<string>();
            var client = _blobClient.GetBlobContainerClient(containerPath[0]);
            
            if (!await client.ExistsAsync())
            {
                return files;
            }

            AsyncPageable<BlobHierarchyItem> blobHierarchyItemsPageable = client.GetBlobsByHierarchyAsync(prefix: containerPath[1]);
            IAsyncEnumerator<BlobHierarchyItem> blobHierarchyItemsEnumerator = blobHierarchyItemsPageable.GetAsyncEnumerator();
            
            try
            {
                while (await blobHierarchyItemsEnumerator.MoveNextAsync())
                {
                    BlobHierarchyItem container = blobHierarchyItemsEnumerator.Current;
                    if (container.IsBlob && !container.Blob.Deleted)
                    {
                        var path = $"{_blobClient.Uri}{containerPath[0]}/{container.Blob.Name}";
                        files.Add(path);
                    }
                }
            }
            finally
            {
                await blobHierarchyItemsEnumerator.DisposeAsync();
            }

            return files;
        }
    }
}