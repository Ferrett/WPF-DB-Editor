using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
namespace ShopWpf.Models
{
    public class GameStats
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int GameID { get; set; } 

        [Required]
        public float HoursPlayed { get; set; }

        public int AchievementsGot { get; set; }

        [Required]
        public DateTime PurchasehDate { get; set; }

        public DateTime LastLaunchDate { get; set; }

        public int ReviewID { get; set; }
    }
}
