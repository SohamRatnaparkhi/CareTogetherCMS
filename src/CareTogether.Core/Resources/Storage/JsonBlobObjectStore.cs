﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Newtonsoft.Json;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CareTogether.Resources.Storage
{
    public class JsonBlobObjectStore<T> : IObjectStore<T>
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly string objectType;
        private readonly ConcurrentDictionary<Guid, BlobContainerClient> organizationBlobContainerClients;

        public JsonBlobObjectStore(BlobServiceClient blobServiceClient, string objectType)
        {
            this.blobServiceClient = blobServiceClient;
            this.objectType = objectType;
            organizationBlobContainerClients = new(); //TODO: Share this across all services using the same blobServiceClient.
        }


        public async Task<OneOf<Success, Error>> DeleteAsync(Guid organizationId, Guid locationId, string objectId)
        {
            try
            {
                var tenantContainer = await CreateContainerIfNotExists(organizationId);
                var objectBlob = tenantContainer.GetBlockBlobClient($"{locationId}/{objectType}/{objectId}.json");

                await objectBlob.DeleteIfExistsAsync();
                return new Success();
            }
            catch (Exception)
            {
                //TODO: Exception detail logging
                return new Error();
            }
        }

        public async Task<OneOf<Success<T>, Error>> GetAsync(Guid organizationId, Guid locationId, string objectId)
        {
            try
            {
                var tenantContainer = await CreateContainerIfNotExists(organizationId);
                var objectBlob = tenantContainer.GetBlockBlobClient($"{locationId}/{objectType}/{objectId}.json");

                var objectStream = await objectBlob.DownloadStreamingAsync();
                var objectText = new StreamReader(objectStream.Value.Content).ReadToEnd();
                var objectValue = JsonConvert.DeserializeObject<T>(objectText);

                return new Success<T>(objectValue);
            }
            catch (Exception)
            {
                //TODO: Exception detail logging
                return new Error();
            }
        }

        public async Task<OneOf<Success, Error>> UpsertAsync(Guid organizationId, Guid locationId, string objectId, T value)
        {
            try
            {
                var tenantContainer = await CreateContainerIfNotExists(organizationId);
                var objectBlob = tenantContainer.GetBlockBlobClient($"{locationId}/{objectType}/{objectId}.json");

                var objectText = JsonConvert.SerializeObject(value);
                var objectStream = new MemoryStream(Encoding.UTF8.GetBytes(objectText));
                await objectBlob.UploadAsync(objectStream,
                    new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" } });

                return new Success();
            }
            catch (Exception)
            {
                //TODO: Exception detail logging
                return new Error();
            }
        }


        private async Task<BlobContainerClient> CreateContainerIfNotExists(Guid organizationId)
        {
            if (organizationBlobContainerClients.ContainsKey(organizationId))
            {
                return organizationBlobContainerClients[organizationId];
            }
            else
            {
                var blobClient = blobServiceClient.GetBlobContainerClient(organizationId.ToString());

                if (!await blobClient.ExistsAsync())
                {
                    await blobClient.CreateAsync();
                }

                organizationBlobContainerClients[organizationId] = blobClient;
                return blobClient;
            }
        }
    }
}