using APLICACIONPRUEBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;



public class UsuarioController : Controller
{
    private readonly IConfiguration _configuration;

    public UsuarioController(IConfiguration configuration)
    {
        _configuration = configuration;
    }



    public IActionResult Index(int? idUsuario, string email)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        List<Usuario> usuarios = new List<Usuario>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("buscarUsuario", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID_Usuario", (object?)idUsuario ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                    Nombre = reader["Nombre"].ToString(),
                    PrimerApellido = reader["PrimerApellido"].ToString(),
                    SegundoApellido = reader["SegundoApellido"].ToString(),
                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                    Email = reader["Email"].ToString(),
                    Contraseña = reader["Contraseña"].ToString()
                });
            }
        }

        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Usuario usuario)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        int resultado = 0;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("Registrar", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@PrimerApellido", usuario.PrimerApellido);
            cmd.Parameters.AddWithValue("@SegundoApellido", usuario.SegundoApellido);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);

            SqlParameter outputParam = new SqlParameter("@Existe", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            connection.Open();
            cmd.ExecuteNonQuery();

            resultado = (int)outputParam.Value;
        }

        if (resultado == 1)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Mensaje = "El correo ya está registrado.";
            return View(usuario);
        }
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {

        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        Usuario usuario = null;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("buscarUsuario", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID_Usuario", id);
            cmd.Parameters.AddWithValue("@Email", DBNull.Value);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                usuario = new Usuario
                {
                    ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                    Nombre = reader["Nombre"].ToString(),
                    PrimerApellido = reader["PrimerApellido"].ToString(),
                    SegundoApellido = reader["SegundoApellido"].ToString(),
                    Email = reader["Email"].ToString(),
                    Contraseña = reader["Contraseña"].ToString()
                };
            }
        }

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost]
    public IActionResult Editar(Usuario usuario)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        int resultado = 0;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("actualizarUsuario", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID_Usuario", usuario.ID_Usuario);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@PrimerApellido", usuario.PrimerApellido);
            cmd.Parameters.AddWithValue("@SegundoApellido", usuario.SegundoApellido);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);

            SqlParameter outputParam = new SqlParameter("@Resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            connection.Open();
            cmd.ExecuteNonQuery();

            resultado = (int)outputParam.Value;
        }

        if (resultado == 1)
        {
            return RedirectToAction("Index");
        }
        else if (resultado == 2)
        {
            ViewBag.Mensaje = "Ya existe un usuario con ese correo.";
            return View(usuario);
        }
        else
        {
            ViewBag.Mensaje = "No se encontró el usuario.";
            return View(usuario);
        }
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        int resultado = 0;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("eliminarUsuario", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID_Usuario", id);

            SqlParameter outputParam = new SqlParameter("@Resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            connection.Open();
            cmd.ExecuteNonQuery();

            resultado = (int)outputParam.Value;
        }

        if (resultado == 1)
        {
            TempData["Mensaje"] = "Usuario eliminado correctamente.";
        }
        else
        {
            TempData["Mensaje"] = "No se encontró el usuario a eliminar.";
        }

        return RedirectToAction("Index");
    }

}
