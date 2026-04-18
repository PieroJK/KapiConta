    using Inmobiliaria_KapiConta.Models;

public static class AutomatizacionMapper
{
    public static AutomatizacionDetalleItem ToItem(CuentaAutomatizacionDetalle x)
    {
        return new AutomatizacionDetalleItem
        {
            IdDetalle = x.IdDetalle,
            IdCuentaRelacionada = x.IdCuentaRelacionada,
            TipoMovimiento = x.TipoMovimiento,
            Porcentaje = x.Porcentaje,
            CuentaCodigo = x.CuentaRelacionada?.Codigo,
            CuentaDescripcion = x.CuentaRelacionada?.Descripcion
        };
    }

    public static CuentaAutomatizacionDetalle ToEntity(AutomatizacionDetalleItem x)
    {
        return new CuentaAutomatizacionDetalle
        {
            IdDetalle = x.IdDetalle ?? 0,
            IdCuentaRelacionada = x.IdCuentaRelacionada,
            TipoMovimiento = x.TipoMovimiento,
            Porcentaje = x.Porcentaje
        };
    }
}