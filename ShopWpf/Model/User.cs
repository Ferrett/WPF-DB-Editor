using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(25), MinLength(4)]
        public string Nickame { get; set; } = null!;

        [Required]
        public string AvatarURL { get; set; } = null!;

        public string? Email { get; set; }

        [Required]
        public float MoneyOnAccount { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime LastLogInDate { get; set; }
    }
}