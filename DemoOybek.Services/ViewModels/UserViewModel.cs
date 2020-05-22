using System;
using System.Collections.Generic;
using System.Text;

namespace DemoOybek.Services.ViewModels
{
    public class UserViewModel
    {
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
