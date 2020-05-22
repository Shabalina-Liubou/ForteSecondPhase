using System;
using System.Collections.Generic;
using System.Text;

namespace DemoOybek.Services.ViewModels
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSecondTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool HasAcceptedTerms { get; set; }
    }
}
