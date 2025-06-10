using ScisaAPI.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ScisaAPI.Utils
{
    public class ExportarExcel
    {
        public static byte[] ExportarListadoPokemon(List<Pokemon> lista)
        {
            byte[] datos;
            //Especificación de licencia no comercial
            ExcelPackage.License.SetNonCommercialPersonal("Azael Lopez Robles");
            using (var package = new ExcelPackage())
            {
                package.Workbook.Properties.Author = "Azael Lopez Robles";
                
                //Hoja
                var worksheet = package.Workbook.Worksheets.Add("ListadoPokemon");

                //Columnas
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "URL de Imagen";

                //Estilo
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.AutoFitColumns();
                }

                // Datos
                for (int i = 0; i < lista.Count; i++)
                {
                    var p = lista[i];
                    worksheet.Cells[i + 2, 1].Value = p.Id;
                    worksheet.Cells[i + 2, 2].Value = p.Nombre;
                    worksheet.Cells[i + 2, 3].Value = p.Imagen;                    
                }

                datos = package.GetAsByteArray();

            }

            return datos;
                 
        }
    }
}
