using System;
using System.Collections.Generic;

namespace MicroBinder.Events
{
    public class EventDataItem
    {
        public string TargetMethod { get; set; }
        public IEnumerable<string> Parameters { get; set; }
        public string SourceEvent { get; set; }
        public Delegate CachedDelegate { get; set; } 
    }
}