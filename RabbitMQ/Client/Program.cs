using System.Collections.Generic;
using System.Diagnostics;

const string ROUTING_HEADER = "RH";

if (args.Length == 0)
{
    Console.WriteLine("Too few args");
    return;
}

int count = Convert.ToInt32(args[0]);
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

    Stopwatch watch = Stopwatch.StartNew();
    for (int i = 0; i < count; i++)
    {
        Publish(channel, 
            i.ToString(), 
            (i % 2).ToString(),
            (i%3).ToString());
    }
    watch.Stop();

    Console.WriteLine($"Time {watch.ElapsedMilliseconds} ms");

    WriteLine("End");
    Console.ReadLine();
}
catch (Exception ex)
{
    WriteLine(ex.ToString());
}


static void Publish(IModel channel, string message, string routingKey, string header)
{
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    properties.MessageId = Guid.NewGuid().ToString();
    properties.Headers =
        new Dictionary<string, object>()
        {
            {ROUTING_HEADER, header}
        };
    
    var body = Encoding.UTF8.GetBytes(message);

    WriteLine($"Next seq number: {channel.NextPublishSeqNo} messageId: {properties.MessageId}");
    // channel.BasicPublish(
    //     "",
    //     "testdev1", 
    //     true, 
    //     properties, 
    //     body
    // );
    channel.BasicPublish(
        "headersdev1",
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