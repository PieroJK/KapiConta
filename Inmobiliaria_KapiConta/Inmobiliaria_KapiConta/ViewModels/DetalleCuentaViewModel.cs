// ViewModels/DetalleCuentaViewModel.cs
using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Helpers;
using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class DetalleCuentaViewModel : INotifyPropertyChanged
    {
        // ─── INotifyPropertyChanged ───────────────────────────────────
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // ─── Parámetros recibidos ─────────────────────────────────────
        private readonly string _cuenta;
        private readonly string _mes;

        // ─── Comandos ─────────────────────────────────────────────────
        public ICommand CerrarCommand { get; private set; }
        public ICommand ExcelCommand { get; private set; }
        public Action Cerrar { get; set; }  // asignado desde la View al abrir

        // ─── SubDiarios para el ComboBox ──────────────────────────────
        public ObservableCollection<SubDiarioItem> SubDiarios { get; } = new()
        {
            new SubDiarioItem("",  "TODOS"),
            new SubDiarioItem("V", "VENTAS"),
            new SubDiarioItem("C", "COMPRAS"),
            new SubDiarioItem("I", "INGRESO"),
            new SubDiarioItem("E", "EGRESO"),
            new SubDiarioItem("P", "PLANILLA"),
            new SubDiarioItem("D", "DIARIO"),
            new SubDiarioItem("G", "OTROS")
        };

        private SubDiarioItem _subDiarioSeleccionado;
        public SubDiarioItem SubDiarioSeleccionado
        {
            get => _subDiarioSeleccionado;
            set
            {
                _subDiarioSeleccionado = value;
                OnPropertyChanged();
                CargarDetalle();  // recarga al cambiar el filtro, igual que SelectionChanged
            }
        }

        // ─── Datos del DataGrid ───────────────────────────────────────
        // Usamos DataView igual que el original (DataTable → DefaultView)
        // para no romper el binding de columnas que ya tienes en el XAML
        private DataView _detalle;
        public DataView Detalle
        {
            get => _detalle;
            set { _detalle = value; OnPropertyChanged(); }
        }

        // Guardamos el DataTable para el export (igual que el original)
        private DataTable _dtDatos;

        // ─── Título del header ────────────────────────────────────────
        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(); }
        }

        // ─── Constructor ──────────────────────────────────────────────
        public DetalleCuentaViewModel(string cuenta, string mes)
        {
            _cuenta = cuenta;
            _mes = mes;
            Titulo = $"Cuenta: {cuenta} | Mes: {mes}";

            CerrarCommand = new RelayCommand(() => Cerrar?.Invoke());
            ExcelCommand = new RelayCommand(Exportar, () => _dtDatos?.Rows.Count > 0);

            // Selecciona "TODOS" por defecto — esto dispara CargarDetalle()
            // a través del setter de SubDiarioSeleccionado
            _subDiarioSeleccionado = SubDiarios[0];  // sin disparar CargarDetalle aún
            CargarDetalle();                          // carga inicial explícita
        }

        // ─── Carga de datos ───────────────────────────────────────────
        private void CargarDetalle()
        {
            try
            {
                using var conn = DbConnectionFactory.Create();
                conn.Open();

                string filtroSubDiario = string.IsNullOrEmpty(SubDiarioSeleccionado?.Codigo)
                    ? ""
                    : "AND sd.diario = @subDiario";

                // Query idéntica al original — no se tocó la lógica SQL
                string query = $@"
                    SELECT 
                        a.referencia,
                        a.fecha,
                        pc.codigo,
                        pc.descripcion,
                        ad.debe,
                        ad.haber,
                        ad.glosa
                    FROM asiento_detalle ad
                    INNER JOIN asiento     a   ON ad.id_asiento      = a.id_asiento
                    INNER JOIN plan_cuenta pc  ON ad.id_plan_cuenta  = pc.id_plan_cuenta
                    INNER JOIN mes         m   ON a.id_mes           = m.id_mes
                    INNER JOIN sub_diario  sd  ON a.id_sub_diario    = sd.id_sub_diario
                    WHERE a.id_empresa = @empresa
                      AND pc.codigo   = @cuenta
                      AND m.mes       = @mes
                      {filtroSubDiario}
                    ORDER BY 
                        CASE LEFT(a.referencia, 1)
                            WHEN 'V' THEN 1
                            WHEN 'C' THEN 2
                            WHEN 'I' THEN 3
                            WHEN 'E' THEN 4
                            WHEN 'P' THEN 5
                            WHEN 'D' THEN 6
                            WHEN 'G' THEN 7
                            ELSE 99
                        END,
                        a.fecha;";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@empresa", Session.CurrentEmpresa.IdEmpresa);
                cmd.Parameters.AddWithValue("@cuenta", _cuenta);
                cmd.Parameters.AddWithValue("@mes", _mes);

                if (!string.IsNullOrEmpty(SubDiarioSeleccionado?.Codigo))
                    cmd.Parameters.AddWithValue("@subDiario", SubDiarioSeleccionado.Codigo);

                using var reader = cmd.ExecuteReader();
                _dtDatos = new DataTable();
                _dtDatos.Load(reader);

                Detalle = _dtDatos.DefaultView;
                ((RelayCommand)ExcelCommand).RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle: " + ex.Message);
            }
        }

        // ─── Exportar Excel ───────────────────────────────────────────
        private void Exportar()
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Leonardo");

                string tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"DetalleCuenta_{_cuenta}_{_mes}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Detalle");

                ws.Cells["A1"].LoadFromDataTable(_dtDatos, true);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                package.SaveAs(new FileInfo(tempFile));

                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Excel: " + ex.Message);
            }
        }
    }

    // ─── Mini-modelo para el ComboBox de SubDiario ────────────────────
    // Va en el mismo archivo para no crear un archivo extra por una clase tan pequeña
    public class SubDiarioItem
    {
        public string Codigo { get; }
        public string Nombre { get; }
        public SubDiarioItem(string codigo, string nombre)
        {
            Codigo = codigo;
            Nombre = nombre;
        }
    }
}
