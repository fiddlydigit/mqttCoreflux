
using Coreflux.API.Networking.MQTT;

namespace AlexanderMQTTClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            //inicialização do mqttcontroller que se liga. só precisas de 3 eventos:
            // On Connect ->  Evento de Connect
            // On Disconnect -> Evento de disconnect
            // NewPayload -> topico quando algo muda
            MQTTController.StartAsync("127.0.0.1", 1883);
            MQTTController.OnConnect += MQTTController_OnConnect;
            MQTTController.OnDisconnect += MQTTController_OnDisconnect; 
            MQTTController.NewPayload += MQTTController_NewPayload;
            _logger = logger;
        }

        private void ListTopicsToGet()
        {
            var dummy=MQTTController.GetDataAsync("TOPIC/MYVALUE");
            dummy.Wait();
            var payload=dummy.Result;

        }


        private void MQTTController_NewPayload(MQTTNewPayload obj)
        {
            //Existe um sistema de tags. Por isso trabalha com topicos de mqtt hard coded depois vemos isso no fim. 
            // identifica as necessidades do lado no sql, para esta fase isto chega
           if(obj.topic.Equals("TOPIC/MYVALUE"))
            {
                _logger.LogInformation($"Received {obj.payload} in topic  {obj.topic} @ {DateTimeOffset.Now}");
            }
        }

        private void MQTTController_OnDisconnect()
        {
            //não fazemos ainda nada mas podes fazer o start outra vez....
            _logger.LogError("Worker disconnected at: {time}", DateTimeOffset.Now);
        }

        private void MQTTController_OnConnect()
        {
            ListTopicsToGet();
            _logger.LogInformation("Worker connected at: {time}", DateTimeOffset.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //serviço não faz nada sincrono!!! ainda
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}