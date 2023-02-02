using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public class Game
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(25),MinLength(4)]
        public string Name { get; set; } = null!;

        [Required]
        public string LogoURL { get; set; } = null!;

        [Required]
        public float Price { get; set; }

        [Required]
        public  Developer Developer { get; set; } = null!;

        [Required]
        public DateTime PublishDate { get; set; }

        public int AchievementsCount { get; set; }

        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
