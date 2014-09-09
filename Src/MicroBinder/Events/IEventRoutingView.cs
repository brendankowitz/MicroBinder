namespace MicroBinder.Events
{
    internal interface IEventRoutingView
    {
        EventToPresenter EventToPresenter { get; set; }
    }
}
