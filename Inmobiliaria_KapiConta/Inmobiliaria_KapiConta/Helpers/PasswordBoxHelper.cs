using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inmobiliaria_KapiConta.Helpers
{
    public class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty,OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox == null) return;

            // Remover evento anterior para evitar duplicados
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            // Cambiar contraseña solo si es diferente
            var newPassword = (string)e.NewValue;
            if (passwordBox.Password != newPassword)
            {
                passwordBox.Password = newPassword;
            }

            // Suscribir al evento
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            // Actualizar la propiedad adjunta cuando la contraseña cambia
            SetBoundPassword(passwordBox, passwordBox.Password);
        }
    }
}
