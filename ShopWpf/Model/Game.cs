using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public record Game
    {
        public int id { get; set; }

        public string name { get; set; } = null!;

        public string logoURL { get; set; } = null!;

        public float price { get; set; }

        public int developerID { get; set; } 

        public DateTime publishDate { get; set; }

        public int achievementsCount { get; set; }
    }
}
