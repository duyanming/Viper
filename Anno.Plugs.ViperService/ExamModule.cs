/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/9/7 11:00:05 
Functional description： ExamModule
******************************************************/
using Anno.Const.Attribute;
using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anno.Plugs.ViperService
{
    public class ExamModule: BaseModule
    {
        public string SayHi(string name)
        {
            return $"Hi {name} I am Anno.";
        }
        public int Add(int x, int y)
        {
            return x + y;
        }
        public dynamic Dynamic()
        {
            return new { Name = "Anno", Age = 18 };
        }
        public object Object()
        {
            return new { Name = "Object", Age = 18 };
        }

        public dynamic Dyn()
        {
            return new ActionResult(true, new { Name = "Dyn", Age = 18 });
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "上传文件")]
        public dynamic UpLoadFile() {
            var file = Request<AnnoFile>("annoFile");
            var filePath = AppDomain.CurrentDomain.BaseDirectory;
            using (var stream = System.IO.File.Create(Path.Combine(filePath,file.FileName)))
            {
                stream.Write(file.Content,0,file.Length);
            }
            return new ActionResult(true, new { Msg = "上传成功", SourceId = 18 });
        }
    }
}
