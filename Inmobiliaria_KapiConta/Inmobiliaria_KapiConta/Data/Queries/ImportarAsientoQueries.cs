namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class ImportarAsientoQueries
    {
        public static string CargarCuentas = @"
            SELECT id_plan_cuenta, codigo 
            FROM plan_cuenta 
            WHERE id_empresa = @id AND estado = true;";

        public static string CargarTerceros = @"
            SELECT id_tercero, documento 
            FROM tercero 
            WHERE estado = true;";

        public static string CargarTiposFacturacion = @"
            SELECT id_tipo_facturacion, cod 
            FROM tipo_facturacion 
            WHERE estado = true;";

        public static string CargarTiposDocTercero = @"
            SELECT id_tercero_tipo_documento, cod 
            FROM tercero_tipo_documento;";

        public static string CargarSubDiarios = @"
            SELECT id_sub_diario, diario 
            FROM sub_diario;";

        public static string CargarLibros = @"
            SELECT id_libro, cod 
            FROM libro;";

        public static string CargarCostos = @"
            SELECT id_costo, descripcion 
            FROM costo 
            WHERE id_empresa = @id AND estado = true;";

        public static string CargarMeses = @"
            SELECT id_mes, mes 
            FROM mes;";

        public static string CargarTiposOperacion = @"
            SELECT id_tipo_operacion, codigo 
            FROM tipo_operacion_asiento 
            WHERE estado = true;";

        public static string InsertarTercero = @"
            INSERT INTO tercero(documento, razon_social, id_tercero_tipo_documento, estado)
            VALUES(@doc, @razon, @tipo, true)
            RETURNING id_tercero;";

        public static string InsertarCosto = @"
            INSERT INTO costo (id_empresa, codigo, descripcion, estado)
            VALUES (@idEmpresa, @codigo, @descripcion, true)
            RETURNING id_costo;";

        public static string ActualizarCodigoCosto = @"
            UPDATE costo SET codigo = @codigo
            WHERE id_costo = @id;";

        public static string BuscarTipoCambio = @"
            SELECT id_tipo_cambio FROM tipo_cambio
            WHERE fecha = @fecha AND estado = true LIMIT 1;";

        public static string InsertarTipoCambio = @"
            INSERT INTO tipo_cambio(fecha, moneda, compra, venta, estado)
            VALUES(@fecha, 'USD', @tc, @tc, true)
            RETURNING id_tipo_cambio;";

        public static string InsertarCabecera = @"
            INSERT INTO asiento
            (id_empresa, id_periodo, id_mes, id_sub_diario, id_libro,
             referencia, fecha, fecha_ven, moneda, id_tipo_cambio, id_usuario)
            VALUES
            (@idEmpresa, @idPeriodo, @idMes, @idSubDiario, @idLibro,
             @referencia, @fecha, @fechaVen, @moneda, @idTipoCambio, @idUsuario)
            RETURNING id_asiento;";

        public static string InsertarDetalle = @"
            INSERT INTO asiento_detalle
            (id_asiento, id_plan_cuenta, moneda, debe, haber,
             id_tipo_facturacion, serie_comprobante, id_tercero,
             glosa, id_relacion, id_costo, id_tipo_operacion)
            VALUES
            (@idAsiento, @idCuenta, @moneda, @debe, @haber,
             @idTipoFacturacion, @serieDoc, @idTercero,
             @glosa, @idRelacion, @idCosto, @idTipoOperacion);";

        public static string DesactivarAsientos = @"
            UPDATE asiento
            SET estado = false
            WHERE id_empresa    = @idEmpresa
              AND id_periodo    = @idPeriodo
              AND id_mes        = @idMes
              AND id_sub_diario = @idSubDiario;";

        public static string ObtenerUltimaReferencia = @"
            SELECT referencia FROM asiento
            WHERE id_empresa    = @idEmpresa
              AND id_periodo    = @idPeriodo
              AND id_mes        = @idMes
              AND id_sub_diario = @idSubDiario
              AND estado = true
            ORDER BY referencia DESC
            LIMIT 1;";
    }
}