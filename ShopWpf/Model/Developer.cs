using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public class Developer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50), MinLength(4)]
        public string Name { get; set; } = null!;

        [Required]
        public string LogoURL { get; set; } = null!;

        [Required]

        public DateTime RegistrationDate { get; set; }

        public ICollection<Game>? PublishedGames { get; set; } = new List<Game>();
    }
}
