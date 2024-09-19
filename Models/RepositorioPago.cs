using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace inmobiliariaAST.Models
{
    public class RepositorioPago
    {
        private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

        // metodo para obtener todos los pagos
        public List<Pago> GetPagos(bool? estado = null)
        {
            List<Pago> pagos = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                    string query = @"
                        SELECT p.ID_pago, p.Numero_pago, p.Fecha_pago, p.Importe, p.Concepto, p.Estado, p.ID_contrato,
                            CONCAT(i.Nombre, ' ', i.Apellido) AS InquilinoNombreCompleto, inm.Direccion AS InmuebleDireccion
                        FROM Pago p
                        JOIN Contrato c ON p.ID_contrato = c.ID_contrato
                        JOIN Inquilino i ON c.ID_inquilino = i.ID_inquilino
                        JOIN Inmueble inm ON c.ID_inmueble = inm.ID_inmueble
                        WHERE (@estado IS NULL OR p.Estado = @estado)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@estado", estado.HasValue ? (object)estado.Value : DBNull.Value);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                ID_pago = reader.GetInt32("ID_pago"),
                                Numero_pago = reader.GetInt32("Numero_pago"),
                                Fecha_pago = reader.GetDateTime("Fecha_pago"),
                                Importe = reader.GetDecimal("Importe"),
                                Concepto = reader.GetString("Concepto"),
                                Estado = reader.GetBoolean("Estado"),
                                ID_contrato = reader.GetInt32("ID_contrato"),
                                InquilinoNombreCompleto = reader.GetString("InquilinoNombreCompleto"),
                                InmuebleDireccion = reader.GetString("InmuebleDireccion")
                            });
                        }
                    }
                }
            }
            return pagos;
        }


        // metodo para obtener un pago por su ID
        public Pago? Get(int id)
        {
            Pago? pago = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                        SELECT p.ID_pago, p.Numero_pago, p.Fecha_pago, p.Importe, p.Concepto, p.Estado, p.ID_contrato,
                            CONCAT(i.Nombre, ' ', i.Apellido) AS InquilinoNombreCompleto, inm.Direccion AS InmuebleDireccion
                        FROM Pago p
                        JOIN Contrato c ON p.ID_contrato = c.ID_contrato
                        JOIN Inquilino i ON c.ID_inquilino = i.ID_inquilino
                        JOIN Inmueble inm ON c.ID_inmueble = inm.ID_inmueble
                            WHERE ID_pago = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                ID_pago = reader.GetInt32("ID_pago"),
                                Numero_pago = reader.GetInt32("Numero_pago"),
                                Fecha_pago = reader.GetDateTime("Fecha_pago"),
                                Importe = reader.GetDecimal("Importe"),
                                Concepto = reader.GetString("Concepto"),
                                Estado = reader.GetBoolean("Estado"),
                                ID_contrato = reader.GetInt32("ID_contrato"),
                                InquilinoNombreCompleto = reader.GetString("InquilinoNombreCompleto"),
                                InmuebleDireccion = reader.GetString("InmuebleDireccion")
                            };
                        }
                    }
                }
            }
            return pago;
        }

        // metodo para agregar un nuevo pago
        public int Alta(Pago pago)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO Pago (ID_contrato, Numero_pago, Fecha_pago, Importe, Concepto, Estado) 
                                 VALUES (@idContrato, @numeroPago, @fechaPago, @importe, @concepto, @estado)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idContrato", pago.ID_contrato);
                    command.Parameters.AddWithValue("@numeroPago", pago.Numero_pago);
                    command.Parameters.AddWithValue("@fechaPago", pago.Fecha_pago);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@estado", pago.Estado);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // metodo para editar solo el concepto de un pago
        public int EditarPago(Pago pago)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"UPDATE Pago SET Concepto = @concepto WHERE ID_pago = @idPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@idPago", pago.ID_pago);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        //metodo para obtener proximo numero de pago
        public int GetProximoNumPago(int idContrato)
        {
            int proxNumPago = 1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT IFNULL(MAX(Numero_pago), 0) FROM Pago WHERE ID_contrato = @ID_Contrato";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Contrato", idContrato);

                    //ejecutar la consulta
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        proxNumPago = Convert.ToInt32(result) + 1;
                    }
                }
            }
            return proxNumPago;
        }

        // metodo para anular un pago (eliminación lógica)
        public int AnularPago(int idPago)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"UPDATE Pago SET Estado = FALSE WHERE ID_pago = @idPago";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idPago", idPago);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }
    }
}
