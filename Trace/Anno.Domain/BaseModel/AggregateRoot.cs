using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;

namespace Anno.Domain.BaseModel
{
    public class AggregateRoot : AggregateRoot<long>
    {

    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>
    {
        /// <summary>
        /// 变化列表
        /// </summary>
        private Dictionary<string, ExpandoObject> _changeEvent = new Dictionary<string, ExpandoObject>();

        /// <summary>
        /// 返回事件列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ExpandoObject> GetChanges()
        {
            return _changeEvent;
        }
        /// <summary>
        /// 记录发出的事件
        /// </summary>
        /// <param name="table"></param>
        /// <param name="event"></param>
        protected void AddChange(string table, dynamic @event)
        {
            if (_changeEvent.ContainsKey(table))
            {
                _changeEvent[table]= PropertyMerge(_changeEvent[table], @event);
            }
            else
            {
                _changeEvent.Add(table, @event);
            }
        }
        /// <summary>
        /// 清空事件
        /// </summary>
        public void ClearChange()
        {
            _changeEvent = new Dictionary<string, ExpandoObject>();
        }

        /// <summary>
        /// dynamic  ExpandoObject 属性合并；
        /// 将event 的属性合并到 
        /// </summary>
        /// <param name="expando">根对象</param>
        /// <param name="event">新加入属性</param>
        /// <returns></returns>
        private ExpandoObject PropertyMerge(ExpandoObject expando,dynamic @event)
        {
            var dic = (IDictionary<string, object>)expando;
            var origin = @event.GetType().GetProperties();
            foreach (var item in origin)
            {
                dic[item.Name] = item.GetValue(@event);
            }
            return expando;
        }
    }
}
