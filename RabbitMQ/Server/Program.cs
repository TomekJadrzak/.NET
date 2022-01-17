using RabbitMQ.Client.Events;

if (args.Length == 0)
{
    Console.WriteLine("Too few args");
    return;
}

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

    var consumer = new EventingBasicConsumer(channel);
    
    consumer.Received += (_, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);
        channel.BasicAck(ea.DeliveryTag, false);
    };
    
    // channel.BasicConsume(
    //     "testdev1",
    //     false,
    //     "test-tag",
    //     false,
    //     false,
    //     null,
    //     consumer);
    channel.BasicConsume(
        args[0],
        false,
        "test-tag",
        false,
        false,
        null,
        consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

Console.WriteLine("End");