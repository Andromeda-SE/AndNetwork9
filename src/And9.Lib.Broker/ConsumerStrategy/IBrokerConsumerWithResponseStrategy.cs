namespace And9.Lib.Broker.ConsumerStrategy;

public interface IBrokerConsumerWithResponseStrategy<in TArg, TResult>
{
    ValueTask<TResult> ExecuteAsync(TArg? arg);
}