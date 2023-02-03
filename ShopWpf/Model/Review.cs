using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ShopWpf.Models
{
    public record Review
    {
        public int id { get; set; }

        public string? text { get; set; }

        public bool isPositive { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastEditDate { get; set; }

        public int gameID { get; set; }

        public int userID { get; set; }
    }
}
