using Confluent.Kafka;

namespace AdminApi.Kafka;

public class KafkaProducer {
    private static readonly IConfiguration _config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddIniFile("kafkaclient.properties", false)
        .Build();

    private static readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(_config.AsEnumerable()).Build();
    
    public static void Produce (string topic, string key, string value) {
        _producer.Produce(
            topic,
            new Message<string, string> { Key = key, Value = value },
            (deliveryReport) => {
                if (deliveryReport.Error.Code != ErrorCode.NoError) {
                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                } else {
                    Console.WriteLine($"Produced event to topic {topic}: key = {deliveryReport.Message.Key, -10} value = {deliveryReport.Message.Value}");
                }
            }
        );

        // send any outstanding or buffered messages to the Kafka broker
        _producer.Flush(TimeSpan.FromSeconds(10));
    }
}