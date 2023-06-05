using RabbitMQ.Client;
using System;

namespace RabbitMQExcelCreate.Web.Services
{
    public class RabbitMQClientService
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        // Exchange, routing ve kuyruk adları
        public static string ExchangeName = "ExcelDirectExchange";
        public static string RoutingExcel = "excel-route-file";
        public static string QueueName = "queue-excel-file";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        // RabbitMQ ile bağlantı kurma işlemi
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                // Eğer kanal zaten açıksa, mevcut kanalı döndür
                return _channel;
            }

            // Kanal oluşturma
            _channel = _connection.CreateModel();

            // Exchange oluşturma ve tanımlama
            /**
                ExchangeName: Exchange'in adını belirtir. Exchange, mesajların yayınlandığı ve alındığı yerdir.
                type: "direct": Exchange türünü belirtir. Direct exchange, mesajı routingKey değeriyle eşleşen kuyruklara yönlendirir. Yani, routingKey doğrudan kuyruk adıyla eşleşmelidir.
                durable: true: Bu, exchange'in kalıcı olup olmadığını belirtir. Eğer true olarak ayarlanırsa, exchange kalıcı olacak ve RabbitMQ sunucusunun yeniden başlatılması durumunda bile korunacaktır.
                autoDelete: false: Bu, exchange'in otomatik silinip silinmeyeceğini belirtir. Eğer false olarak ayarlanırsa, exchange manuel olarak silinene kadar kalacaktır.
             */
            _channel.ExchangeDeclare(ExchangeName, type: "direct", durable: true, autoDelete: false);

            // Kuyruk oluşturma ve tanımlama
            /*
                QueueName: Kuyruğun adını belirtir. Kuyruk, mesajların geçici olarak depolandığı ve tüketilmeyi beklediği yerdir.
                durable: true: Bu, kuyruğun kalıcı olup olmadığını belirtir. Eğer true olarak ayarlanırsa, kuyruk kalıcı olacak ve RabbitMQ sunucusunun yeniden başlatılması durumunda bile korunacaktır.
                exclusive: false: Bu, kuyruğun başka bir bağlantı tarafından kullanılıp kullanılamayacağını belirtir. Eğer false olarak ayarlanırsa, kuyruk birden fazla bağlantı tarafından kullanılabilir.
                autoDelete: false: Bu, kuyruğun otomatik silinip silinmeyeceğini belirtir. Eğer false olarak ayarlanırsa, kuyruk manuel olarak silinene kadar kalacaktır.
                arguments: null: Bu, kuyruk için ek argümanları belirtir. Ek argümanlar, özel RabbitMQ işlevlerini etkinleştirmek veya yapılandırmak için kullanılabilir. null olarak ayarlandığında, ek argümanlar kullanılmaz.
             */
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Kuyruğu exchange ile bağlama
            /*
                exchange: ExchangeName: Kuyruğun bağlanacağı exchange'in adını belirtir. Exchange, mesajların yönlendirildiği yerdir.
                queue: QueueName: Exchange ile bağlanacak kuyruğun adını belirtir. Bu, mesajların exchange tarafından bu kuyruğa yönlendirileceği anlamına gelir.
                routingKey: RoutingExcel: Routing anahtarını belirtir. Bu, exchange'in mesajları kuyruğa yönlendirmek için kullanacağı anahtar değeridir. Exchange'in bu anahtara sahip mesajları ilgili kuyruğa yönlendireceği anlamına gelir.
             */
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingExcel);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");

            return _channel;
        }

        // Kaynakları temizleme
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
        }
    }
}
