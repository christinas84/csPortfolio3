using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace csPortfolio3.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime Sendtime { get; set; }

        public bool Success { get; set; }
    }
}