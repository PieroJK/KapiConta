namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class AsientoQueries
    {
        public static string Listar = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.estado = true
            ORDER BY a.fecha DESC;";

        public static string ListarPorEmpresaYPeriodo = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.estado      = true
              AND a.id_empresa  = @idEmpresa
              AND a.id_periodo  = @idPeriodo
            ORDER BY a.fecha DESC;";

        public static string ObtenerPorId = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.id_asiento = @idAsiento;";

        public static string Insertar = @"
            INSERT INTO asiento
            (id_empresa, id_mes, id_sub_diario, referencia, id_libro,
             fecha, moneda, id_tipo_cambio, fecha_ven, id_usuario, id_periodo)
            VALUES
            (@idEmpresa, @idMes, @idSubDiario, @referencia, @idLibro,
             @fecha, @moneda, @idTipoCambio, @fechaVen, @idUsuario, @idPeriodo)
            RETURNING *;";

        public static string Actualizar = @"
            UPDATE asiento
            SET id_mes          = @idMes,
                id_sub_diario   = @idSubDiario,
                referencia      = @referencia,
                id_libro        = @idLibro,
                fecha           = @fecha,
                moneda          = @moneda,
                id_tipo_cambio  = @idTipoCambio,
                fecha_ven       = @fechaVen,
                fecha_modificacion = current_timestamp
            WHERE id_asiento  = @idAsiento
              AND id_empresa  = @idEmpresa;";

        public static string EliminarLogico = @"
            UPDATE asiento
            SET estado = false
            WHERE id_asiento = @idAsiento
              AND id_empresa = @idEmpresa;";
    }
}
