﻿using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserController _userController;

        public LoginWindow()
        {
            InitializeComponent();
            _userController = new UserController();
            ErrorImage.Visibility = Visibility.Hidden;
            ErrorTextBlock.Visibility = Visibility.Hidden;
        }

        private async void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            // Validation 

            if (string.IsNullOrWhiteSpace(login))
            {
                ShowErrorMessage("Введіть логін!");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Введіть пароль!");
                return;
            }


            // hash password
            // string hashedPassword = PasswordHelper.HashPassword(password);
            // LoginTextBox.Text = hashedPassword;

            // Auth
            User objUser = await _userController.GetUserAsync(login);

            if (objUser == null)
            {
                ShowErrorMessage($"Користувача з логіном {login} не існує.");
                ClearFields();
                return;
            }

            if (!PasswordHelper.ValidatePassword(password, objUser.Password))
            {
                ShowErrorMessage("Логін або пароль недійсні!");
                ClearFields();
                return;
            }

            WorkWindow workWindow = new WorkWindow(objUser);
            workWindow.Show();
            this.Close();
        }

        private void ClearFields()
        {
            LoginTextBox.Text = string.Empty;
            PasswordTextBox.Password = string.Empty;
        }

        private void ShowErrorMessage(string errorMessage)
        {
            ErrorImage.Visibility = Visibility.Visible;
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = errorMessage;
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            LoginTextBox.Text += button.Content;
            LoginTextBox.CaretIndex = LoginTextBox.Text.Length;
            LoginTextBox.Focus();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            string text = LoginTextBox.Text;
            text = text.Remove(text.Length - 1);
            LoginTextBox.Text = text;
            LoginTextBox.CaretIndex = LoginTextBox.Text.Length;
            LoginTextBox.Focus();
        }
    }
}
