using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace MyProject.Models
{
    public class Movie
    {
        [Key]

        public int MovieId {get; set;}
        [Required]
        public string Title {get; set;}
        [Required]
        public int Year{get; set;}
        [Required]
        public string Director{get; set;}
        [Required]
        [Range(0.1,10)]
        [Display(Name = "IMDb Rating")]
        public decimal Rating{get; set;}
        [Required]
        [Display(Name = "Main cast")]
        public string Stars{get; set;}
        [Required]
        public string Description{get; set;}
        public int UserId{get; set;}
        // [Required]
        public string ImagePath{get; set;}
        public User Creator{get; set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Comment> Comments {get;set;}
        public List<Banana> Actions{get; set;}

    }
}