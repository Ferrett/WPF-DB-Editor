using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public class Developer
    {
        public int id { get; set; }

        public string name { get; set; } = null!;

        public string logoURL { get; set; } = null!;

        public DateTime registrationDate { get; set; }

        public ICollection<Game>? publishedGames { get; set; } = new List<Game>();
    }
}
