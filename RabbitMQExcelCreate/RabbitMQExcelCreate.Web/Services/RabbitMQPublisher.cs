using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQExcelCreate.Web.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }
        public void Publish(CreateExcelMessage createExcelMessage)
        {
            // RabbitMQClientService sınıfından bir kanal bağlantısı alınır
            var channel = _rabbitmqClientService.Connect();

            // createExcelMessage nesnesi JSON formatına dönüştürülür
            var bodyString = JsonSerializer.Serialize(createExcelMessage);

            // JSON verisi UTF-8 olarak byte dizisine dönüştürülür
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            // Mesajın özelliklerini oluşturmak için kanaldan basic özellikler alınır
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Mesaj kalıcı olacak şekilde ayarlanır

            // Mesaj, RabbitMQ'ya yayınlanır
            /*
                exchange: RabbitMQClientService.ExchangeName: Mesajın yayınlanacağı exchange'in adını belirtir. Exchange, mesajları alıcı kuyruklara yönlendirmek için kullanılır.
                routingKey: RabbitMQClientService.RoutingExcel: Mesajın yönlendirileceği kuyruğu belirtir. Routing anahtarı, exchange tarafından mesajların doğru kuyruğa yönlendirilmesi için kullanılır. Bu parametre, mesajın doğru kuyruğa iletilebilmesi için kullanılır.
                mandatory: true: Bu, mesajın bir kuyruğa yönlendirilemediği durumda geri dönmesini belirtir. Eğer true olarak ayarlanırsa, mesaj bir kuyruğa yönlendirilemezse RabbitMQ tarafından geri döner. Eğer false olarak ayarlanırsa, mesaj sessizce atılır.
                basicProperties: properties: Mesajın özelliklerini belirtir. Bu parametre, mesajın kalıcılığı, önceliği, başlık bilgileri gibi özelliklerini belirlemek için kullanılır. properties değişkeni, channel.CreateBasicProperties() ile oluşturulan bir IBasicProperties nesnesidir.
                body: bodyByte: Mesajın içeriğini temsil eder. Bu parametre, byte dizisi olarak gönderilen mesajın kendisini içerir.
             */
            channel.BasicPublish(
                exchange: RabbitMQClientService.ExchangeName,
                routingKey: RabbitMQClientService.RoutingExcel,
                mandatory: true,
                basicProperties: properties,
                body: bodyByte);
        }
    }
}
