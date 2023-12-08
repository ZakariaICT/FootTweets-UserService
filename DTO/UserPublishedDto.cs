using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.DTO
{
    public class UserPublishedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
    }
}
