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

        public int color {get;set;}

        public int hp {get;set;}

        public int hunger {get;set;}

        public int str {get;set;}

        public int dex {get;set;}

        public int inte {get;set;}

        public int xp {get;set;}

        public int lvl {get;set;}

        public Mimic(){
        }

        public Mimic(User owner, string name, string species, int color, int hp, int hunger, int str, int dex, int inte, int xp, int lvl){
            this.owner = owner;
            this.species = species;
            this.color = color;
            this.hp = hp;
            this.hunger = hunger;
            this.str = str;
            this.dex = dex;
            this.inte = inte;
            this.xp = xp;
            this.lvl = lvl;
        }
    }
}