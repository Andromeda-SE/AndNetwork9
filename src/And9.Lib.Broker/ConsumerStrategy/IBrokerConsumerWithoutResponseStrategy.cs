namespace And9.Lib.Broker.ConsumerStrategy;

public interface IBrokerConsumerWithoutResponseStrategy<in T>
{
    ValueTask ExecuteAsync(T arg);
}