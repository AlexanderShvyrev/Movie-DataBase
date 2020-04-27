using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;


namespace MyProject.Models
{
    public class MovieCreate
    {
        [Required]
        public string Title {get; set;}
        [Required]
        public int Year{get; set;}
        [Required]
        public string Director{get; set;}
        [Required]
        [Range(1,10)]
        [Display(Name = "IMDb Rating")]
        public int Rating{get; set;}
        [Display(Name = "Main cast")]
        
        public string Stars{get; set;}
        [Required]
        public string Description{get; set;}
        // [Required]
        public IFormFile Image{get; set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}