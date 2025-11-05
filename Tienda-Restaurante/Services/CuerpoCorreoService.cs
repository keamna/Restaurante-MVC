using System.Text;

namespace Tienda_Restaurante.Services
{
    public class CuerpoCorreoService : ICuerpoCorreoService
    {
        public string GenerarCuerpoVenta(List<DetalleCarrito> detalles)
        {
            var body = new StringBuilder();
            double total = 0;

            body.AppendLine("<!DOCTYPE html>");
            body.AppendLine("<html lang='es'>");
            body.AppendLine("<head>");
            body.AppendLine("<meta charset='UTF-8'>");
            body.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            body.AppendLine("<title>Confirmación de Compra</title>");
            body.AppendLine("</head>");
            body.AppendLine("<body style='font-family:Poppins, Arial, sans-serif; background-color:#fffcfa; color:#18181b; margin:0; padding:30px;'>");

            // Contenedor
            body.AppendLine("<div style='max-width:600px; margin:auto; background-color:#ffffff; border-radius:12px; box-shadow:0 4px 8px rgba(0,0,0,0.1); padding:30px;'>");

            // Encabezado
            body.AppendLine("<h2 style='text-align:center; color:#fc7f09; margin-bottom:10px;'>¡Gracias por tu compra!</h2>");
            body.AppendLine("<p style='text-align:center; color:#6b7280; font-size:15px;'>Tu pedido fue recibido correctamente y está siendo procesado.</p>");
            body.AppendLine("<hr style='border:none; border-top:2px solid #fc7f09; width:80px; margin:20px auto;'>");

            // Tabla en la que van lso productos
            body.AppendLine("<div style='display:flex; justify-content:center; margin-top:20px;'>");
            body.AppendLine("<table cellpadding='10' cellspacing='0' style='border-collapse:collapse; width:90%; text-align:center;'>");
            body.AppendLine("<thead>");
            body.AppendLine("<tr style='background-color:#fc7f09; color:#ffffff;'>");
            body.AppendLine("<th>Platillo</th>");
            body.AppendLine("<th>Cantidad</th>");
            body.AppendLine("<th>Precio</th>");
            body.AppendLine("</tr>");
            body.AppendLine("</thead>");
            body.AppendLine("<tbody>");

            //llena la tabla
            foreach (var item in detalles)
            {
                body.AppendLine("<tr style='border-bottom:1px solid #f2f2f2;'>");
                body.AppendLine($"<td style='padding:10px; font-weight:500;'>{item.Platillo.PlatilloName}</td>");
                body.AppendLine($"<td style='padding:10px; color:#18181b;'>{item.Cantidad}</td>");
                body.AppendLine($"<td style='padding:10px; color:#18181b;'>₡{item.PrecioUnitario:N2}</td>");
                body.AppendLine("</tr>");
                total += item.PrecioUnitario * item.Cantidad;
            }

            body.AppendLine("</tbody>");
            body.AppendLine("</table>");
            body.AppendLine("</div>");

            // Total
            body.AppendLine($"<h3 style='text-align:center; color:#18181b; margin-top:25px;'>Total: <span style='color:#fc7f09;'>₡{total:N2}</span></h3>");

            // Mensaje final
            body.AppendLine("<p style='text-align:center; color:#6b7280; font-size:14px; margin-top:15px;'>¡Gracias por preferirnos!</p>");

            // Cierre
            body.AppendLine("</div>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            return body.ToString();
        }

        public string GenerarCuerpoReserva(List<Reserva> reservas)
        {
            var body = new StringBuilder(); 
            body.AppendLine("<!DOCTYPE html>"); 
            body.AppendLine("<html lang='es'>"); 
            body.AppendLine("<head>"); 
            body.AppendLine("<meta charset='UTF-8'>"); 
            body.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>"); 
            body.AppendLine("<title>Confirmación de Reserva</title>"); body.AppendLine("</head>"); 
            body.AppendLine("<body style='font-family:Poppins, Arial, sans-serif; background-color:#fffcfa; color:#18181b; margin:0; padding:30px;'>"); 
            
            // Contenedor
            body.AppendLine("<div style='max-width:600px; margin:auto; background-color:#ffffff; border-radius:12px; box-shadow:0 4px 8px rgba(0,0,0,0.1); padding:30px;'>"); 

            // Encabezado
            body.AppendLine("<h2 style='text-align:center; color:#fc7f09; margin-bottom:10px;'>¡Reserva Confirmada!</h2>"); 
            body.AppendLine("<p style='text-align:center; color:#6b7280; font-size:15px;'>Tu reserva ha sido registrada correctamente.</p>"); 
            body.AppendLine("<hr style='border:none; border-top:2px solid #fc7f09; width:80px; margin:20px auto;'>"); 
            
            // Tabla con los detalles de la reserva
            body.AppendLine("<div style='display:flex; justify-content:center; margin-top:20px;'>");
            body.AppendLine("<table cellpadding='10' cellspacing='0' style='border-collapse:collapse; width:90%; text-align:center;'>"); 
            body.AppendLine("<thead>"); body.AppendLine("<tr style='background-color:#fc7f09; color:#ffffff;'>"); 
            body.AppendLine("<th>Fecha</th>"); body.AppendLine("<th>Hora Inicio</th>"); 
            body.AppendLine("<th>Hora Fin</th>"); body.AppendLine("<th>Usuario</th>"); 
            body.AppendLine("</tr>"); body.AppendLine("</thead>"); 
            body.AppendLine("<tbody>"); 
            
            foreach (var item in reservas) { 
                body.AppendLine("<tr style='border-bottom:1px solid #f2f2f2;'>"); 
                body.AppendLine($"<td style='padding:10px; font-weight:500;'>{item.Fecha:dd/MM/yyyy}</td>"); 
                body.AppendLine($"<td style='padding:10px; color:#18181b;'>{item.HoraInicio:hh\\:mm}</td>"); 
                body.AppendLine($"<td style='padding:10px; color:#18181b;'>{item.HoraFin:hh\\:mm}</td>"); 
                body.AppendLine($"<td style='padding:10px; color:#18181b;'>{item.Usuario?.UserName}</td>"); 
                body.AppendLine("</tr>"); } body.AppendLine("</tbody>"); body.AppendLine("</table>"); body.AppendLine("</div>"); 
                body.AppendLine("<p style='text-align:center; color:#6b7280; font-size:14px; margin-top:15px;'>¡Gracias por preferirnos!</p>"); 
            
            // Cierre
            body.AppendLine("</div>"); 
            body.AppendLine("</body>"); 
            body.AppendLine("</html>");

            return body.ToString();
        }
    }
}
