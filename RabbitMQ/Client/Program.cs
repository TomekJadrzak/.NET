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


    for (int i = 0; i < 20; i++)
    {
        Publish(channel, i.ToString());
    }
    
    WriteLine("End");
    Console.ReadLine();
}
catch (Exception ex)
{
    WriteLine(ex.ToString());
}


static void Publish(IModel channel, string message)
{
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    properties.MessageId = Guid.NewGuid().ToString();
    
    var body = Encoding.UTF8.GetBytes(message);

    WriteLine($"Next seq number: {channel.NextPublishSeqNo} messageId: {properties.MessageId}");
    channel.BasicPublish(
        "",
        "testdev1", 
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