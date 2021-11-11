using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore
{
    public static class FileToBase64
    {
        public static string ToBase64(this Stream stream)
        {
            if (stream == null)
            {
                return null;
            }
            using BinaryReader objReader = new BinaryReader(stream);
            //读取文件内容
            byte[] byteFile = objReader.ReadBytes((int)stream.Length);
            string base64 = Convert.ToBase64String(byteFile);
            return base64;

        }

        public static string ToBase64(this IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            using Stream stream = file.OpenReadStream();
            using BinaryReader objReader = new BinaryReader(stream);
            //读取文件内容
            byte[] byteFile = objReader.ReadBytes((int)stream.Length);
            var afile = new AnnoFile()
            {
                ContentType = file.ContentType,
                ContentDisposition = file.ContentDisposition,
                Content = byteFile,
                FileName = file.FileName,
                Length = file.Length,
                Name = file.Name
            };
            //string base64 = Convert.ToBase64String(byteFile);
            string base64 = Newtonsoft.Json.JsonConvert.SerializeObject(afile);
            return base64;

        }
    }
    class AnnoFile
    {
        //
        // 摘要:
        //     Gets the raw Content-Type header of the uploaded file.
        public string ContentType
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the raw Content-Disposition header of the uploaded file.
        public string ContentDisposition
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the file length in bytes.
        public long Length
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the form field name from the Content-Disposition header.
        public string Name
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the file name from the Content-Disposition header.
        public string FileName
        {
            get; set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public byte[] Content { get; set; }
    }
}
