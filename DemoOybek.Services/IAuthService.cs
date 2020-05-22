using DemoOybek.Services.ViewModels;
using System;

namespace DemoOybek.Services
{
    public interface IAuthService
    {
        UserViewModel Login(LoginViewModel viewModel);
        UserViewModel Register(RegisterViewModel viewModel);
    }
}
