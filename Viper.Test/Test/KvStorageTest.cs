/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/10/27 13:51:56 
Functional description： KvStorageTest
******************************************************/
using Anno.Rpc.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Viper.Test
{
    using Anno.Rpc.Storage;
    public class KvStorageTest
    {
        public KvStorageTest() {
            Init();
        }
        public void Handle() {
            using (KvStorageEngine kvEngine = new KvStorageEngine())
            {
                var rlt = kvEngine.Set("viper", "Viper 你好啊！");
                var getViper = kvEngine.Get("viper");
                var rltobj = kvEngine.Set("12", new ViperTest() { Id = 12, Name = "Viper" });
                var getobj = kvEngine.Get<ViperTest>("12");
            }
        }

        void Init()
        {
            DefaultConfigManager.SetDefaultConfiguration("KvStorage", "127.0.0.1", 7010, false);
        }
    }
    public class ViperTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
