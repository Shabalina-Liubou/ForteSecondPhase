using DemoOybek.Data;
using DemoOybek.DomainObjects;
using DemoOybek.Services.Errors;
using DemoOybek.Services.Implementation;
using DemoOybek.Services.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DemoOybek.Services.Test
{
    public class AuthServiceTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AuthService_Login_Success()
        {
            AuthService authService = GetService();

            var loginModel = new LoginViewModel
            {
                Email = "john.doe@example.com",
                Password = "123456aa"
            };

            var result = authService.Login(loginModel);
            Assert.IsNotNull(result);
        }

        [Test]
        public void AuthService_Login_NotFound()
        {
            var service = GetService();

            try
            {
                service.Login(new LoginViewModel
                {
                    Email = "non_existing_user@example.com",
                    Password = "123456aa"
                });
                Assert.Fail("Did not throw necessary exception");
            }
            catch (NotFoundException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Threw wrong exception of type {ex.GetType().Name}");
            }
        }

        [Test]
        public void AuthService_Login_WrongPassword()
        {
            var service = GetService();

            Assert.Throws<AuthenticationException>(() => service.Login(new LoginViewModel
            {
                Email = "john.doe@example.com",
                Password = "Wrong password"
            }), "Did not throw expected exception AuthenticationException");
        }

        [Test]
        public void AuthService_Login_EmptyViewModel()
        {
            var service = GetService();

            Assert.Throws<ArgumentNullException>(() => service.Login(null),
                "Did not React to null viewmodel");
        }

        [Test]
        public void AuthService_Login_EmptyEamilArgument()
        {
            var service = GetService();

            Assert.Throws<ArgumentException>(() => service.Login(new LoginViewModel
            {
                Email = "",
                Password = "Wrong password"
            }), "Did not React to empty email");
        }

        [Test]
        public void AuthService_Login_EmptyPasswordArgument()
        {
            var service = GetService();

            Assert.Throws<ArgumentException>(() => service.Login(new LoginViewModel
            {
                Email = "john.doe@example.com",
                Password = ""
            }), "Did not React to empty password");
        }

        [Test]
        public void AuthService_Register_Success()
        {
            var service = GetService();

            var result = service.Register(new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "123456aa",
                PasswordSecondTime = "123456aa",
                HasAcceptedTerms = true
            });
            Assert.IsNotNull(result);
        }

        [Test]
        public void AuthService_Register_ViewModelNull()
        {
            var service = GetService();

            Assert.Throws<ArgumentNullException>(() => service.Register(null));
        }

        [Test]
        public void AuthService_Register_EmailEmpty()
        {
            var service = GetService();

            Assert.Throws<ArgumentException>(() => service.Register(new RegisterViewModel
            {
                Email = "",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "123456aa",
                PasswordSecondTime = "123456aa",
                HasAcceptedTerms = true
            }));
        }

        [Test]
        public void AuthService_Register_PasswordEmpty()
        {
            var service = GetService();

            Assert.Throws<ArgumentException>(() => service.Register(new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "",
                PasswordSecondTime = "",
                HasAcceptedTerms = true
            }));
        }

        [Test]
        public void AuthService_Register_EmailDuplicate()
        {
            var service = GetService();

            Assert.Throws<DuplicateException>(() => service.Register(new RegisterViewModel
            {
                Email = "john.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "123456aa",
                PasswordSecondTime = "123456aa",
                HasAcceptedTerms = true
            }));
        }


        [Test]
        public void AuthService_Register_WeakPassword()
        {
            var service = GetService();

            Assert.Throws<WeakPasswordException>(() => service.Register(new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "123",
                PasswordSecondTime = "123",
                HasAcceptedTerms = true
            }));
        }

        [Test]
        public void AuthService_Register_PasswordMismatch()
        {
            var service = GetService();

            Assert.Throws<PasswordMismatchException>(() => service.Register(new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "87^&*hHAA",
                PasswordSecondTime = "AS@@Dm87__)",
                HasAcceptedTerms = true
            }));
        }

        [Test]
        public void AuthService_Register_TermsNotAccepted()
        {
            var service = GetService();

            Assert.Throws<TermsNotAcceptedException>(() => service.Register(new RegisterViewModel
            {
                Email = "newuser@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Password = "123456aa",
                PasswordSecondTime = "123456aa",
                HasAcceptedTerms = false
            }));
        }

        private static IRepository<User> GetRepository()
        {
            var mockUserRepository = new Mock<IRepository<User>>();

            mockUserRepository.Setup(x => x.HashPassword(It.IsAny<string>())).Returns((string str) =>
            {
                var crypt = new System.Security.Cryptography.SHA256Managed();
                var hash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                return hash.ToString();
            });

            var list = new List<User>
            {
                new User
                    {
                        UserId = 1,
                        DateCreated = DateTime.Now,
                        Email = "john.doe@example.com",
                        FirstName = "John",
                        LastName = "Doe",
                        LastLoggedIn = DateTime.Now,
                        PasswordHash = mockUserRepository.Object.HashPassword("123456aa"),
                        Role = "Admin"
                    }
            };

            mockUserRepository.Setup(a => a.Where(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((Expression<Func<User, bool>> x) => list.Where(x.Compile()).AsQueryable());
            mockUserRepository.Setup(a => a.GetAll()).Returns(() => list.AsQueryable());

            mockUserRepository.Setup(a => a.SaveChanges()).Returns(0);

            mockUserRepository.Setup(a => a.Insert(It.IsAny<User>())).Callback((User entry) => {
                if (entry == null)
                    throw new ArgumentNullException(nameof(entry));
                entry.UserId = list.Max(x => x.UserId) + 1;
                list.Add(entry);
            });

            return mockUserRepository.Object;
        }

        private static AuthService GetService()
        {
            var mockRepo = GetRepository();
            var authService = new AuthService(mockRepo);
            return authService;
        }
    }
}