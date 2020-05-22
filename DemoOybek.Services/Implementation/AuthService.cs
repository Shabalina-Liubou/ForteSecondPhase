using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoOybek.Data;
using DemoOybek.DomainObjects;
using DemoOybek.Services.Errors;
using DemoOybek.Services.ViewModels;

namespace DemoOybek.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> Repository;
        public AuthService(IRepository<User> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public UserViewModel Login(LoginViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (string.IsNullOrWhiteSpace(viewModel.Email))
                throw new ArgumentException(nameof(viewModel.Email));

            if (string.IsNullOrWhiteSpace(viewModel.Password))
                throw new ArgumentException(nameof(viewModel.Password));

            var user = Repository.Where(x => x.Email == viewModel.Email).FirstOrDefault();
            if (user == null)
            {
                var ex = new NotFoundException($"User with this email not found");
                ex.Data.Add("Email", viewModel.Email);
                throw ex;
            }

            var passwordHash = HashPassword(viewModel.Password);
            if (passwordHash != user.PasswordHash)
            {
                throw new AuthenticationException();
            }

            return new UserViewModel
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };
        }

        public UserViewModel Register(RegisterViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        private string HashPassword(string password)
        {
            return password;
        }
    }
}
