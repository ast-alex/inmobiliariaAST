using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace inmobiliariaAST.Models
{
    public class RepositorioContrato
    {
        private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

        public List<Contrato> GetContratos()
        {
            List<Contrato> contratos = new List<Contrato>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"SELECT c.ID_contrato, c.Fecha_Inicio, c.Fecha_Fin, c.Monto_Mensual, c.Estado,
                                        c.Fecha_Terminacion_Anticipada, c.Multa, 
                                        i.Direccion AS InmuebleDireccion, 
                                        CONCAT(iq.Nombre, ' ', iq.Apellido) AS InquilinoNombreCompleto
                                FROM Contrato c
                                JOIN Inmueble i ON c.ID_inmueble = i.ID_inmueble
                                JOIN Inquilino iq ON c.ID_inquilino = iq.ID_inquilino";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contrato contrato = new Contrato
                            {
                                ID_contrato = reader.GetInt32("ID_contrato"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin = reader.GetDateTime("Fecha_Fin"),
                                Monto_Mensual = reader.GetDecimal("Monto_Mensual"),
                                Estado = reader.GetBoolean("Estado"),
                                Fecha_Terminacion_Anticipada = reader.IsDBNull(reader.GetOrdinal("Fecha_Terminacion_Anticipada"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("Fecha_Terminacion_Anticipada"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa"))
                                    ? (decimal?)null
                                    : reader.GetDecimal("Multa"),

                                // reemplazo ID_inmueble y ID_inquilino 
                                InmuebleDireccion = reader.GetString("InmuebleDireccion"),
                                InquilinoNombreCompleto = reader.GetString("InquilinoNombreCompleto")
                            };

                            contratos.Add(contrato);
                        }
                    }
                }
            }

            return contratos;
        }


        public Contrato? Get(int id)
        {
            Contrato? contrato = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                 string query = @"SELECT c.ID_contrato, c.ID_inmueble, c.ID_inquilino, c.Fecha_Inicio, c.Fecha_Fin, c.Monto_Mensual, c.Estado,
                                        c.Fecha_Terminacion_Anticipada, c.Multa, 
                                        i.Direccion AS InmuebleDireccion, 
                                        CONCAT(iq.Nombre, ' ', iq.Apellido) AS InquilinoNombreCompleto
                                FROM Contrato c
                                JOIN Inmueble i ON c.ID_inmueble = i.ID_inmueble
                                JOIN Inquilino iq ON c.ID_inquilino = iq.ID_inquilino 
                                WHERE c.ID_contrato = @id";
                                

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                ID_contrato = reader.GetInt32("ID_contrato"),
                                ID_inmueble = reader.GetInt32("ID_inmueble"),
                                ID_inquilino = reader.GetInt32("ID_inquilino"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin = reader.GetDateTime("Fecha_Fin"),
                                Monto_Mensual = reader.GetDecimal("Monto_Mensual"),
                                Estado = reader.GetBoolean("Estado"),

                                // Verificación de nulos para los campos opcionales
                                Fecha_Terminacion_Anticipada = reader.IsDBNull(reader.GetOrdinal("Fecha_Terminacion_Anticipada"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("Fecha_Terminacion_Anticipada"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa"))
                                    ? (decimal?)null
                                    : reader.GetDecimal("Multa"),
                                
                                  // reemplazo ID_inmueble y ID_inquilino 
                                InmuebleDireccion = reader.GetString("InmuebleDireccion"),
                                InquilinoNombreCompleto = reader.GetString("InquilinoNombreCompleto")
                            };
                        }
                    }
                }
            }
            return contrato;
        }

        //alta contrato
        public void Alta(Contrato contrato)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Contrato (ID_inmueble, ID_inquilino, Fecha_Inicio, Fecha_Fin, 
                                                        Monto_Mensual, Estado, Fecha_Terminacion_Anticipada, Multa) 
                                VALUES (@ID_inmueble, @ID_inquilino, @Fecha_Inicio, @Fecha_Fin, 
                                        @Monto_Mensual, @Estado, @Fecha_Terminacion_Anticipada, @Multa)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignamos los parámetros con los valores del objeto contrato
                    command.Parameters.AddWithValue("@ID_inmueble", contrato.ID_inmueble);
                    command.Parameters.AddWithValue("@ID_inquilino", contrato.ID_inquilino);
                    command.Parameters.AddWithValue("@Fecha_Inicio", contrato.Fecha_Inicio);
                    command.Parameters.AddWithValue("@Fecha_Fin", contrato.Fecha_Fin);
                    command.Parameters.AddWithValue("@Monto_Mensual", contrato.Monto_Mensual);
                    command.Parameters.AddWithValue("@Estado", contrato.Estado);

                    // Manejo de valores nulos
                    if (contrato.Fecha_Terminacion_Anticipada.HasValue)
                    {
                        command.Parameters.AddWithValue("@Fecha_Terminacion_Anticipada", contrato.Fecha_Terminacion_Anticipada);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Fecha_Terminacion_Anticipada", DBNull.Value);
                    }

                    if (contrato.Multa.HasValue)
                    {
                        command.Parameters.AddWithValue("@Multa", contrato.Multa);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Multa", DBNull.Value);
                    }

                    // Ejecutamos la consulta para insertar los datos
                    command.ExecuteNonQuery();
                }
            }
        }


        //modificar
        public void Modificar(Contrato contrato)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Contrato SET 
                                    ID_inmueble = @ID_inmueble, 
                                    ID_inquilino = @ID_inquilino, 
                                    Fecha_Inicio = @Fecha_Inicio, 
                                    Fecha_Fin = @Fecha_Fin, 
                                    Monto_Mensual = @Monto_Mensual, 
                                    Estado = @Estado, 
                                    Fecha_Terminacion_Anticipada = @Fecha_Terminacion_Anticipada, 
                                    Multa = @Multa
                                WHERE ID_contrato = @ID_contrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asignamos los parámetros con los valores del objeto contrato
                    command.Parameters.AddWithValue("@ID_contrato", contrato.ID_contrato);
                    command.Parameters.AddWithValue("@ID_inmueble", contrato.ID_inmueble);
                    command.Parameters.AddWithValue("@ID_inquilino", contrato.ID_inquilino);
                    command.Parameters.AddWithValue("@Fecha_Inicio", contrato.Fecha_Inicio);
                    command.Parameters.AddWithValue("@Fecha_Fin", contrato.Fecha_Fin);
                    command.Parameters.AddWithValue("@Monto_Mensual", contrato.Monto_Mensual);
                    command.Parameters.AddWithValue("@Estado", contrato.Estado);

                    // Manejo de valores nulos
                    if (contrato.Fecha_Terminacion_Anticipada.HasValue)
                    {
                        command.Parameters.AddWithValue("@Fecha_Terminacion_Anticipada", contrato.Fecha_Terminacion_Anticipada);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Fecha_Terminacion_Anticipada", DBNull.Value);
                    }

                    if (contrato.Multa.HasValue)
                    {
                        command.Parameters.AddWithValue("@Multa", contrato.Multa);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Multa", DBNull.Value);
                    }

                    // Ejecutamos la consulta para actualizar los datos
                    command.ExecuteNonQuery();
                }
            }
        }


        //eliminar logico
        public void Eliminar(int id){
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"UPDATE Contrato SET Estado = 0 WHERE ID_contrato = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
