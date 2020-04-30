using System.ComponentModel.DataAnnotations;
namespace MyProject.Models
{
    public class Banana
    {
        [Key]
        public int BananaId{get; set;}
        public int UserId {get;set;}
        public int MovieId{get; set;}

        public User NavUser {get;set;}
        public Movie NavMovie{get; set;}
    }
}