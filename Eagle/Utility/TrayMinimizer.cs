﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;


namespace Eagle.Utility
{
    public class TrayMinimizer
    {
        /// <summary>
        /// Enables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void EnableMinimizeToTray(Window window)
        {
            if (MinimizeInstances.ContainsKey(window))
            {
                //Console.WriteLine(string.Format("Minimization already enabled for '{0}'", window.Title));
            }
            else
            {
                var instance = new MinimizeToTrayInstance(window);
                instance.Enable();
                MinimizeInstances.Add(window, instance);
            }
        }
        /// <summary>
        /// Disables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void DisableMinimizeToTray(Window window)
        {
            if (!MinimizeInstances.ContainsKey(window))
            {
                //Console.WriteLine("Minimization not enabled for '{0}'", window.Title);
            }
            else
            {
                var instance = MinimizeInstances[window];
                instance.Disable();
                MinimizeInstances.Remove(window);
            }
        }

        private static Dictionary<Window, MinimizeToTrayInstance> _minimizeInstances;
        /// <summary>
        /// Gets or sets the windows for which tray minimization is currently enabled.
        /// </summary>
        /// <value>The windows for which tray minimization is currently enabled.</value>
        private static Dictionary<Window, MinimizeToTrayInstance> MinimizeInstances
        {
            get
            {
                if (_minimizeInstances == null)
                {
                    _minimizeInstances = new Dictionary<Window, MinimizeToTrayInstance>();
                }
                return _minimizeInstances;
            }
            set { _minimizeInstances = value; }
        }

        /// <summary>
        /// Allows minimization of a window to the tray.
        /// </summary>
        private class MinimizeToTrayInstance
        {
            private Window _window;
            private NotifyIcon _notifyIcon;
            private ContextMenu _menu;

            private static bool _settingsOpen = false;

            /// <summary>
            /// Initializes a new instance of the MinimizeToTrayInstance class.
            /// </summary>
            /// <param name="window">Window instance to attach to.</param>
            public MinimizeToTrayInstance(Window window)
            {
                if (window == null)
                    throw new ArgumentException("window parameter is null.");
                _window = window;
            }

            /// <summary>
            /// Enables minimization for this Window.
            /// </summary>
            public void Enable()
            {
                _window.StateChanged += HandleStateChanged;
            }

            /// <summary>
            /// Disables minimization for this Window.
            /// </summary>
            public void Disable()
            {
                _window.StateChanged -= HandleStateChanged;
            }

            /// <summary>
            /// Handles the Window's StateChanged event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            private void HandleStateChanged(object sender, EventArgs e)
            {
                if (_notifyIcon == null)
                {
                    _notifyIcon = new NotifyIcon();
                    _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);

                    _menu = new ContextMenu();

                    var menuItem = new MenuItem { Text = "E&xit" };
                    menuItem.Click += OnExit;
                    _menu.MenuItems.Add(0, menuItem);

                    var menuItem1 = new MenuItem { Text = "Settings" };
                    menuItem1.Click += OnShowSettingsWindow;
                    _menu.MenuItems.Add(0, menuItem1);
                    
                    _notifyIcon.ContextMenu = _menu;
                }

                _notifyIcon.Text = _window.Title;

                // Show/hide Window and NotifyIcon
                var minimized = (_window.WindowState == WindowState.Minimized);
                _window.ShowInTaskbar = !minimized;
                _notifyIcon.Visible = minimized;
                //if (minimized && !Settings.Default.MinimizeBalloonShown)
                //{
                //    // If this is the first time minimizing to the tray, show the user what happened
                _notifyIcon.ShowBalloonTip(1000, _window.Title, "Remember that I'm always running down here!", ToolTipIcon.None);
                //    Settings.Default.MinimizeBalloonShown = true;
                //    Settings.Default.Save();
                //}
            }

            private void OnShowSettingsWindow(object sender, EventArgs eventArgs)
            {
                if (!_settingsOpen)
                {
                    _settingsOpen = true;
                    var s = new Settings();

                    //When done changing settings, restart the observer
                    s.Closed += (o, args) =>
                        {
                            _settingsOpen = false;
                            ((MainWindow) _window).ResetObserver();
                        };

                    s.ShowDialog();
                }
            }

            private void OnExit(object sender, EventArgs eventArgs)
            {
                _window.Close();
            }

            /// <summary>
            /// Restores a window when the notify icon or its balloon are clicked.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            private void HandleNotifyIconOrBalloonClicked(object sender, EventArgs e)
            {
                _window.WindowState = WindowState.Normal;
            }
        }

    }
}