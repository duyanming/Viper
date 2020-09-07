using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Anno.Const;
using Anno.Log;

namespace Anno.Redis
{

    /// <summary>   
    /// <author>
    ///        <name>杜燕明</name>
    ///        <date>2016.08.28</date>
    /// </author>
    /// </summary>
    public static class RedisHelper
    {

        private static readonly string Coonstr = RedisConfigure.Default().Conn;//bif.Const.Redis.RedisExchangeHosts
        private static object _locker = new Object();
        private static ConnectionMultiplexer _instance = null;
        /// <summary>
        /// 使用一个静态属性来返回已连接的实例，如下列中所示。这样，一旦 ConnectionMultiplexer 断开连接，便可以初始化新的连接实例。
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = ConnectionMultiplexer.Connect(Coonstr);
                            //注册如下事件
                            _instance.ConnectionFailed += MuxerConnectionFailed;
                            _instance.ConnectionRestored += MuxerConnectionRestored;
                            _instance.ErrorMessage += MuxerErrorMessage;
                            _instance.ConfigurationChanged += MuxerConfigurationChanged;
                            _instance.HashSlotMoved += MuxerHashSlotMoved;
                            _instance.InternalError += MuxerInternalError;
                            //_instance.PreserveAsyncOrder = false;
                        }
                    }
                }
                return _instance;
            }
        }
        static RedisHelper()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase()
        {
            return Instance.GetDatabase();
        }

        /// <summary>
        /// 这里的 MergeKey 用来拼接 Key 的前缀，具体不同的业务模块使用不同的前缀。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string MergeKey(string key)
        {
            return RedisConfigure.Default().Prefix + key;
        }
        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            key = MergeKey(key);
            return Deserialize<T>(GetDatabase().StringGet(key));
        }
        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            key = MergeKey(key);
            return Deserialize<object>(GetDatabase().StringGet(key));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        public static void Set(string key, object value, int expireMinutes = 0)
        {
            key = MergeKey(key);
            if (expireMinutes > 0)
            {
                GetDatabase().StringSet(key, Serialize(value), TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                GetDatabase().StringSet(key, Serialize(value));
            }
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            key = MergeKey(key);
            return GetDatabase().KeyExists(key);  //可直接调用
        }
        /// <summary>
        /// 设置有效期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public static bool KeyExpire(string key, int expireMinutes)
        {
            key = MergeKey(key);
            GetDatabase().KeyExpire(key, TimeSpan.FromMinutes(expireMinutes));
            return true;
        }
        /// <summary>
        /// Redis Keys模糊匹配
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IEnumerable<RedisKey> GetLikeKeys(string keys)
        {
            string[] cs = Coonstr.Split(':');
            if (cs.Length == 1)
            {
                cs[1] = "6379";
            }
            return GetServer(cs[0], Convert.ToInt32(cs[1])).Keys(pattern: RedisConfigure.Default().Prefix + keys + "*");
        }

        /// <summary>
        /// 模糊匹配 Key 个数
        /// </summary>
        /// <param name="keys">Key</param>
        /// <returns></returns>
        public static int GetLikeKeysCount(string keys)
        {
            return GetLikeKeys(keys).Count();
        }

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            key = MergeKey(key);
            return GetDatabase().KeyDelete(key);
        }
        /// <summary>
        /// 异步设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public static async Task SetAsync(string key, object value, int expireMinutes = 0)
        {
            key = MergeKey(key);
            if (expireMinutes > 0)
            {
                await GetDatabase().StringSetAsync(key, Serialize(value), TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                await GetDatabase().StringSetAsync(key, Serialize(value));
            }
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<object> GetAsync(string key)
        {
            key = MergeKey(key);
            object value = await GetDatabase().StringGetAsync(key);
            return value;
        }

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Increment(string key)
        {
            key = MergeKey(key);
            //三种命令模式
            //Sync,同步模式会直接阻塞调用者，但是显然不会阻塞其他线程。
            //Async,异步模式直接走的是Task模型。
            //Fire - and - Forget,就是发送命令，然后完全不关心最终什么时候完成命令操作。
            //即发即弃：通过配置 CommandFlags 来实现即发即弃功能，在该实例中该方法会立即返回，如果是string则返回null 如果是int则返回0.这个操作将会继续在后台运行，一个典型的用法页面计数器的实现：
            return GetDatabase().StringIncrement(key, flags: CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Decrement(string key, string value)
        {
            key = MergeKey(key);
            return GetDatabase().HashDecrement(key, value, flags: CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static string Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(memoryStream, o);
            //    byte[] objectDataAsStream = memoryStream.ToArray();
            //    return objectDataAsStream;
            //}
            return JsonConvert.SerializeObject(o);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        static T Deserialize<T>(string stream)
        {
            if (stream == null)
            {
                return default(T);
            }
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream(stream))
            //{
            //    T result = (T)binaryFormatter.Deserialize(memoryStream);
            //    return result;
            //}
            return JsonConvert.DeserializeObject<T>(stream);
        }
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
           Log.Log.Error("配置更改:" + e.EndPoint, typeof(RedisHelper));
        }
        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
           Log.Log.Error("发生错误时: " + e.Message, typeof(RedisHelper));
        }
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Log.Log.Error("重新建立连接之前的错误: " + e.EndPoint, typeof(RedisHelper));
        }
        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Log.Log.Error("重新连接: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)), typeof(RedisHelper));
        }
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Log.Log.Error("更改集群: " + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint, typeof(RedisHelper));
        }
        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Log.Log.Error("redis类库错误: " + e.Exception.Message, typeof(RedisHelper));
        }

        //场景不一样，选择的模式便会不一样，大家可以按照自己系统架构情况合理选择长连接还是Lazy。
        //建立连接后，通过调用ConnectionMultiplexer.GetDatabase 方法返回对 Redis Cache 数据库的引用。从 GetDatabase 方法返回的对象是一个轻量级直通对象，不需要进行存储。

        ///// <summary>
        ///// 使用的是Lazy，在真正需要连接时创建连接。
        ///// 延迟加载技术
        ///// 微软azure中的配置 连接模板
        ///// </summary>
        //private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        //{
        //    //var options = ConfigurationOptions.Parse(constr);
        //    ////options.ClientName = GetAppName(); // only known at runtime
        //    //options.AllowAdmin = true;
        //    //return ConnectionMultiplexer.Connect(options);
        //    ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect(Coonstr);
        //    muxer.ConnectionFailed += MuxerConnectionFailed;
        //    muxer.ConnectionRestored += MuxerConnectionRestored;
        //    muxer.ErrorMessage += MuxerErrorMessage;
        //    muxer.ConfigurationChanged += MuxerConfigurationChanged;
        //    muxer.HashSlotMoved += MuxerHashSlotMoved;
        //    muxer.InternalError += MuxerInternalError;
        //    return muxer;
        //});


        #region  当作消息代理中间件使用 一般使用更专业的消息队列来处理这种业务场景
        /// <summary>
        /// 当作消息代理中间件使用
        /// 消息组建中,重要的概念便是生产者,消费者,消息中间件。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static long Publish(string channel, string message)
        {
            ISubscriber sub = Instance.GetSubscriber();
            //return sub.Publish("messages", "hello");
            return sub.Publish(channel, message);
        }

        /// <summary>
        /// 在消费者端得到该消息并输出
        /// </summary>
        /// <param name="channelFrom">订阅管道</param>
        /// <param name="cb">订阅处理</param>
        public static void Subscribe(string channelFrom, Action<string> cb)
        {
            ISubscriber sub = Instance.GetSubscriber();
            sub.Subscribe(channelFrom, (channel, message) =>
            {
                cb(message);
                //Console.WriteLine((string)message);
            });
        }
        /// <summary>
        /// Redis发布订阅 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public static void Unsubscribe(string channel)
        {
            ISubscriber sub = Instance.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅 取消全部订阅
        /// </summary>
        public static void UnsubscribeAll()
        {
            ISubscriber sub = Instance.GetSubscriber();
            sub.UnsubscribeAll();
        }
        #endregion

        /// <summary>
        /// GetServer方法会接收一个EndPoint类或者一个唯一标识一台服务器的键值对
        /// 有时候需要为单个服务器指定特定的命令
        /// 使用IServer可以使用所有的shell命令，比如：
        /// DateTime lastSave = server.LastSave();
        /// ClientInfo[] clients = server.ClientList();
        /// 如果报错在连接字符串后加 ,allowAdmin=true;
        /// </summary>
        /// <returns></returns>
        public static IServer GetServer(string host, int port)
        {
            IServer server = Instance.GetServer(host, port);
            return server;
        }

        /// <summary>
        /// 获取全部终结点
        /// </summary>
        /// <returns></returns>
        public static EndPoint[] GetEndPoints()
        {
            EndPoint[] endpoints = Instance.GetEndPoints();
            return endpoints;
        }

    }
}
