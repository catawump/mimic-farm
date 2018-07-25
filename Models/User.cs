using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DefaultProject.Models
{
    public class User : BaseEntity
    {

        [Required(ErrorMessage ="Field is required!")]
        [RegularExpression("^[a-zA-Z ]*$",ErrorMessage="Your name can only contain letters!")]
        [MinLength(2)]
        public string first_name {get;set;}

        [Required(ErrorMessage ="Field is required!")]
        [RegularExpression("^[a-zA-Z ]*$",ErrorMessage="Your name can only contain letters!")]
        [MinLength(2)]
        public string last_name {get;set;}

        [Required(ErrorMessage ="Field is required!")]
        [MinLength(3)]
        public string username {get;set;}

        [Required(ErrorMessage ="Field is required!")]
        [EmailAddress]
        public string email {get;set;}

        [Required(ErrorMessage ="Field is required!")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at least one: upper case (A-Z) or lower case (a-z) letter, number (0-9) and special character (e.g. !@#$%^&*)")]
        [MinLength(8)]    
        public string password {get;set;}
        
        [Compare("password",ErrorMessage="Your passwords must match!")]
        public string confirm_password {get;set;}

        public int level {get;set;}

        public List<Mimic> mimics {get;set;}
        public List<Item> items {get;set;}

        public User(){
            mimics = new List<Mimic>();
            items = new List<Item>();
            level = 1;
        }
    }
}