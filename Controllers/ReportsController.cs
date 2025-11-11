using ClosedXML.Excel;
using InventorySys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace InventorySys.Controllers
{
    public class ReportsController : Controller
    {
        private readonly InventorySysContext _context;

        public ReportsController(InventorySysContext context)
        {
            _context = context;
        }

        public IActionResult ReportsMaterials()
        {
            return View();
        }

        public IActionResult ReportsTransactions()
        {
            return View();
        }

        // GET: Reports/DownloadMaterialsGeneral

        [HttpGet]

        public async Task<IActionResult> DownloadMaterialsGeneral()

        {

            try

            {

                var materials = await _context.TblMaterials

                    .Include(m => m.Collection)

                    .Include(m => m.Finiture)

                    .Include(m => m.Format)

                    .Include(m => m.Site)

                    .Include(m => m.User)

                    .ToListAsync();

                using (var workbook = new XLWorkbook())

                {

                    var worksheet = workbook.Worksheets.Add("Materiales");

                    // Encabezados

                    worksheet.Cell(1, 1).Value = "ID Material";

                    worksheet.Cell(1, 2).Value = "Código Material";

                    worksheet.Cell(1, 3).Value = "Descripción Material";

                    worksheet.Cell(1, 4).Value = "Colección";

                    worksheet.Cell(1, 5).Value = "Acabado";

                    worksheet.Cell(1, 6).Value = "Formato";

                    worksheet.Cell(1, 7).Value = "Sitio";

                    worksheet.Cell(1, 8).Value = "Fecha de Ingreso";

                    worksheet.Cell(1, 9).Value = "Stock";

                    worksheet.Cell(1, 10).Value = "Usuario";

                    // Estilo del encabezado

                    var headerRow = worksheet.Row(1);

                    headerRow.Style.Font.Bold = true;

                    headerRow.Style.Fill.BackgroundColor = XLColor.Green;

                    headerRow.Style.Font.FontColor = XLColor.White;

                    // Datos

                    int row = 2;

                    foreach (var material in materials)

                    {

                        worksheet.Cell(row, 1).Value = material.MaterialId;

                        worksheet.Cell(row, 2).Value = material.MaterialCode;

                        worksheet.Cell(row, 3).Value = material.MaterialDescription;

                        worksheet.Cell(row, 4).Value = material.Collection?.CollectionName ?? "";

                        worksheet.Cell(row, 5).Value = material.Finiture?.FinitureName ?? "";

                        worksheet.Cell(row, 6).Value = material.Format?.FormatName ?? "";

                        worksheet.Cell(row, 7).Value = material.Site?.SiteName ?? "";

                        worksheet.Cell(row, 8).Value = material.MaterialReceivedDate.ToString("dd/MM/yyyy");

                        worksheet.Cell(row, 9).Value = material.MaterialStock;

                        worksheet.Cell(row, 10).Value = material.User?.UserName ?? "";

                        row++;

                    }

                    // Ajustar ancho de columnas

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())

                    {

                        workbook.SaveAs(stream);

                        var content = stream.ToArray();

                        var fileName = $"Reporte_Materiales_General_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                    }

                }

            }

            catch (Exception ex)

            {

                return BadRequest(new { success = false, message = ex.Message });

            }

        }

        // GET: Reports/DownloadMaterialsByDate

        [HttpGet]

        public async Task<IActionResult> DownloadMaterialsByDate(string fechaInicio, string fechaFin)

        {

            try

            {

                if (!DateTime.TryParse(fechaInicio, out var startDateTime) || !DateTime.TryParse(fechaFin, out var endDateTime))

                {

                    return BadRequest(new { success = false, message = "Fechas inválidas" });

                }

                // Convertir DateTime a DateOnly

                var startDate = DateOnly.FromDateTime(startDateTime);

                var endDate = DateOnly.FromDateTime(endDateTime);

                var materials = await _context.TblMaterials

                    .Include(m => m.Collection)

                    .Include(m => m.Finiture)

                    .Include(m => m.Format)

                    .Include(m => m.Site)

                    .Include(m => m.User)

                    .Where(m => m.MaterialReceivedDate >= startDate && m.MaterialReceivedDate <= endDate)

                    .ToListAsync();

                using (var workbook = new XLWorkbook())

                {

                    var worksheet = workbook.Worksheets.Add("Materiales");

                    worksheet.Cell(1, 1).Value = "ID Material";

                    worksheet.Cell(1, 2).Value = "Código Material";

                    worksheet.Cell(1, 3).Value = "Descripción Material";

                    worksheet.Cell(1, 4).Value = "Colección";

                    worksheet.Cell(1, 5).Value = "Acabado";

                    worksheet.Cell(1, 6).Value = "Formato";

                    worksheet.Cell(1, 7).Value = "Sitio";

                    worksheet.Cell(1, 8).Value = "Fecha de Ingreso";

                    worksheet.Cell(1, 9).Value = "Stock";

                    worksheet.Cell(1, 10).Value = "Usuario";

                    var headerRow = worksheet.Row(1);

                    headerRow.Style.Font.Bold = true;

                    headerRow.Style.Fill.BackgroundColor = XLColor.Green;

                    headerRow.Style.Font.FontColor = XLColor.White;

                    int row = 2;

                    foreach (var material in materials)

                    {

                        worksheet.Cell(row, 1).Value = material.MaterialId;

                        worksheet.Cell(row, 2).Value = material.MaterialCode;

                        worksheet.Cell(row, 3).Value = material.MaterialDescription;

                        worksheet.Cell(row, 4).Value = material.Collection?.CollectionName ?? "";

                        worksheet.Cell(row, 5).Value = material.Finiture?.FinitureName ?? "";

                        worksheet.Cell(row, 6).Value = material.Format?.FormatName ?? "";

                        worksheet.Cell(row, 7).Value = material.Site?.SiteName ?? "";

                        worksheet.Cell(row, 8).Value = material.MaterialReceivedDate.ToString("dd/MM/yyyy");

                        worksheet.Cell(row, 9).Value = material.MaterialStock;

                        worksheet.Cell(row, 10).Value = material.User?.UserName ?? "";

                        row++;

                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())

                    {

                        workbook.SaveAs(stream);

                        var content = stream.ToArray();

                        var fileName = $"Reporte_Materiales_{fechaInicio}_{fechaFin}.xlsx";

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                    }

                }

            }

            catch (Exception ex)

            {

                return BadRequest(new { success = false, message = ex.Message });

            }

        }


        // GET: Reports/DownloadTransactionsGeneral


        [HttpGet]

        public async Task<IActionResult> DownloadTransactionsGeneral()

        {

            try

            {

                var transactions = await _context.TblDetailMovements

                    .Include(d => d.MaterialTransaction)

                    .ThenInclude(m => m.Material)

                    .Include(d => d.MaterialTransaction)

                    .ThenInclude(m => m.User)

                    .ToListAsync();

                using (var workbook = new XLWorkbook())

                {

                    var worksheet = workbook.Worksheets.Add("Transacciones");

                    worksheet.Cell(1, 1).Value = "ID Transacción";

                    worksheet.Cell(1, 2).Value = "Usuario";

                    worksheet.Cell(1, 3).Value = "Tipo Transacción";

                    worksheet.Cell(1, 4).Value = "Código Material";

                    worksheet.Cell(1, 5).Value = "Descripción Material";

                    worksheet.Cell(1, 6).Value = "Saldo Inicial";

                    worksheet.Cell(1, 7).Value = "Cantidad Entrada";

                    worksheet.Cell(1, 8).Value = "Cantidad Salida";

                    worksheet.Cell(1, 9).Value = "Saldo Actual";

                    worksheet.Cell(1, 10).Value = "Fecha Transacción";

                    var headerRow = worksheet.Row(1);

                    headerRow.Style.Font.Bold = true;

                    headerRow.Style.Fill.BackgroundColor = XLColor.Blue;

                    headerRow.Style.Font.FontColor = XLColor.White;

                    int row = 2;

                    foreach (var transaction in transactions)

                    {

                        worksheet.Cell(row, 1).Value = transaction.MaterialTransaction?.MaterialTransactionId;

                        worksheet.Cell(row, 2).Value = transaction.MaterialTransaction?.User?.UserName ?? "";

                        worksheet.Cell(row, 3).Value = transaction.MaterialTransaction?.MaterialTransactionType;

                        worksheet.Cell(row, 4).Value = transaction.MaterialTransaction?.Material?.MaterialCode ?? "";

                        worksheet.Cell(row, 5).Value = transaction.MaterialTransaction?.Material?.MaterialDescription ?? "";

                        worksheet.Cell(row, 6).Value = transaction.DetInitBalance;

                        worksheet.Cell(row, 7).Value = transaction.DetCantEntry;

                        worksheet.Cell(row, 8).Value = transaction.DetCantExit;

                        worksheet.Cell(row, 9).Value = transaction.DetCurrentBalance;

                        worksheet.Cell(row, 10).Value = transaction.MaterialTransaction?.MaterialTransactionDate.ToString("dd/MM/yyyy HH:mm");

                        row++;

                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())

                    {

                        workbook.SaveAs(stream);

                        var content = stream.ToArray();

                        var fileName = $"Reporte_Transacciones_General_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                    }

                }

            }

            catch (Exception ex)

            {

                return BadRequest(new { success = false, message = ex.Message });

            }

        }

        // GET: Reports/DownloadTransactionsByDate

        [HttpGet]

        public async Task<IActionResult> DownloadTransactionsByDate(string fechaInicio, string fechaFin)

        {

            try

            {

                if (!DateTime.TryParse(fechaInicio, out var startDate) || !DateTime.TryParse(fechaFin, out var endDate))

                {

                    return BadRequest(new { success = false, message = "Fechas inválidas" });

                }

                // Para transacciones, incluir todo el día final

                var endDateWithTime = endDate.AddDays(1).AddTicks(-1);

                var transactions = await _context.TblDetailMovements

                    .Include(d => d.MaterialTransaction)

                    .ThenInclude(m => m.Material)

                    .Include(d => d.MaterialTransaction)

                    .ThenInclude(m => m.User)

                    .Where(d => d.MaterialTransaction.MaterialTransactionDate >= startDate
&& d.MaterialTransaction.MaterialTransactionDate < endDate.AddDays(1))

                    .ToListAsync();

                using (var workbook = new XLWorkbook())

                {

                    var worksheet = workbook.Worksheets.Add("Transacciones");

                    worksheet.Cell(1, 1).Value = "ID Transacción";

                    worksheet.Cell(1, 2).Value = "Usuario";

                    worksheet.Cell(1, 3).Value = "Tipo Transacción";

                    worksheet.Cell(1, 4).Value = "Código Material";

                    worksheet.Cell(1, 5).Value = "Descripción Material";

                    worksheet.Cell(1, 6).Value = "Saldo Inicial";

                    worksheet.Cell(1, 7).Value = "Cantidad Entrada";

                    worksheet.Cell(1, 8).Value = "Cantidad Salida";

                    worksheet.Cell(1, 9).Value = "Saldo Actual";

                    worksheet.Cell(1, 10).Value = "Fecha Transacción";

                    var headerRow = worksheet.Row(1);

                    headerRow.Style.Font.Bold = true;

                    headerRow.Style.Fill.BackgroundColor = XLColor.Blue;

                    headerRow.Style.Font.FontColor = XLColor.White;

                    int row = 2;

                    foreach (var transaction in transactions)

                    {

                        worksheet.Cell(row, 1).Value = transaction.MaterialTransaction?.MaterialTransactionId;

                        worksheet.Cell(row, 2).Value = transaction.MaterialTransaction?.User?.UserName ?? "";

                        worksheet.Cell(row, 3).Value = transaction.MaterialTransaction?.MaterialTransactionType;

                        worksheet.Cell(row, 4).Value = transaction.MaterialTransaction?.Material?.MaterialCode ?? "";

                        worksheet.Cell(row, 5).Value = transaction.MaterialTransaction?.Material?.MaterialDescription ?? "";

                        worksheet.Cell(row, 6).Value = transaction.DetInitBalance;

                        worksheet.Cell(row, 7).Value = transaction.DetCantEntry;

                        worksheet.Cell(row, 8).Value = transaction.DetCantExit;

                        worksheet.Cell(row, 9).Value = transaction.DetCurrentBalance;

                        worksheet.Cell(row, 10).Value = transaction.MaterialTransaction?.MaterialTransactionDate.ToString("dd/MM/yyyy HH:mm");

                        row++;

                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())

                    {

                        workbook.SaveAs(stream);

                        var content = stream.ToArray();

                        var fileName = $"Reporte_Transacciones_{fechaInicio}_{fechaFin}.xlsx";

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                    }

                }

            }

            catch (Exception ex)

            {

                return BadRequest(new { success = false, message = ex.Message });

            }

        }

    }
}
