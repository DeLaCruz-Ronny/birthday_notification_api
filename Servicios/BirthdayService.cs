using AutoMapper;
using birthday_notification_api.Models;
using birthday_notification_api.Models.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using MySql.Data.MySqlClient;
using System.Data;

namespace birthday_notification_api.Servicios
{
    public class BirthdayService
    {
        private readonly string? _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public BirthdayService(IConfiguration configuration, IMapper mapper)
        {
            Env.Load();

            _context = configuration.GetConnectionString("DefaultConnection");
            _mapper = mapper;

            //Llamar a las variables que estan en el usersecrets y trabajar con cloudinary para las img
            var conf = new ConfigurationBuilder()
                       .AddUserSecrets<Program>()
                       .Build();

            //Obtenemos los usersecrests
            //var cloudName = conf["MiApp:cloudName"];
            //var apiKey = conf["MiApp:apiKey"];
            //var apiSecrets = conf["MiApp:apiSecrets"];

            var cloudName = Environment.GetEnvironmentVariable("CLOUDNAME");
            var apiKey = Environment.GetEnvironmentVariable("APIKEY");
            var apiSecrets = Environment.GetEnvironmentVariable("APISECRETS");

            //Inicializamos Cloudinary
            var account = new Account(cloudName, apiKey, apiSecrets);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<BirthdayDTO>> ObtenerTodos()
        {
            var persona = new List<Birthday>();

            using var connection = new MySqlConnection(_context);
            await connection.OpenAsync();

            string query = "SELECT id,nombre,telefono,fecha_cumpleanos,url_img,fecha_registro FROM Usuarios";
            using var command = new MySqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                persona.Add(new Birthday
                {
                    id = reader.GetInt32("id"),
                    nombre = reader.GetString("nombre"),
                    telefono = reader.GetString("telefono"),
                    fecha_cumpleanos = reader.GetDateTime("fecha_cumpleanos"),
                    url_img = reader.GetString("url_img"),
                    fecha_registro = reader.GetDateTime("fecha_registro")
                });
            }

            return _mapper.Map<List<BirthdayDTO>>(persona);
        }

        public async Task<BirthdayDTO> ObtenerPorId(int id)
        {
            Birthday? persona = null;

            using var connection = new MySqlConnection(_context);
            await connection.OpenAsync();

            string query = "SELECT id,nombre,telefono,fecha_cumpleanos,url_img,fecha_registro FROM Usuarios";
            using var command = new MySqlCommand(query, connection);

            //Evitamos las inyecciones de sql
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                persona = new Birthday
                {
                    id = reader.GetInt32("id"),
                    nombre = reader.GetString("nombre"),
                    telefono = reader.GetString("telefono"),
                    fecha_cumpleanos = reader.GetDateTime("fecha_cumpleanos"),
                    url_img = reader.GetString("url_img"),
                    fecha_registro = reader.GetDateTime("fecha_registro")
                };
            }

            return _mapper.Map<BirthdayDTO>(persona);
        }

        public async Task<bool> CrearPersona(Birthday? newpersona)
        {
            using var connection = new MySqlConnection(_context);
            await connection.OpenAsync();

            string query = "INSERT INTO Usuarios(nombre,telefono,fecha_cumpleanos,url_img) VALUES (@nombre,@telefono,@fecha_cumpleanos,@url_img)";
            using var command = new MySqlCommand(query, connection);

            //Evitamos las inyecciones de sql
            command.Parameters.AddWithValue("@nombre", newpersona?.nombre);
            command.Parameters.AddWithValue("@telefono", newpersona?.telefono);
            command.Parameters.AddWithValue("@fecha_cumpleanos", newpersona?.fecha_cumpleanos);
            command.Parameters.AddWithValue("@url_img", newpersona?.url_img);

            var insertados = await command.ExecuteNonQueryAsync();
            return insertados > 0;
        }

        public async Task<bool> EliminarPersona(int id)
        {
            using var connection = new MySqlConnection(_context);
            await connection.OpenAsync();

            string query = "DELETE FROM Usuarios WHERE id = @id";
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            var delete = await command.ExecuteNonQueryAsync();
            return delete > 0;
        }

        public async Task<bool> ActualizarPersona(Birthday persona)
        {
            using var connection = new MySqlConnection(_context);
            await connection.OpenAsync();

            string query = @"UPDATE Usuarios
                             SET nombre = @nombre,
                                 telefono = @telefono,
                                 fecha_cumpleanos = @fecha_cumpleanos,
                                 url_img = @url_img
                             WHERE id = @id";

            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@nombre", persona?.nombre);
            command.Parameters.AddWithValue("@telefono", persona?.telefono);
            command.Parameters.AddWithValue("@fecha_cumpleanos", persona?.fecha_cumpleanos);
            command.Parameters.AddWithValue("@url_img", persona?.url_img);

            var update = await command.ExecuteNonQueryAsync();
            return update > 0;
        }

        public string SubirImg(string img)
        {
            var parametros = new ImageUploadParams()
            {
                File = new FileDescription(img),
                PublicId = Path.GetFileNameWithoutExtension(img),
                Overwrite = true
            };

            var resultado = _cloudinary.Upload(parametros);
            return resultado.Url.ToString();
        }


    }
}
