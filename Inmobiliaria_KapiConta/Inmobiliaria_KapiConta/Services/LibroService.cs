using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Inmobiliaria_KapiConta.Services
{
    public class LibroService
    {
        // =========================
        // LISTAR
        // =========================
        public List<Libro> Listar()
        {
            var lista = new List<Libro>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(LibroQueries.Listar, cn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(LibroMapper.Map(reader));
            }

            return lista;
        }

        // =========================
        // OBTENER POR ID
        // =========================
        public Libro ObtenerPorId(int idLibro)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(LibroQueries.ObtenerPorId, cn);
            cmd.Parameters.AddWithValue("@idLibro", idLibro);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return LibroMapper.Map(reader);
            }

            return null; // o lanzar excepción si prefieres
        }
    }
}
