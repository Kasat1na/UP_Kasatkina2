using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;
using ReactiveUI;
using WriteErasee.Converter;
using WriteErasee.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WriteErasee.ViewModels
{
	public class AuthorizationViewModel : ViewModelBase
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _captchaInput = string.Empty;
        private string _captchaText = string.Empty; // ��������������� ����� CAPTCHA
        private int _failedAttempts = 0;
        private DateTime _lockTime;

        private User? _currentUser;

        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string CaptchaInput
        {
            get => _captchaInput;
            set => this.RaiseAndSetIfChanged(ref _captchaInput, value);
        }

        public string CaptchaText
        {
            get => _captchaText;
            set => this.RaiseAndSetIfChanged(ref _captchaText, value);
        }
        private bool _isCaptchaRequired;
        public bool IsCaptchaRequired
        {
            get => _isCaptchaRequired;
            set => this.RaiseAndSetIfChanged(ref _isCaptchaRequired, value);
        }


        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> LoginCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GuestCommand { get; }
        private Bitmap? _captchaImage;
        public Bitmap? CaptchaImage
        {
            get => _captchaImage;
            set => this.RaiseAndSetIfChanged(ref _captchaImage, value);
        }


        public AuthorizationViewModel()
        {
            LoginCommand = ReactiveCommand.Create(LoginUser);
            GuestCommand = ReactiveCommand.Create(ToGuest);

            // ��������� CAPTCHA ��� ������
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            CaptchaText = CaptchaGenerator.GenerateCaptcha();
            CaptchaImage = CaptchaGenerator.GenerateCaptchaImage(CaptchaText);
        }

        private async void LoginUser()
        {
            // ���������, ������������ �� ����
            if (_lockTime > DateTime.Now)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "������ �������� ������������",
                    $"���������� ����� ����� {(int)(_lockTime - DateTime.Now).TotalSeconds} ������.",
                    ButtonEnum.Ok
                ).ShowAsync();
                return;
            }

            // ���� CAPTCHA ���������, �� ������� ������� � ��������� CAPTCHA � �������
            if (IsCaptchaRequired && CaptchaInput != CaptchaText)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "������ CAPTCHA",
                    "��������� ��� �� ��������� � ������������. ���������� ��� ���.",
                ButtonEnum.Ok
                ).ShowAsync();

                GenerateCaptcha(); // ��������� CAPTCHA
                return;
            }

            using var db = new _43pKasatkina2upContext(); // �������� ���� ������
            _currentUser = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == Login && u.Password == Password);

            if (_currentUser != null)
            {
                // ���� ����������� �������
                OpenUserWindow(_currentUser);
                _failedAttempts = 0; // ���������� ������� ��������� �������
                CaptchaInput = string.Empty; // ������� ���� CAPTCHA
                IsCaptchaRequired = false; // �������� CAPTCHA
            }
            else
            {
                // ����������� ������� ��������� �������
                _failedAttempts++;

                // ������ ��������� ������� - ���������� CAPTCHA
                if (_failedAttempts == 1)
                {
                    IsCaptchaRequired = true; // ���������� CAPTCHA
                    GenerateCaptcha();
                    await MessageBoxManager.GetMessageBoxStandard(
                        "��������� ��������",
                        "�������� ����� ��� ������. ������� CAPTCHA.",
                        ButtonEnum.Ok
                    ).ShowAsync();
                }
                // ������ ��������� ������� (� CAPTCHA)
                else if (_failedAttempts == 2)
                {
                    // ��������� ������ ����� �� 10 ������
                    _lockTime = DateTime.Now.AddSeconds(10);
                    await MessageBoxManager.GetMessageBoxStandard(
                        "������ ������������",
                        "�������� CAPTCHA. ���� ������������ �� 10 ������.",
                        ButtonEnum.Ok
                    ).ShowAsync();

                    // ������� 10 ������ ����� ��������� ��������
                    await Task.Delay(10000);

                    // ����� 10 ������ ������������ ������ �����
                    _lockTime = DateTime.Now; // ���������� ����� ����������
                }
            }
        }





        private void OpenUserWindow(User user)
        {
            MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel(user) };
        }

        private void ToGuest()
        {
            MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel() };
        }
    }
}