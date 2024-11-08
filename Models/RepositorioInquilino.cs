 using MySql.Data.MySqlClient;

namespace inmobiliariaAST.Models
{
    public class RepositorioInquilino : IRepositorioInquilino
    {
        private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

        public List<Inquilino> GetInquilinos()
        {
            List<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = "SELECT ID_inquilino, DNI, Nombre, Apellido, Telefono, Email, Direccion, Estado FROM inquilino WHERE Estado = true";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            ID_inquilino = reader.GetInt32(0),
                            DNI = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Direccion = reader.GetString(6),
                            Estado = reader.GetBoolean(7)
                        });
                    }
                    connection.Close();
                }
            }
            return inquilinos;
        }

        public Inquilino? Get(int id)
        {
            Inquilino? res = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = "SELECT ID_inquilino, DNI, Nombre, Apellido, Telefono, Email, Direccion, Estado FROM inquilino WHERE ID_inquilino = @id AND Estado = true";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Inquilino
                        {
                            ID_inquilino = reader.GetInt32(0),
                            DNI = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Direccion = reader.GetString(6),
                            Estado = reader.GetBoolean(7)
                        };
                    }
                    connection.Close();
                }
                return res;
            }
        }

        public int Alta(Inquilino inquilino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = @"INSERT INTO inquilino 
                        (DNI, Nombre, Apellido, Telefono, Email, Direccion, Estado)
                        VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion, @estado);
                        SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.DNI);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
                    command.Parameters.AddWithValue("@estado", inquilino.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificar(Inquilino inquilino)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = @"UPDATE inquilino SET 
                        DNI = @dni,
                        Nombre = @nombre, 
                        Apellido = @apellido,
                        Telefono = @telefono, 
                        Email = @email,
                        Direccion = @direccion,
                        Estado = @estado
                        WHERE Id_inquilino = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.ID_inquilino);
                    command.Parameters.AddWithValue("@dni", inquilino.DNI);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
                    command.Parameters.AddWithValue("@estado", inquilino.Estado);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = @"UPDATE inquilino SET Estado = false WHERE ID_inquilino = @id;
                            UPDATE contrato SET Estado = false WHERE ID_inquilino = @id;
                            ";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}


// UPDATE pago SET Estado = false WHERE ID_contrato IN (SELECT ID_contrato FROM contrato WHERE ID_inquilino = @id)";