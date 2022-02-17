using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace And9.Lib.Broker;

public static class RabbitConnectionPool
{
    public static readonly ConnectionFactory Factory = new()
    {
        HostName = "and9.infra.broker",
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