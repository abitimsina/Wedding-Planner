using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        public string WedderOne {get;set;}
        [Required]
        public string WedderTwo {get;set;}
        [DataType(DataType.Date)]
        [FutureDate]
        public DateTime Date {get;set;}
        [Required]
        public string Address {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Reservation> Users {get;set;}
    }

     public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (DateTime)value < DateTime.Now ? new ValidationResult("Wedding must take place in future. Common bro! Who comes to sign up for the Wedding and say I wanna do my wedding in the Past! Lol") : ValidationResult.Success;
        }
    }

}
