using MySql.Data.MySqlClient;

namespace inmobiliariaAST.Models;

public class RepositorioPropietario
{
    private string ConnectionString = "Server=localhost;User=root;Password=;Database=inmobiliaria_e1;SslMode=none";

    public List<Propietario> GetPropietarios()
    {
        List<Propietario> propietarios = new List<Propietario>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT Id_propietario, DNI, Nombre, Apellido, Telefono, Email, Direccion FROM propietario";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    propietarios.Add(new Propietario
                    {
                        Id_propietario = reader.GetInt32(0),
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
        return propietarios;
    }

    public Propietario? Get(int id)
    {
        Propietario? res = null;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT Id_propietario, DNI, Nombre, Apellido, Telefono, Email, Direccion FROM propietario WHERE Id_propietario = @id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Propietario
                    {
                        Id_propietario = reader.GetInt32(0),
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

    public int Alta(Propietario propietario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"INSERT INTO propietario 
                    (DNI ,Nombre, Apellido, Telefono, Email, Direccion)
                    VALUES (@dni ,@nombre, @apellido, @telefono, @email, @direccion);
                    SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", propietario.DNI);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Propietario propietario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"UPDATE propietario SET 
                    DNI = @dni,
                    Nombre = @nombre, 
                    Apellido = @apellido,
                    Telefono = @telefono, 
                    Email = @email,
                    Direccion = @direccion
                    WHERE Id_propietario = @id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", propietario.Id_propietario);
                command.Parameters.AddWithValue("@dni", propietario.DNI);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
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
            var query = "DELETE FROM propietario WHERE Id_propietario = @id";
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
