﻿using System;

namespace SoborniyProject.database.Models
{
    public class LightTraffic
    {
        public Int64 Id { get; set; }
        public Int64? SessionId { get; set; }
        public int PositionId { get; set; }
        public int RedLightDurationSec { get; set; }
        public int YellowLightDurationSec { get; set; }
        public int GreenLightDurationSec { get; set; }
        public int StartColor { get; set; }
        public int NextColor { get; set; }
        public int Status { get; set; }
        public float PreviousDistance  { get; set; }
        public DateTime CreatedAt{ get; set; } 
        public DateTime UpdatedAt{ get; set; } 
        
        public Session Session { get; set; } 
    }
}