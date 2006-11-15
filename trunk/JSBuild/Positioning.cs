using System;
using Microsoft.Win32;
using System.Windows.Forms;


namespace McGiv.Win32.Forms
{


    /// <summary>
    /// Allows the easy loading and saving of control position and size.
    /// Also saves and loads the window position of forms.
    /// </summary>
    internal class Positioning
    {

        /// <summary>
        /// Saves the position and size of the given control.
        /// If control is a Form object it saves its window position.
        /// Best placed in the OnClosing method of the (parent) Form.
        /// </summary>
        /// <param name="control">The control to be saved</param>
        /// <param name="topLevel">Registry top level key.</param>
        /// <param name="subKey">The sub key that 
        ///              the data is saved to.</param>
        public static void Save(Control control,
                   RegistryHive topLevel, string subKey)
        {
            // todo validate subKey
            if(control == null)
            {
                throw new ArgumentException("The control cannot be null");
            }

            // open key
            Microsoft.Win32.RegistryKey key = Open(topLevel, subKey);

            // form state
            Form form = control as Form;
            if(form != null)
            {
                key.SetValue(form.Name + "_windowState", (int)form.WindowState);

                // if window is not in normal mode set to normal before saving
                if(form.WindowState != FormWindowState.Normal)
                {
                    // save normal windo size
                    form.WindowState = FormWindowState.Normal;
                }
            }

            // save screen res
            key.SetValue(control.Name + "_screenHeight",
                          Screen.PrimaryScreen.Bounds.Height);
            key.SetValue(control.Name + "_screenWidth",
                           Screen.PrimaryScreen.Bounds.Width);

            // control position/size
            key.SetValue(control.Name + "_top", control.Top);
            key.SetValue(control.Name + "_left", control.Left);
            key.SetValue(control.Name + "_width", control.Width);
            key.SetValue(control.Name + "_height", control.Height);

            // close key after use
            key.Close();
        }

        /// <summary>
        /// Loads the position and size of a control.
        /// If control is a form also loads the window position.
        /// Best placed in the Form's constructor.
        /// </summary>
        /// <param name="control">The control to be loaded.</param>
        /// <param name="topLevel">Registry top level key.</param>
        /// <param name="subKey">The sub key where the data is saved.</param>
        public static void Load(Control control,
                    RegistryHive topLevel, string subKey)
        {

            // todo validate subKey

            if(control == null)
            {
                throw new ArgumentException("The control cannot be null");
            }

            // open key
            Microsoft.Win32.RegistryKey key = Open(topLevel, subKey);

            // check that screen res is the same
            // if not or can't be found revert to origional.
            try
            {
                if(Screen.PrimaryScreen.Bounds.Height !=
                   (int)key.GetValue(control.Name + "_screenHeight") ||
                   Screen.PrimaryScreen.Bounds.Width !=
                   (int)key.GetValue(control.Name + "_screenWidth"))
                {

                    return;
                }
            }
            catch(NullReferenceException) { return; }

            control.SuspendLayout();

            // form state
            Form form = control as Form;
            if(form != null)
            {
                try
                {
                    form.WindowState =
                      (FormWindowState)key.GetValue(form.Name +
                      "_windowState");
                    form.StartPosition = FormStartPosition.Manual;
                }
                catch(NullReferenceException) { }
            }

            // control position/size
            try
            {
                control.Top = (int)key.GetValue(control.Name + "_top");
            }
            catch(NullReferenceException) { }
            try
            {
                control.Left = (int)key.GetValue(control.Name + "_left");
            }
            catch(NullReferenceException) { }
            try
            {
                control.Width = (int)key.GetValue(control.Name + "_width");
            }
            catch(NullReferenceException) { }
            try
            {
                control.Height = (int)key.GetValue(control.Name + "_height");
            }
            catch(NullReferenceException) { }

            control.ResumeLayout();

            // close key after use
            key.Close();

        }

        /// <summary>
        /// Opens a Registry key with write access.
        /// If the key dosn't exist it creates one.
        /// </summary>
        /// <param name="topLevel">Registry top level key.</param>
        /// <param name="subKey">Subkey to be opened.</param>
        /// <returns>Writable registry key</returns>
        private static Microsoft.Win32.RegistryKey
                Open(RegistryHive topLevel, string subKey)
        {

            switch(topLevel)
            {

                case RegistryHive.ClassesRoot:
                    {

                        return Registry.CurrentUser.CreateSubKey(subKey);
                    }
                case RegistryHive.CurrentConfig:
                    {

                        return Registry.CurrentConfig.CreateSubKey(subKey);
                    }
                case RegistryHive.CurrentUser:
                    {

                        return Registry.CurrentUser.CreateSubKey(subKey);
                    }
                case RegistryHive.DynData:
                    {

                        return Registry.DynData.CreateSubKey(subKey);
                    }
                case RegistryHive.LocalMachine:
                    {

                        return Registry.LocalMachine.CreateSubKey(subKey);
                    }
                case RegistryHive.PerformanceData:
                    {

                        return Registry.PerformanceData.CreateSubKey(subKey);
                    }
                case RegistryHive.Users:
                    {

                        return Registry.Users.CreateSubKey(subKey);
                    }
                default:
                    {

                        return null;
                    }

            }
        }

    }
}