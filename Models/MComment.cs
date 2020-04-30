using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class MComment
    {
        [Key]
        public int MCommentId {get;set;}
        [Required]
        [MinLength(10)]
        [Display (Name = "Comment must be at least 10 characters long")]
        public string MContent {get;set;}

        public int UserId {get;set;}
        public int MessageId {get;set;}
        public User MUser {get;set;}
        public Message MMessage {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}