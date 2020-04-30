using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models
{
    public class Comment
    {
        [Key]

        public int CommentId {get;set;}
        [Required]
        [Display (Name = "Comment must be at least 5 characters long")]
        public string Content {get;set;}
        public int UserId{get; set;}
        public int MovieId{get; set;}
        public User NavCUser{get; set;}
        public Movie NavCMovie{get; set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        

    }
}