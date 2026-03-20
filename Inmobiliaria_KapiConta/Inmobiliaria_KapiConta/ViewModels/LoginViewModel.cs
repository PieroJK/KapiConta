using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthService _authService = new AuthService();

    private string _usuario;
    public string Usuario
    {
        get => _usuario;
        set { _usuario = value; OnPropertyChanged(); }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(Login);
    }

    private void Login()
    {
        bool isValid = _authService.Login(Usuario, Password);

        if (isValid)
        {
            MessageBox.Show("Login correcto ?");
            // ?? aquí luego abrimos MainWindow
        }
        else
        {
            MessageBox.Show("Usuario o contraseña incorrectos ?");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}