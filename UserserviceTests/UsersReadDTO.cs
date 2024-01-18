using System.ComponentModel.DataAnnotations;

namespace UserService.DTO
{
    public class UsersReadDTO
    {
        [Key]
        [Required]
        public Guid Uid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }

        public string FavouriteTeam { get; set; }

        //[Display(Name = "Upload File")]
        //[DataType(DataType.Upload)]
        public Uri Image { get; set; }

        //public string ImageUrl { get; set; }
        [Required]

        public string Country { get; set; }

        [Required]

        public string UidAuth { get; set; }
    }
}
