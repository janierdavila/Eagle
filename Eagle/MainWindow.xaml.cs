using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Eagle.Utility;

namespace Eagle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EagleObserver _observer;
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Eagle Observer";

            TrayMinimizer.EnableMinimizeToTray(this);
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //Logic to remove from Alt-Tab
            //WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            //int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            //exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            //SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            this.WindowState = WindowState.Minimized;
            this.OnStateChanged(EventArgs.Empty);

            _observer = new EagleObserver(Dispatcher);
            _observer.Start();
        }

        public void ResetObserver()
        {
            _observer.Restart();
        }

        #region Window styles

        //The following code was taken from here:
        //http://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher

        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
    }
}
