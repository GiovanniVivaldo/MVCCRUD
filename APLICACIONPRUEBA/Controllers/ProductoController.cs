using APLICACIONPRUEBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

public class ProductoController : Controller
{
    private readonly IConfiguration _configuration;

    public ProductoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Mostrar productos con filtros
    public IActionResult VerProductos(int? IdProducto, string NombreProducto, string marca, float? precio, string cantidad, string categoria)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("usuarioEmail")))
        {
            return RedirectToAction("Login", "Auth");
        }

        List<Producto> productos = new List<Producto>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand cmd = new SqlCommand("VisualizarProductos", connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID_Producto", (object?)IdProducto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Nombre_Producto", string.IsNullOrWhiteSpace(NombreProducto) ? DBNull.Value : NombreProducto);
            cmd.Parameters.AddWithValue("@Marca", string.IsNullOrWhiteSpace(marca) ? DBNull.Value : marca);
            cmd.Parameters.AddWithValue("@Precio", (object?)precio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cantidad", string.IsNullOrWhiteSpace(cantidad) ? DBNull.Value : cantidad);
            cmd.Parameters.AddWithValue("@Categoria", string.IsNullOrWhiteSpace(categoria) ? DBNull.Value : categoria);

            connection.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    productos.Add(new Producto
                    {
                        ID_Producto = Convert.ToInt32(reader["ID_Producto"]),
                        Nombre_Producto = reader["Nombre_Producto"].ToString(),
                        Marca = reader["Marca"].ToString(),
                        Precio = float.Parse(reader["Precio"].ToString()),
                        Cantidad = reader["Cantidad"].ToString(),
                        Categoria = reader["Categoria"].ToString()
                    });
                }
            }
        }

        return View(productos);
    }

    // POST: Registrar venta
    [HttpPost]
    public IActionResult Vender(int idProducto, int cantidad)
    {
        var email = HttpContext.Session.GetString("usuarioEmail");

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Auth");
        }

        int idUsuario = 0;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // 1. Buscar ID del usuario por email
            using (SqlCommand cmd = new SqlCommand("buscarUsuario", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@ID_Usuario", DBNull.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        idUsuario = Convert.ToInt32(reader["ID_Usuario"]);
                    }
                }
            }

            if (idUsuario == 0)
            {
                TempData["Mensaje"] = "Usuario no encontrado.";
                return RedirectToAction("VerProductos");
            }

            // 2. Registrar venta
            using (SqlCommand cmd = new SqlCommand("RegistrarVenta", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);
                cmd.Parameters.AddWithValue("@ID_Producto", idProducto);
                cmd.Parameters.AddWithValue("@CantidadVendida", cantidad);

                SqlParameter output = new SqlParameter("@Resultado", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(output);

                cmd.ExecuteNonQuery();

                int resultado = (int)output.Value;
                if (resultado == 1)
                    TempData["Mensaje"] = "Venta registrada exitosamente.";
                else
                    TempData["Mensaje"] = "No hay suficiente stock para vender.";
            }
        }

        return RedirectToAction("VerProductos");
    }
}

