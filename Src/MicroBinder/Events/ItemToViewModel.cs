using System;
using System.Reflection;
using System.ServiceModel;
using Xamarin.Forms;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyPropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace MicroBinder.Events
{
    public sealed class ItemToViewModel
    {
        public static readonly DependencyProperty EventProperty = DependencyProperty
            .CreateAttached<ItemToViewModel, AttachedEventMessage>(
                bindable => ItemToViewModel.GetEvent(bindable),
                null,
                BindingMode.OneWay,
                null,
                (b, o, n) => OnSetEventChanged(b, o, n));
        
        static readonly MethodInfo RoutedEventHandlerDefinition = 
            typeof(ItemToViewModel)
            .GetTypeInfo()
            .GetDeclaredMethod("RoutedEventHandler")
            .GetGenericMethodDefinition();

        public static AttachedEventMessage GetEvent(BindableObject element)
        {
            return element.GetValue(EventProperty) as AttachedEventMessage;
        }

        public static void SetEvent(BindableObject element, AttachedEventMessage value)
        {
            element.SetValue(EventProperty, value);
            Attach(() => (Element)element, element, value);
        }

        public static void OnSetEventChanged(BindableObject bo, AttachedEventMessage oldValue, AttachedEventMessage newValue)
        {
            SetEvent (bo, newValue);
        }

   
        public static void Attach(Func<Element> elementResolve, object source, AttachedEventMessage message)
        {
            var element = elementResolve();

            foreach (var ev in message.EventData.EventItems)
            {
                if (string.IsNullOrEmpty(ev.TargetMethod)) // && string.IsNullOrEmpty(element.Name))
                    throw new ArgumentNullException("elementResolve", "The element hosting the Event must be named.");

                //if (string.IsNullOrEmpty(ev.TargetMethod))
                //    ev.TargetMethod = element.Name;

                var eventInfo = source.GetType().GetRuntimeEvent(ev.SourceEvent);

                if (eventInfo == null)
                    throw new ArgumentException(string.Format("Event '{0}' was not found on '{1}'",
                                                              ev.SourceEvent,
                                                              ev.TargetMethod ?? element.GetType().Name));

                var handlerMethod = eventInfo.EventHandlerType.GetTypeInfo().GetDeclaredMethod("Invoke");
                var eventArgs = handlerMethod.GetParameters()[1];
                var routedEventHandler = RoutedEventHandlerDefinition.MakeGenericMethod(eventArgs.ParameterType);
                routedEventHandler.Invoke(null, new[] {elementResolve, source, eventInfo, ev});
            }
        }

        public static void RoutedEventHandler<TArgs>(Func<FrameworkElement> visualSourceResolve, object eventSource, EventInfo eventInfo, EventDataItem data)
        {
            Action<object, TArgs> handlerClosure = (x, y) =>
                    {
                        var visualSource = visualSourceResolve();
                        var tryViewModel = visualSource.Parent.BindingContext;

                        EventToPresenter.Invoke(tryViewModel, visualSource, eventSource, data, y);

                    };

            var del = handlerClosure.GetMethodInfo().CreateDelegate(eventInfo.EventHandlerType, handlerClosure.Target);
            eventInfo.AddEventHandler(eventSource, del);
        }
    }
}
