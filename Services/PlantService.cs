using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IPlantService
    {
        Task<IEnumerable<Plant>> GetAll();
        Task<int> StartWater(int id, int userId);
        Task<int> StopWater(int id, int userID);
        Task<Plant> GetById(int id);
        Task<Plant> Create(Plant plant, int userId);
        //Task Update(Plant plant, int id);
        Task Delete(int id, int userId);
    }

    public class PlantService : IPlantService
    {
        private DataContext _context;
        int timeToFillWater = 10;
        int restGapBetweenFillingSession = 30;

        public PlantService(DataContext context, IConfiguration configuration)
        {
            _context = context;

            var timeToFillWaterConfig = configuration.GetValue<int>("TimeToFillWater");
            var restGapBetweenFillingSessionConfig = configuration.GetValue<int>("RestGapBetweenFillingSession");

            if (timeToFillWaterConfig > 0)
                timeToFillWater = timeToFillWaterConfig;

            if (restGapBetweenFillingSessionConfig > 0)
                restGapBetweenFillingSession = restGapBetweenFillingSessionConfig;
        }

        public async Task<Plant> Create(Plant plant, int userId)
        {
            plant.CreatedDate = DateTime.UtcNow;
            plant.LastUpdatedDate = DateTime.UtcNow;
            plant.LastWateredTime = DateTime.UtcNow;
            plant.IsBeingWatered = false;
            plant.IsActive = true;
            plant.CreatedBy = userId;
            plant.LastUpdatedBy = userId;
            plant.LastWateredBy = userId;

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return plant;
        }

        public async Task Delete(int id, int userId)
        {
            var plant = await _context.Plants.FirstOrDefaultAsync(x => x.Id == id);
            if (plant != null)
            {
                plant.LastUpdatedBy = userId;
                plant.IsActive = false;

                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Plant>> GetAll()
        {
            return await _context.Plants.ToListAsync();
        }

        public async Task<Plant> GetById(int id)
        {
            return await _context.Plants.FirstOrDefaultAsync(x => x.Id == id);
        }

        //Starts to water plants
        //returns error code in case of some error
        // errCode => 0 -> no error; 1 -> gap needed from last watering session;
        // -1 -> plant not found
        public async Task<int> StartWater(int id, int userId)
        {
            int errCode = -1;
            var plant = await _context.Plants.FirstOrDefaultAsync(x => x.Id == id);
            if (plant != null)
            {
                var currentTime = DateTime.UtcNow;

                if (plant.LastWateredTime.AddSeconds(restGapBetweenFillingSession) > currentTime)
                    errCode = 1;

                else
                {
                    plant.LastUpdatedBy = userId;
                    plant.LastUpdatedDate = currentTime;

                    plant.LastWateredBy = userId;
                    plant.LastWateredTime = currentTime;

                    _context.Plants.Update(plant);
                    await _context.SaveChangesAsync();
                    errCode = 0;
                }
            }
            return errCode;
        }

        //Stops to water plants
        //returns error code in case of some error
        // errCode => 0 -> no error; 1 -> already stopped;
        // -1 -> plant not found

        public async Task<int> StopWater(int id, int userId)
        {
            int errCode = -1;
            var plant = await _context.Plants.FirstOrDefaultAsync(x => x.Id == id);
            if (plant != null)
            {
                var currentTime = DateTime.UtcNow;

                if (plant.LastWateredTime.AddSeconds(timeToFillWater) < currentTime)
                    errCode = 1;

                else
                {
                    plant.LastWateredTime = plant.LastWateredTime.AddSeconds(-timeToFillWater);
                    plant.LastUpdatedBy = userId;
                    plant.LastUpdatedDate = currentTime;

                    _context.Plants.Update(plant);
                    await _context.SaveChangesAsync();
                    errCode = 0;
                }
            }
            return errCode;
        }


        public void Update(Plant user, int id)
        {
            throw new NotImplementedException();
        }
    }
}