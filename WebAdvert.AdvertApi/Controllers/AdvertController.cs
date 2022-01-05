using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Dtos;
using WebAdvert.AdvertApi.Services;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Controllers
{
    [Route("api/v1/advert")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;
        private readonly IMessagePublisher _messagePublisher;

        public AdvertController(IAdvertStorageService advertStorageService, IMessagePublisher messagePublisher)
        {
            _advertStorageService = advertStorageService;
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(CreateAdvertResponse),200)]
        public async Task<IActionResult> Create([FromBody]AdvertModel advertModel)
        {
            try
            {
                var recordId = await _advertStorageService.Add(advertModel);
                return Ok(new CreateAdvertResponse
                {
                    Id = recordId
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("confirm")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm([FromBody] ConfirmAdvertModel advertModel)
        {
            try
            {
                await _advertStorageService.Confirm(advertModel);
                await _messagePublisher.PublishAdvertConfirmed(advertModel);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
