using Anno.EventBus;
using Anno.Domain.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.EventHandlers
{
    public class MailSend : IEventHandler<NoticeEvent>
    {
        public void Handler(NoticeEvent entity)
        {
            Console.WriteLine($"你好{entity.Name},{entity.Msg}");
        }
    }
    public class MailEnd : IEventHandler<NoticeEvent>
    {
        public void Handler(NoticeEvent entity)
        {
            Console.WriteLine($"消息发送完毕！");
        }
    }
}
