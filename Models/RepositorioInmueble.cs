using MySql.Data.MySqlClient;
namespace inmobiliariaAST.Models;

public class RepositorioInmueble{
    private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

    //Obtener Inmuebles
    public List<Inmueble> GetInmuebles()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"
                SELECT 
                    i.ID_inmueble,
                    i.Direccion AS Inmueble_Direccion,
                    i.Uso AS uso,
                    i.Tipo as tipo,
                    i.Cantidad_Ambientes AS CA,
                    i.Latitud as Latitud,
                    i.Longitud AS Longitud,
                    i.Precio AS Price,
                    i.Estado ,
                    p.ID_propietario,
                    p.Nombre AS Propietario_Nombre,
                    p.Apellido AS Propietario_Apellido
                FROM 
                    Inmueble i
                JOIN 
                    Propietario p ON i.ID_propietario = p.ID_propietario
                WHERE 
                    i.Estado = 1;";
            
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var inmueble = new Inmueble
                    {
                        ID_inmueble = reader.GetInt32(0),
                        Direccion = reader.GetString(1),
                        Uso = Enum.Parse<UsoInmueble>(reader.GetString(2)),
                        Tipo = Enum.Parse<TipoInmueble>(reader.GetString(3)),
                        Cantidad_Ambientes = reader.GetInt32(4),
                        Latitud = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        Longitud = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                        Precio = reader.GetDecimal(7),
                        Estado = reader.GetBoolean(8),
                        ID_propietario = reader.GetInt32(9),
                        Propietario = new Propietario // Aquí añadimos los datos del propietario
                        {
                            ID_propietario = reader.GetInt32(9),
                            Nombre = reader.GetString(10),
                            Apellido = reader.GetString(11)
                        }
                    };

                    inmuebles.Add(inmueble);
                }
                connection.Close();
            }
        }
        return inmuebles;
    }


    //inmueble por ID
    public Inmueble? Get(int id)
    {
        Inmueble? res = null;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"
                SELECT 
                    i.ID_inmueble,
                    i.Direccion,
                    i.Uso,
                    i.Tipo,
                    i.Cantidad_Ambientes,
                    i.Latitud,
                    i.Longitud,
                    i.Precio,
                    i.Estado,
                    p.ID_propietario,
                    p.Nombre AS Propietario_Nombre,
                    p.Apellido AS Propietario_Apellido
                FROM 
                    Inmueble i
                JOIN 
                    Propietario p ON i.ID_propietario = p.ID_propietario
                WHERE 
                    i.ID_inmueble = @id;";
                    
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Inmueble
                    {
                        ID_inmueble = reader.GetInt32(0),
                        Direccion = reader.GetString(1),
                        Uso = Enum.Parse<UsoInmueble>(reader.GetString(2)),
                        Tipo = Enum.Parse<TipoInmueble>(reader.GetString(3)),
                        Cantidad_Ambientes = reader.GetInt32(4),
                        Latitud = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        Longitud = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                        Precio = reader.GetDecimal(7),
                        Estado = reader.GetBoolean(8),
                        ID_propietario = reader.GetInt32(9),
                        Propietario = new Propietario 
                        {
                            ID_propietario = reader.GetInt32(9),
                            Nombre = reader.GetString(10),
                            Apellido = reader.GetString(11)
                        }
                    };
                }
                connection.Close();
            }
        }
        return res;
    }


    //Alta Inmueble
    public int Alta(Inmueble inmueble)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"INSERT INTO inmueble 
                        (Direccion, Uso, Tipo, Cantidad_Ambientes, Latitud, Longitud, Precio, Estado, ID_propietario)
                        VALUES (@direccion, @uso, @tipo, @cantidad_ambientes, @latitud, @longitud, @precio, @estado, @id_propietario);
                        SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo.ToString());
                command.Parameters.AddWithValue("@cantidad_ambientes", inmueble.Cantidad_Ambientes);
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud.HasValue ? (object)inmueble.Latitud.Value : DBNull.Value);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud.HasValue ? (object)inmueble.Longitud.Value : DBNull.Value);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@estado", true);
                command.Parameters.AddWithValue("@id_propietario", inmueble.ID_propietario);
                
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }

 
    //Modificar Inmueble
    public int Modificar(Inmueble inmueble)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = @"UPDATE inmueble SET 
                        Direccion = @direccion,
                        Uso = @uso, 
                        Tipo = @tipo,
                        Cantidad_Ambientes = @cantidad_ambientes,
                        Latitud = @latitud,
                        Longitud = @longitud,
                        Precio = @precio,
                        ID_propietario = @id_propietario
                        WHERE ID_inmueble = @id ;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inmueble.ID_inmueble);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo.ToString());
                command.Parameters.AddWithValue("@cantidad_ambientes", inmueble.Cantidad_Ambientes);
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud.HasValue ? (object)inmueble.Latitud.Value : DBNull.Value);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud.HasValue ? (object)inmueble.Longitud.Value : DBNull.Value);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                // command.Parameters.AddWithValue("@estado", inmueble.Estado.ToString());
                command.Parameters.AddWithValue("@id_propietario", inmueble.ID_propietario);
                
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }


    //Baja Inmueble
    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "UPDATE inmueble SET Estado = 0  WHERE ID_inmueble = @id;";
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