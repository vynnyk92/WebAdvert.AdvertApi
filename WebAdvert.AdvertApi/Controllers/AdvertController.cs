using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Dtos;
using WebAdvert.AdvertApi.Services;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        [HttpPost]
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
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm([FromBody] ConfirmAdvertModel advertModel)
        {
            try
            {
                await _advertStorageService.Confirm(advertModel);
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
