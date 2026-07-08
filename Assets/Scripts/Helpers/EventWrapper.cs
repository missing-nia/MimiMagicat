
namespace Magicat.Helpers
{
    // I'm not sure this is the right way to handle this but I legitimately don't understand
    // how to use delegates over events in this case LOL
    /// <summary>
    /// Wrapper class for Action events. Goal is to allow passing of events anonymously for buff subscription logic.
    /// </summary>
    public class EventWrapper
    {
        public event System.Action Event;

        public void Invoke()
        {
            Event?.Invoke();
        }
    }

    /// <summary>
    /// Wrapper class for Action events with a message of type T. Allows passing of event anonymously for events such as buff application of a target.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventWrapperWithMessage<T>
    {
        public event System.Action<T> Event;

        public void Invoke(T message)
        {
            Event?.Invoke(message);
        }
    }
}
