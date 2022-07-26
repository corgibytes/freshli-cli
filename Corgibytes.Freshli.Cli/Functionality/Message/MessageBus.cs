using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Corgibytes.Freshli.Cli.Functionality.Message;

public class MessageBus
{
    private readonly IConnectableObservable<IMessage> _connectableObservable;
    private readonly Subject<IMessage> _jobs = new();

    public MessageBus()
    {
        _connectableObservable = _jobs.ObserveOn(Scheduler.Default).Publish();
        _connectableObservable.Connect();
        SubscribeHandlers();
    }

    // ReSharper disable once UnusedMember.Global
    public void Dispatch(IMessage job) => _jobs.OnNext(job);

    private void SubscribeHandlers() => RegisterHandler<TestMessage>(job => new TestMessageHandler().Handle(job));

    private void RegisterHandler<T>(Action<T> handleAction) where T : IMessage =>
        _connectableObservable.OfType<T>().Subscribe(handleAction);
}
