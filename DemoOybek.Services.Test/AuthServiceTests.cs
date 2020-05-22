using DemoOybek.Services.Errors;
using DemoOybek.Services.Implementation;
using DemoOybek.Services.Test.Mocks;
using DemoOybek.Services.ViewModels;
using NUnit.Framework;
using System;

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
            Assert.Throws<AuthenticationException>(() => service.Login(new LoginViewModel { 
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
            var result = service.Register(new RegisterViewModel { 
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

        //WeakPasswordException


        private static AuthService GetService()
        {
            var mockRepo = new MockUserRepository();
            var authService = new AuthService(mockRepo);
            return authService;
        }
    }
}