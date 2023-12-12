using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;

namespace BoldReportsSamples.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }

    [HttpPost]
    public IActionResult Export(string writerFormat)
    {
        // Here, we have loaded the sales-order-detail sample report from application the folder wwwroot\Resources.
        FileStream inputStream = new FileStream("sales-order-detail.rdl", FileMode.Open, FileAccess.Read);
        MemoryStream reportStream = new MemoryStream();
        inputStream.CopyTo(reportStream);
        reportStream.Position = 0;
        inputStream.Close();
        BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();

        string fileName = null;
        WriterFormat format;
        string type = null;

        if (writerFormat == "PDF")
        {
            fileName = "sales-order-detail.pdf";
            type = "pdf";
            format = WriterFormat.PDF;
        }
        else if (writerFormat == "Word")
        {
            fileName = "sales-order-detail.docx";
            type = "docx";
            format = WriterFormat.Word;
        }
        else if (writerFormat == "CSV")
        {
            fileName = "sales-order-detail.csv";
            type = "csv";
            format = WriterFormat.CSV;
        }
        else
        {
            fileName = "sales-order-detail.xlsx";
            type = "xlsx";
            format = WriterFormat.Excel;
        }

        writer.LoadReport(reportStream);
        MemoryStream memoryStream = new MemoryStream();
        writer.Save(memoryStream, format);

        // Download the generated export document to the client side.
        memoryStream.Position = 0;
        FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
        fileStreamResult.FileDownloadName = fileName;
        return fileStreamResult;
    }
}
