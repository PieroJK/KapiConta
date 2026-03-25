using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.Helpers
{
    public static class Session
    {
        public static Usuario CurrentUser { get; set; }
        public static Empresa CurrentEmpresa { get; set; }
        public static Periodo CurrentPeriodo { get; set; }
        

        public static bool IsLogged => CurrentUser != null;

        public static void Logout()
        {
            CurrentUser = null;
            CurrentEmpresa = null;
            CurrentPeriodo = null;
        }
    }
}