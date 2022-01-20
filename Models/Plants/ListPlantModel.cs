using System;

namespace WebApi.Models.Plants
{
    public class ListPlantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBeingWatered { get; set; }
        public DateTime LastWateredTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
