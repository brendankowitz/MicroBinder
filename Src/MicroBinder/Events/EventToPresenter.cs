using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyPropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace MicroBinder.Events
{
    internal class EventToPresenter
    {
        static readonly Regex bindingParser = new Regex(@"^\{Binding[ ]?[Path=]*?(?<path>[\w\.]+)*\}$");
        static readonly Type[] delegateCache = new[] { typeof(Action), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>) };

        public static void Invoke(object presenterObj, FrameworkElement element, object source, EventDataItem eventData, object eventArgs)
        {
            var parameters = new List<object>();
            if(eventData.Parameters != null && eventData.Parameters.Any())
            {
                var sourceType = source.GetType();
                foreach(var param in eventData.Parameters)
                {
                    if(param.Equals("$sender$"))
                    {
                        parameters.Add(source);
                    }
                    else if(param.Equals("$args$"))
                    {
                        parameters.Add(eventArgs);
                    }
                    else if(param.StartsWith("{"))
                    {
                        var results = bindingParser.Matches(param);
                        if(results.Count == 0)
                            throw new ArgumentException(string.Format("'{0}' couldn't be parsed as binding syntax", param));
                        var b = new Binding(results[0].Groups["path"].Value);
                        var be = new BindingEvaluator(b){ BindingContext = element.BindingContext };
                        parameters.Add(be.Value);
                    }
                    else
                    {
                        var property = sourceType.GetTypeInfo().GetDeclaredProperty(param);
                        var value = property.GetValue(source, null);
                        parameters.Add(value);
                    }
                }
            }

            if(eventData.CachedDelegate == null)
            { //Create a cached invoke delegate if an Action delegate matches
                var presenterType = presenterObj.GetType();

                MethodInfo targetMethod;
                //if (parameters.Any(x => x == null) == false)
                //{
                    //targetMethod = presenterType.GetTypeInfo().GetDeclaredMethods(eventData.TargetMethod, parameters.Select(x => x.GetType()).ToArray());
                //}
                //else
                //{ //could not be found by parameter types
                    targetMethod = presenterType.GetTypeInfo().GetDeclaredMethod(eventData.TargetMethod);
                //}

                if(targetMethod == null)
                    throw new NotImplementedException(string.Format("Method {0} does not exist on {1}", eventData.TargetMethod, presenterType.Name));

                var @delegate = delegateCache.FirstOrDefault(x => x.GetTypeInfo().GenericTypeArguments.Count() == parameters.Count);

                if(@delegate != null)
                {
                    var typedDelegate = parameters.Count > 0
                                            ? @delegate.MakeGenericType(parameters.Select(x => x.GetType()).ToArray())
                                            : @delegate;
                    eventData.CachedDelegate = targetMethod.CreateDelegate(typedDelegate, presenterObj);
                }
                else
                { //reflection invoke
                    try
                    {
                        targetMethod.Invoke(presenterObj, parameters.ToArray());
                    }
                    catch(TargetInvocationException ex)
                    { // bubble real error
                        throw ex.InnerException ?? ex;
                    }
                }
            }
            if(eventData.CachedDelegate != null)
            { //delegate invoke
                try
                {
                    eventData.CachedDelegate.DynamicInvoke(parameters.ToArray());
                }
                catch(TargetInvocationException ex)
                { // bubble real error
                    throw ex.InnerException ?? ex;
                }
            }
        }
    }

    internal class BindingEvaluator : FrameworkElement
    {
        public object Value
        {
            get
            {
                //Value = DependencyProperty.;
                SetBinding(ValueProperty, _binding);
                return GetValue(ValueProperty);
            }
            private set { SetValue(ValueProperty, value); }
        }
        static readonly DependencyProperty ValueProperty =
            DependencyProperty.Create("Value", typeof(object), typeof(BindingEvaluator), null);

        readonly Binding _binding;
        public BindingEvaluator(Binding binding)
        {
            this._binding = binding;
        }
    }
}
