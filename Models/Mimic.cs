using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DefaultProject.Models
{
    public class Mimic : BaseEntity
    {
        public User owner {get;set;}

        [Required]
        [MinLength(10)]
        public string name {get;set;}

        public string species {get;set;}

        public string color {get;set;}

        public string image {get;set;}

        public int hp {get;set;}

        public int hunger {get;set;}

        public int str {get;set;}

        public int dex {get;set;}

        public int inte {get;set;}


        public Mimic(){
        }

        public Mimic(User owner, string name, string species, string color, string image, int hp, int hunger, int str, int dex, int inte){
            this.owner = owner;
            this.species = species;
            this.color = color;
            this.image = image;
            this.hp = hp;
            this.hunger = hunger;
            this.str = str;
            this.dex = dex;
            this.inte = inte;
        }
    }
}