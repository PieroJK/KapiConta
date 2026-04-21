namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class TipoCambioQueries
    {
        public static string Listar = @"
            SELECT id_tipo_cambio, fecha, moneda, compra, venta, estado
            FROM tipo_cambio
            WHERE estado = true
            ORDER BY fecha DESC;";

        public static string ObtenerPorFecha = @"
            SELECT id_tipo_cambio, fecha, moneda, compra, venta, estado
            FROM tipo_cambio
            WHERE fecha = @fecha
              AND estado = true;";

        public static string ObtenerPorId = @"
            SELECT id_tipo_cambio, fecha, moneda, compra, venta, estado
            FROM tipo_cambio
            WHERE id_tipo_cambio = @idTipoCambio;";

        public static string Insertar = @"
            INSERT INTO tipo_cambio (fecha, moneda, compra, venta, estado)
            VALUES (@fecha, @moneda, @compra, @venta, @estado)
            RETURNING *;";

        public static string Actualizar = @"
            UPDATE tipo_cambio
            SET compra = @compra,
                venta  = @venta,
                moneda = @moneda
            WHERE id_tipo_cambio = @idTipoCambio;";

        public static string EliminarLogico = @"
            UPDATE tipo_cambio
            SET estado = false
            WHERE id_tipo_cambio = @idTipoCambio;";
    }
}
