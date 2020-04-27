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
        [MinLength(25)]

        public string Content {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public int UserId {get;set;}
        public int MovieId{get; set;}

        public User NavUser {get;set;}
        public Movie NavMovie{get; set;}

    }
}