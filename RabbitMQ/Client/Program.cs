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
    
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    while (true)
    {
        Console.Write("> ");
        string? message = Console.ReadLine();
        if (string.IsNullOrEmpty((message)))
        {
            break;
        }

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            "",
            "testdev1", 
            false, 
            properties, 
            body
        );

        Console.WriteLine("Send");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

Console.WriteLine("End");