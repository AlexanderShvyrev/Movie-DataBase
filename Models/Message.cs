using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MyProject.Models
{
    public class Message
    {
        [Key]
        public int MessageId {get;set;}
        [Required]
        [MinLength(10)]
        [Display (Name = "Message must be at least 10 characters long")]
        public string MessagePost {get;set;}
        
        public int UserId {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public User MessageCreator {get;set;}

        public List<MComment> PostedComments {get;set;}
    }
}