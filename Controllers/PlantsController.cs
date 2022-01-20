using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Entities;
using WebApi.Models.Users;
using System.Threading.Tasks;
using WebApi.Models.Plants;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PlantsController : ControllerBase
    {
        private IPlantService _plantService;
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public PlantsController(
            IUserService userService,
            IPlantService plantService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _plantService = plantService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> ListPlants()
        {
            var User = ValidateAndGetUser();
            if (User is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var plants = await _plantService.GetAll();
            return Ok(plants);
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] CreatePlantModel Plant)
        {
            var User = ValidateAndGetUser();
            if (User is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            try
            {
                var plantEntity = _mapper.Map<Plant>(Plant);
                var newPlant = await _plantService.Create(plantEntity, User.Id);
                if (newPlant is null || newPlant.Id <= 0)
                    return BadRequest("Plant Not Created");
                return Ok(newPlant);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("Water/{plantId}")]
        public async Task<IActionResult> StartWatering([FromRoute] int plantId)
        {
            var User = ValidateAndGetUser();
            if (User is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            try
            {
                var errId = await _plantService.StartWater(plantId, User.Id);
                if (errId == 1)
                    return BadRequest("Gap needed from last session");
                if (errId == -1)
                    return BadRequest("Plant Not found");
                return Ok("Success");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("StopWater/{plantId}")]
        public async Task<IActionResult> StopWatering([FromRoute] int plantId)
        {
            var User = ValidateAndGetUser();
            if (User is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            try
            {
                var errId = await _plantService.StopWater(plantId, User.Id);
                if (errId == -1)
                    return BadRequest("Plant Not found");
                return Ok("Success");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plant = await _plantService.GetById(id);
            var plantModel = _mapper.Map<ListPlantModel>(plant);
            return Ok(plantModel);
        }

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var User = ValidateAndGetUser();
            if (User is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            await _plantService.Delete(id, User.Id);
            return NoContent();
        }

        private User ValidateAndGetUser()
        {
            var usrIdentity = User.Identity.Name;
            int.TryParse(usrIdentity, out int userId);

            if (usrIdentity is null || userId == 0)
                return null;
            else
            {
                var user = _userService.GetById(userId);

                if (user == null)
                    return null;
                else return user;
            }
        }
    }
}
