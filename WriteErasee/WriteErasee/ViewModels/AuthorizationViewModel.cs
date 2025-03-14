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
        private string _captchaText = string.Empty; // Сгенерированный текст CAPTCHA
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

            // Генерация CAPTCHA при старте
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            CaptchaText = CaptchaGenerator.GenerateCaptcha();
            CaptchaImage = CaptchaGenerator.GenerateCaptchaImage(CaptchaText);
        }

        private async void LoginUser()
        {
            // Проверяем, заблокирован ли вход
            if (_lockTime > DateTime.Now)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "Доступ временно заблокирован",
                    $"Попробуйте снова через {(int)(_lockTime - DateTime.Now).TotalSeconds} секунд.",
                    ButtonEnum.Ok
                ).ShowAsync();
                return;
            }

            // Если CAPTCHA требуется, но введена неверно – обновляем CAPTCHA и выходим
            if (IsCaptchaRequired && CaptchaInput != CaptchaText)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка CAPTCHA",
                    "Введенный код не совпадает с изображением. Попробуйте еще раз.",
                ButtonEnum.Ok
                ).ShowAsync();

                GenerateCaptcha(); // Обновляем CAPTCHA
                return;
            }

            using var db = new _43pKasatkina2upContext(); // Контекст базы данных
            _currentUser = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == Login && u.Password == Password);

            if (_currentUser != null)
            {
                // Если авторизация успешна
                OpenUserWindow(_currentUser);
                _failedAttempts = 0; // Сбрасываем счетчик неудачных попыток
                CaptchaInput = string.Empty; // Очищаем поле CAPTCHA
                IsCaptchaRequired = false; // Скрываем CAPTCHA
            }
            else
            {
                // Увеличиваем счетчик неудачных попыток
                _failedAttempts++;

                // Первая неудачная попытка - показываем CAPTCHA
                if (_failedAttempts == 1)
                {
                    IsCaptchaRequired = true; // Показываем CAPTCHA
                    GenerateCaptcha();
                    await MessageBoxManager.GetMessageBoxStandard(
                        "Требуется проверка",
                        "Неверный логин или пароль. Введите CAPTCHA.",
                        ButtonEnum.Ok
                    ).ShowAsync();
                }
                // Вторая неудачная попытка (с CAPTCHA)
                else if (_failedAttempts == 2)
                {
                    // Блокируем кнопку входа на 10 секунд
                    _lockTime = DateTime.Now.AddSeconds(10);
                    await MessageBoxManager.GetMessageBoxStandard(
                        "Доступ заблокирован",
                        "Неверная CAPTCHA. Вход заблокирован на 10 секунд.",
                        ButtonEnum.Ok
                    ).ShowAsync();

                    // Ожидаем 10 секунд перед повторной попыткой
                    await Task.Delay(10000);

                    // После 10 секунд разблокируем кнопку входа
                    _lockTime = DateTime.Now; // Сбрасываем время блокировки
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