using System;

namespace WebApi.Entities
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBeingWatered { get; set; }
        public DateTime LastWateredTime { get; set;}
        public int LastWateredBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
