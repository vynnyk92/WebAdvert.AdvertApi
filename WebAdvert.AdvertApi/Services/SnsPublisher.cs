using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebAdvert.AdvertApi.Settings;
using WebAdvert.Models;
using WebAdvert.Models.Messages;

namespace WebAdvert.AdvertApi.Services
{
    public interface IMessagePublisher
    {
        Task PublishAdvertConfirmed(ConfirmAdvertModel confirmAdvertModel);
    }
    public class SnsPublisher : IMessagePublisher
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly SNSConfig _snsConfig;
        private readonly IAdvertStorageService _dynamoDbAdvertStorageService;

        public SnsPublisher(IAmazonSimpleNotificationService amazonSimpleNotificationService,
            IAdvertStorageService dynamoDbAdvertStorageService,
            IOptions<SNSConfig> snsConfig)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _snsConfig = snsConfig.Value;
            _dynamoDbAdvertStorageService = dynamoDbAdvertStorageService;
        }

        public async Task PublishAdvertConfirmed(ConfirmAdvertModel confirmAdvertModel)
        {
            var advert = await _dynamoDbAdvertStorageService.GetAdvert(confirmAdvertModel.Id);
            var message = new AdvertConfirmedMessage()
            {
                Id = confirmAdvertModel.Id,
                Title = advert.Title
            };
            var messageString = JsonConvert.SerializeObject(message);
            await _amazonSimpleNotificationService.PublishAsync(_snsConfig.TopicArn, messageString);
        }
    }
}
