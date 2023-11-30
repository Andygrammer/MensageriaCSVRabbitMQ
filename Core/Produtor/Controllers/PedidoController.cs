using Core.Entidades;
using Microsoft.AspNetCore.Mvc;
using Produtor.Handlers;
using RabbitMQ.Client;
using System.Text;

namespace Produtor.Controllers
{
    [ApiController]
    [Route("/Pedido")]
    public class PedidoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PedidoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("ExportExcel")]
        public async Task<IActionResult> PostExcel()
        {
            var fila = _configuration.GetSection("RabbitMQ")["NomeFila"] ?? string.Empty;
            var servidor = _configuration.GetSection("RabbitMQ")["Servidor"] ?? string.Empty;
            var usuario = _configuration.GetSection("RabbitMQ")["Usuario"] ?? string.Empty;
            var senha = _configuration.GetSection("RabbitMQ")["Senha"] ?? string.Empty;

            var factory = new ConnectionFactory() { HostName = servidor, UserName = usuario, Password = senha };
            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                var nomePlan = _configuration.GetSection("Excel")["NomePlanilha"] ?? string.Empty;
                var nomePasta = _configuration.GetSection("Excel")["PastaPlanilha"] ?? string.Empty;
                var dirPlan = _configuration.GetSection("Excel")["DiretorioPlanilha"] ?? string.Empty;

                List<Pedido> dadosEscrever = new()
                {
                    new Pedido(1, new Usuario(1, "André", "andre@email.com")),
                    new Pedido(2, new Usuario(2, "Joana", "joana@email.com")),
                    new Pedido(3, new Usuario(3, "José", "jose@email.com"))
                };

                var csvModel = new CsvModel<Pedido>
                {
                    FileName = nomePlan,
                    FolderName = nomePasta,
                    DirName = dirPlan,
                    DataToWrite = dadosEscrever,
                    BatchSize = 10,
                    BufferSize = 1024
                };

                var excel = await ExcelHandler.WriteCsv(csvModel);
                var excelResult = Encoding.UTF8.GetPreamble().Concat(excel.ToArray()).ToArray();

                channel.QueueDeclare(
                    queue: fila,
                    durable: false,
                    exclusive: false,
                    autoDelete: true, // impede erros nas execuções posteriores
                    arguments: null);

                var body = excelResult;

                channel.BasicPublish(
                    exchange: "",
                    routingKey: fila,
                    basicProperties: null,
                    body: body);

                return File(excelResult, "text/csv", nomePlan);
            }
        }
    }
}