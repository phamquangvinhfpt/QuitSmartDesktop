using QuitSmartApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuitSmartApp.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmNewPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isLoading = false;

        // Navigation action
        public Action? NavigateBack { get; set; }

        public ChangePasswordViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            // Initialize commands
            ChangePasswordCommand = new RelayCommand(async () => await ChangePasswordAsync(), CanChangePassword);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Properties
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                SetProperty(ref _currentPassword, value);
                CommandManager.InvalidateRequerySuggested();
                ClearMessages();
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                SetProperty(ref _newPassword, value);
                CommandManager.InvalidateRequerySuggested();
                ClearMessages();
            }
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                SetProperty(ref _confirmNewPassword, value);
                CommandManager.InvalidateRequerySuggested();
                ClearMessages();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                SetProperty(ref _successMessage, value);
                OnPropertyChanged(nameof(HasSuccessMessage));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Commands
        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelCommand { get; }

        // Methods
        private bool CanChangePassword()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(CurrentPassword) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmNewPassword);
        }

        private async Task ChangePasswordAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                // Validate form
                if (!ValidateForm())
                    return;

                // Change password
                bool success = await _authenticationService.ChangePasswordAsync(CurrentPassword, NewPassword);

                if (success)
                {
                    SuccessMessage = "Mật khẩu đã được thay đổi thành công!";

                    // Clear form
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;

                    // Auto close after success
                    await Task.Delay(2000);
                    NavigateBack?.Invoke();
                }
                else
                {
                    ErrorMessage = "Mật khẩu hiện tại không đúng hoặc có lỗi xảy ra.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Cancel()
        {
            NavigateBack?.Invoke();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu mới.";
                return false;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return false;
            }

            if (NewPassword == CurrentPassword)
            {
                ErrorMessage = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                return false;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "Xác nhận mật khẩu mới không khớp.";
                return false;
            }

            return true;
        }

        private void ClearMessages()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = string.Empty;
            if (!string.IsNullOrEmpty(SuccessMessage))
                SuccessMessage = string.Empty;
        }
    }
}