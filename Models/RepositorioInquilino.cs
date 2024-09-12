 using MySql.Data.MySqlClient;

namespace inmobiliariaAST.Models
{
    public class RepositorioInquilino
    {
        private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

        public List<Inquilino> GetInquilinos()
        {
            List<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                var query = "SELECT Id_inquilino, DNI, Nombre, Apellido, Telefono, Email, Direccion FROM inquilino";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            Id_inquilino = reader.GetInt32(0),
                            DNI = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Direccion = reader.GetString(6)
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
                var query = "SELECT Id_inquilino, DNI, Nombre, Apellido, Telefono, Email, Direccion FROM inquilino WHERE Id_inquilino = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Inquilino
                        {
                            Id_inquilino = reader.GetInt32(0),
                            DNI = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Direccion = reader.GetString(6)
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
                        (DNI, Nombre, Apellido, Telefono, Email, Direccion)
                        VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion);
                        SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.DNI);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
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
                        Direccion = @direccion
                        WHERE Id_inquilino = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.Id_inquilino);
                    command.Parameters.AddWithValue("@dni", inquilino.DNI);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
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
                var query = "DELETE FROM inquilino WHERE Id_inquilino = @id";
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
