using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public record User
    {
        public int id { get; set; }

        public string login { get; set; } = null!;

        public string passwordHash { get; set; } = null!;

        public string nickame { get; set; } = null!;

        public string avatarURL { get; set; } = null!;

        public string? email { get; set; }

        public DateTime creationDate { get; set; }
    }
}