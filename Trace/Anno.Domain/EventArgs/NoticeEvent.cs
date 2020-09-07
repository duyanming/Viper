using Anno.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.EventArgs
{
    public class NoticeEvent:EventData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }
    }
}
