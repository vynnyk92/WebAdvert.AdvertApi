using System;
using Amazon.DynamoDBv2.DataModel;

namespace WebAdvert.AdvertApi.Dtos
{
    [DynamoDBTable("Advert")]
    public class AdvertDto
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public string FilePath { get; set; }
        [DynamoDBProperty]
        public double Price { get; set; }
        [DynamoDBProperty]
        public DateTime CreationDateTime { get; set; }
        [DynamoDBProperty]
        public AdvertStatusDto Status { get; set; }
    }

    public static class AdvertDtoExtensions
    {
        public static void UpdateDbProperties(this AdvertDto advertDto)
        {
            advertDto.Id = Guid.NewGuid().ToString();
            advertDto.CreationDateTime = DateTime.UtcNow;
            advertDto.Status = AdvertStatusDto.Pending;
        }

        public static void ActivateAndSetPath(this AdvertDto advertDto, string filePath)
        {
            advertDto.Status = AdvertStatusDto.Active;
            advertDto.FilePath = filePath;
        }
    }
}
