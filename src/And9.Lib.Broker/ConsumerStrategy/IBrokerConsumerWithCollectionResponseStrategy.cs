namespace And9.Lib.Broker.ConsumerStrategy;

public interface IBrokerConsumerWithCollectionResponseStrategy<in TArg, out TResult>
{
    IAsyncEnumerable<TResult> ExecuteAsync(TArg arg);
}