using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Loader;
using RabbitMQ.Client.Events;

try
{
    ConnectionFactory connectionFactory =
        new ConnectionFactory()
        {
            HostName = "localhost",
            VirtualHost = "dev1",
            UserName = "mqtest_dev1",
            Password = "mqtest",
        };

    using var connection = connectionFactory.CreateConnection();
    using var channel = connection.CreateModel();
    var queue = channel.QueueDeclare(
        string.Empty,
        false,
        true,
        true,
        null);
    
    channel.ConfirmSelect();
    
    channel.BasicAcks +=
        (_, eventArgs) =>
        {
            WriteLine("BasicAck " + eventArgs.DeliveryTag.ToString() + (eventArgs.Multiple ? "*" : string.Empty));
        };
    channel.BasicNacks +=
        (_, eventArgs) =>
        {
            WriteLine("BasicNack " + eventArgs.DeliveryTag.ToString() + (eventArgs.Multiple ? "*" : string.Empty));
        };
    channel.BasicReturn +=
        (_, eventArgs) =>
        {
            WriteLine($"BasicReturn {eventArgs.ReplyCode} {eventArgs.ReplyText}");
            WriteLine("Message id " + eventArgs.BasicProperties.MessageId);
        };

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, args) =>
    {
        var body = args.Body.ToArray();
        var response = Encoding.UTF8.GetString(body);
        WriteLine(
            $"Received answer: {response} messageId: {args.BasicProperties.MessageId} correlationId: {args.BasicProperties.CorrelationId}");
    };
    
    channel.BasicConsume(
        queue.QueueName,
        true,
        "consumer-tag",
        false,
        true,
        null,
        consumer);    

    while (true)
    {
        string? line = Console.ReadLine();

        if (string.IsNullOrEmpty(line))
        {
            break;
        }

        Publish(channel,
            line,
            "rpc",
            queue.QueueName);
    }

    WriteLine("End");
    Console.ReadLine();
}
catch (Exception ex)
{
    WriteLine(ex.ToString());
}


static void Publish(IModel channel, string message, string routingKey, string replyTo)
{
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    properties.MessageId = Guid.NewGuid().ToString();
    properties.CorrelationId = Guid.NewGuid().ToString();
    properties.ReplyTo = replyTo;
    var body = Encoding.UTF8.GetBytes(message);

    WriteLine($"Next seq number: {channel.NextPublishSeqNo} messageId: {properties.MessageId} correlationId {properties.CorrelationId}");
    channel.BasicPublish(
        "",
        routingKey, 
        true, 
        properties, 
        body
    );

    WriteLine("Sent");
}

static void WriteLine(string message)
{
    int threadId = Environment.CurrentManagedThreadId;
    Console.WriteLine( threadId.ToString() + " " + message);
}