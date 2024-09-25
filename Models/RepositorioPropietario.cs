using MySql.Data.MySqlClient;

namespace inmobiliariaAST.Models;

public class RepositorioPropietario
{
    private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

    public List<Propietario> GetPropietarios()
    {
        List<Propietario> propietarios = new List<Propietario>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT ID_propietario, DNI, Nombre, Apellido, Telefono, Email, Direccion, Estado FROM propietario WHERE Estado = true";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    propietarios.Add(new Propietario
                    {
                        ID_propietario = reader.GetInt32(0),
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
        return propietarios;
    }

    public Propietario? Get(int id)
    {
        Propietario? res = null;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT ID_propietario, DNI, Nombre, Apellido, Telefono, Email, Direccion, Estado FROM propietario WHERE ID_propietario = @id AND Estado = true";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Propietario
                    {
                        ID_propietario = reader.GetInt32(0),
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

    public int Alta(Propietario propietario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"INSERT INTO propietario 
                    (DNI ,Nombre, Apellido, Telefono, Email, Direccion, Estado)
                    VALUES (@dni ,@nombre, @apellido, @telefono, @email, @direccion, @estado);
                    SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", propietario.DNI);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                command.Parameters.AddWithValue("@estado", true);
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
                    Direccion = @direccion,
                    Estado = @estado
                    WHERE ID_propietario = @id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", propietario.ID_propietario);
                command.Parameters.AddWithValue("@dni", propietario.DNI);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                command.Parameters.AddWithValue("@estado", propietario.Estado);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    //logico
    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            
            var query = @"UPDATE propietario SET Estado = false WHERE ID_propietario = @id;
                        UPDATE inmueble SET Estado = false WHERE ID_propietario = @id;
                        UPDATE contrato SET Estado = false WHERE ID_inmueble IN (SELECT ID_inmueble FROM inmueble WHERE ID_propietario = @id)";
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
