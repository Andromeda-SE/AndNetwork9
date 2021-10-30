using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace AndNetwork9.Shared.Backend.Rabbit;

public static class RabbitConnectionPool
{
    public static readonly ConnectionFactory Factory = new()
    {
        HostName = "broker",
    };


    public static void SetConfiguration(IConfiguration configuration)
    {
        Factory.UserName = configuration["RABBITMQ_USER"];
        Factory.Password = configuration["RABBITMQ_PASS"];
    }

    public static void SetConfiguration(string username, string password)
    {
        Factory.UserName = username;
        Factory.Password = password;
    }
}