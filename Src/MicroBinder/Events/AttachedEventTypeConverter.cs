using System;
using System.Globalization;
using Xamarin.Forms;

namespace MicroBinder.Events
{
    public class AttachedEventTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom (Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom (CultureInfo culture, object value)
        {
            var message = value as string;
            if(string.IsNullOrEmpty(message) == false)
            {
                return new AttachedEventMessage(message);
            }
            return null;
        }

    }
}