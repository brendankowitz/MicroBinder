using Xamarin.Forms;

namespace MicroBinder.Events
{
    [TypeConverter(typeof(AttachedEventTypeConverter))]
    public class AttachedEventMessage
    {
        static readonly AttachedEventParser parser = new AttachedEventParser();

        readonly string message;
        public string Message { get { return message; } }

        public EventData EventData { get; private set; }

        public AttachedEventMessage(string message)
        {
            this.message = message;
            EventData = parser.Parse(message);
        }
    }
}