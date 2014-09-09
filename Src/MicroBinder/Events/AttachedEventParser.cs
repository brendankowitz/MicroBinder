using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicroBinder.Events
{
    internal class AttachedEventParser
    {
        static readonly Regex parseMethod = new Regex(@"(?<method>[\w0-9+]+)\(?(?<parameters>[\w, ${}\.]+)*\)?");
        internal EventData Parse(string messages)
        {
            var results = new List<EventDataItem>();
            foreach (string item in messages.Split(';'))
            {
                var result = parseMethod.Matches(item);
                if (result.Count == 0)
                    throw new ArgumentException(string.Format("'{0}' couldn't be parsed.{1}Reference expression: {2}", item, Environment.NewLine, messages));

                var data = new EventDataItem();
                var method = result[0].Groups["method"].Value;

                if (method.Contains("_"))
                {
                    var splitVal = method.Split('_');
                    data.SourceEvent = splitVal[1];
                    data.TargetMethod = splitVal[0];
                }
                else
                {
                    data.SourceEvent = method;
                }

                var parameters = result[0].Groups["parameters"].Value;

                if (!string.IsNullOrEmpty(parameters))
                {
                    var split = parameters.Split(',');
                    data.Parameters = split.Select(param => param.Trim()).ToList();
                }

                results.Add(data);

            }
            return new EventData {EventItems = results.ToArray()};
        }
    }
}
