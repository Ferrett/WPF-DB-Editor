using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
namespace ShopWpf.Models
{
    public record GameStats
    {
        public int id { get; set; }

        public int userID { get; set; }

        public int gameID { get; set; } 

        public float hoursPlayed { get; set; }

        public int achievementsGot { get; set; }

        public DateTime purchasehDate { get; set; }

        public DateTime lastLaunchDate { get; set; }

        public int reviewID { get; set; }
    }
}
