using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public class Review
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(9999)]
        public string? Text { get; set; }

        [Required]
        public bool IsPositive { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }

        [Required]
        public int GameID { get; set; }

        [Required]
        public int UserID { get; set; }
    }
}
