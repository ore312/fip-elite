﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Elite
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        public ICommand ToggleWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        if (Application.Current.MainWindow == null || Application.Current.MainWindow.Visibility == Visibility.Collapsed)
                        {
                            var window = Application.Current.MainWindow = new MainWindow();
                            window.ShowActivated = false;
                            
                            var im = (System.Windows.Controls.Image)window.FindName("im");
                            if (im != null && App.WindowWidth > 0 && App.WindowHeight > 0)
                            {
                                im.Width = App.WindowWidth;
                                im.Height = App.WindowHeight;
                            }

                            Application.Current.MainWindow.Show();
                            App.FipHandler.RefreshDevicePages();

                            Properties.Settings.Default.Visible = true;
                        }
                        else
                        {
                            Application.Current.MainWindow.ShowActivated = false;
                            if (Application.Current.MainWindow.Visibility == Visibility.Visible)
                            {
                                Application.Current.MainWindow.Hide();

                                Properties.Settings.Default.Visible = false;
                            }
                            else
                            {
                                Application.Current.MainWindow.Show();
                                App.FipHandler.RefreshDevicePages();


                                Properties.Settings.Default.Visible = true;
                            }
                        }

                        Properties.Settings.Default.Save();
                    },
                    CanExecuteFunc = () => App.JsonTask != null
                };
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () =>
                    {
                        if (App.JsonTask?.Status.Equals(TaskStatus.Running) == true)
                        {
                            MessageBox.Show("Waiting for background tasks to complete...","fip-elite", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        }
                        else
                        {
                            App.IsShuttingDown = true;
                            Application.Current.Shutdown();
                        }
                    }
                };
            }
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null  || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
