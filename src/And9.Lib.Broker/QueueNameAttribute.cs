namespace And9.Lib.Broker;

public class QueueNameAttribute : Attribute
{
    private QueueNameAttribute() => QueueName = string.Empty;

    public QueueNameAttribute(string queueName) => QueueName = queueName;

    public string QueueName { get; }
}