using MySql.Data.MySqlClient;

namespace inmobiliariaAST.Models
{
    public class RepositorioUsuario{
        private string ConnectionString = "Server=localhost;User=root;Password=;Database=inm;SslMode=none";

        
        //obtener usuarios
        public List<Usuario> GetUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Estado = TRUE";

                using (var command = new MySqlCommand(query, connection)){
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                ID_usuario = reader.GetInt32("ID_usuario"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"),
                                Rol = reader.GetInt32("Rol"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Estado = reader.GetBoolean("Estado"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                            });
                        }
                    }
                }
            }
            return usuarios;
        }
        
        
        
        //alta usuario
        public int Alta(Usuario usuario)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                    INSERT INTO Usuario (Email, Password, Rol, Nombre, Apellido, Estado, Avatar)
                    VALUES (@Email, @Password, @Rol, @Nombre, @Apellido, @Estado, @Avatar);
                    SELECT LAST_INSERT_ID();";
                
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Password", usuario.Password);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Estado", usuario.Estado);
                    command.Parameters.AddWithValue("@Avatar", usuario.Avatar ?? (object)DBNull.Value);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        //getid
        public Usuario? GetID(int id){
             using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                    SELECT * FROM Usuario WHERE ID_usuario = @ID_usuario";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_usuario", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                ID_usuario = reader.GetInt32("ID_usuario"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"),
                                Rol = reader.GetInt32("Rol"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Estado = reader.GetBoolean("Estado"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void ActualizarUsuario(Usuario usuario){
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                UPDATE Usuario
                SET Email = @Email,
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    Estado = @Estado,
                    Avatar = @Avatar
                WHERE ID_usuario = @ID_usuario";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_usuario", usuario.ID_usuario);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@Estado", usuario.Estado);
                command.Parameters.AddWithValue("@Avatar", usuario.Avatar ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
                }
            }
        }

        //eliminacion logica
        public void EliminarUsuario(int id){
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                        UPDATE Usuario
                        SET Estado = FALSE
                        WHERE ID_usuario = @ID_usuario";

                 using (var command = new MySqlCommand(query, connection))
                 {
                    command.Parameters.AddWithValue("@ID_usuario", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                 }                        
            }
        }
         // Obtener todos los usuarios (inactivos incluidos) - útil para administradores
        public List<Usuario> ObtenerTodosUsuariosIncluidosInactivos()
        {
            var usuarios = new List<Usuario>();

           using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM Usuario";
                
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                ID_usuario = reader.GetInt32("ID_usuario"),
                                Email = reader.GetString("Email"),
                                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString("Password"),
                                Rol = reader.GetInt32("Rol"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Estado = reader.GetBoolean("Estado"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                            });
                        }
                    }
                }
            }
            return usuarios;
        }

        // Permitir al usuario eliminar su avatar
        public void EliminarAvatar(int id)
        {
           using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                    UPDATE Usuario
                    SET Avatar = @AvatarDefault
                    WHERE ID_usuario = @ID_usuario";
                    
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AvatarDefault", Usuario.AvatarDefault);
                    command.Parameters.AddWithValue("@ID_usuario", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // Verificar si un usuario es administrador
        public bool EsAdministrador(int id)
        {
           using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT Rol FROM Usuario WHERE ID_usuario = @ID_usuario";
                
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_usuario", id);

                    connection.Open();
                    var rol = command.ExecuteScalar();
                    return rol != null && (int)rol == (int)Roles.Administrador;
                }
            }
        }

        //obtener uisuario por email y password
        public Usuario? GetByEmail(string email)
        {
            Usuario? usuario = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = @"
                    SELECT * FROM Usuario
                    WHERE Email = @Email
                    AND Estado = TRUE";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                ID_usuario = reader.GetInt32("ID_usuario"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"), // Aquí obtienes el hash almacenado
                                Rol = reader.GetInt32("Rol"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Estado = reader.GetBoolean("Estado"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                            };
                        }
                    }
                }
            }
            return usuario;
        }
    }
}