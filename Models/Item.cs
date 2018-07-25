using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DefaultProject.Models
{
    public class Item : BaseEntity
    {
        public User owner {get;set;}

        public string name {get;set;}

        public string type {get;set;}

        public string image {get;set;}

        public int value {get;set;}

        public int number {get;set;}

        public Item(){
        }

        public Item(User owner, string name, string type, string image, int value, int number){
            this.owner = owner;
            this.name = name;
            this.type = type;
            this.image = image;
            this.value = value;
            this.number = number;
        }
    }
}