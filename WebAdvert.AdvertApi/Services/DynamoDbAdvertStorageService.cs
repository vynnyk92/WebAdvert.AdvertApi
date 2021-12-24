using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using WebAdvert.AdvertApi.Dtos;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Services
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel advertModel);

        Task<bool> Confirm(ConfirmAdvertModel confirmAdvertModel);

        Task<bool> HealthCheckAsync();
    }

    public class DynamoDbAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public DynamoDbAdvertStorageService(IMapper mapper, IAmazonDynamoDB amazonDynamoDB)
        {
            _mapper = mapper;
            _amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<string> Add(AdvertModel advertModel)
        {
            var dbModel = _mapper.Map<AdvertDto>(advertModel);
            dbModel.UpdateDbProperties();
            using var context = new DynamoDBContext(_amazonDynamoDB);
            await context.SaveAsync(dbModel);

            return dbModel.Id;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel confirmAdvertModel)
        {
            using var context = new DynamoDBContext(_amazonDynamoDB);
            var record = await context.LoadAsync<AdvertDto>(confirmAdvertModel.Id);
            if (record == null)
            {
                throw new KeyNotFoundException($"record with Id={confirmAdvertModel.Id} not found");
            }

            if (confirmAdvertModel.Status.Equals(AdvertStatus.Active))
            {
                record.SetActiveStatus();
                await context.SaveAsync(record);
            }
            else
            {
                await context.DeleteAsync(record);
            }

            return true;
        }

        public async Task<bool> HealthCheckAsync()
        {
            var tableData = await _amazonDynamoDB.DescribeTableAsync("Adverts");
            return tableData.Table.TableStatus.Equals(TableStatus.ACTIVE);
        }
    }
}
