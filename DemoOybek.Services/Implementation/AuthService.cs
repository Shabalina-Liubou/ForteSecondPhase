using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (string.IsNullOrWhiteSpace(viewModel.Email))
                throw new ArgumentException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(viewModel.Password))
                throw new ArgumentException("Password cannot be empty");

            var userExists = Repository.Where(x => x.Email == viewModel.Email).Any();
            if (userExists)
                throw new DuplicateException();

            ValidatePassword(viewModel.Password);
            var passwordHashed = HashPassword(viewModel.Password);

            CheckPasswordMismatch(viewModel.Password, viewModel.PasswordSecondTime);

            if (!viewModel.HasAcceptedTerms)
                throw new TermsNotAcceptedException("Please, accept the terms, if you want to use our product.");

            var user = new User
            {
                DateCreated = DateTime.UtcNow,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Role = Roles.User.ToString(),
                LastLoggedIn = DateTime.UtcNow,
                PasswordHash = passwordHashed,
                HasAcceptedTerms = true
            };

            Repository.Insert(user);
            Repository.SaveChanges();

            return new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                UserId = user.UserId
            };
        }

        public string HashPassword(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        private void ValidatePassword(string password)
        {
            if (password.Length <= 3)
                throw new WeakPasswordException();
        }

        private void CheckPasswordMismatch(string firstPassword, string secondPassword)
        {
            if (!firstPassword.Equals(secondPassword))
                throw new PasswordMismatchException();
        }

    }
}
