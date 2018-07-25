using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DefaultProject.Models
{
    public abstract class BaseEntity {

        [Key]
        public int id {get; set;}

        public DateTime created_at {get; set;}

        public DateTime updated_at {get; set;}

    }
}