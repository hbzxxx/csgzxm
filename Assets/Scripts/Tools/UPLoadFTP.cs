using COSXML;
using COSXML.Auth;
using COSXML.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace UPLoadFTP
{
    class UpLoadFiles
    {
        private static string FTPCONSTR = "ftp://101.35.87.105:21/";//FTP的服务器地址格式(ftp://192.168.1.234/)。ip地址和端口换成自己的，这些建议写在配置文件中，方便修改
        private static string FTPUSERNAME = "XXX";//我的FTP服务器的用户名
        private static string FTPPASSWORD = "XXX";//我的FTP服务器的密码
        public static float uploadProgress;//上传进度
        public static float downloadProgress;//下载进度

        static Task ret1;
        static string bucket = @"immortal-1258231114";

        static Task ret2;


        public static void OnUploadArchive(string filePath,string name)
        {
            Debug.Log($"[云存档] 开始上传，文件路径: {filePath}, 存档名: {name}");
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[云存档] 上传失败：文件不存在 {filePath}");
                return;
            }
            if (UpLoadFiles.ret1 == null
                    || UpLoadFiles.ret1.IsCompleted)
            {
                ret1 = UpLoadFiles.OnUploadAsync(filePath,name);
            }
            else
            {
                Debug.LogError("[云存档] 上传失败：上一次上传任务尚未完成");
            }
        }
        public static async Task OnUploadAsync(string filePath,string name)
        {
            // 腾讯云 SecretId
            string secretId = "AKIDxkHYYHitnQmrfCSRAcmfIeiV2IHyPZR2";
            // 腾讯云 SecretKey
            string secretKey = "LQElQqzrSnreAPO6XhaDZeT0GFxPaU7T";
            // 存储桶所在地域
            string region = "ap-guangzhou";

            // 普通初始化方式
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region)
                .SetDebugLog(true)
                .Build();

            // TCE 初始化方式
            // string domain = "your.domain";  // 替换成您的 Domain
            // string endpoint = String.Format("cos.{0}.{1}", region, domain);
            // CosXmlConfig config = new CosXmlConfig.Builder()
            //     .setEndpointSuffix(endpoint)
            //     .SetRegion(region)
            //     .SetDebugLog(true)
            //     .Build();

            long keyDurationSecond = 600;
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, keyDurationSecond);

            // service 初始化完成
            CosXmlServer cosXml = new CosXmlServer(config, qCloudCredentialProvider);

            try
            {


                // 上传对象
                Console.WriteLine(" ======= Put Object ======");
                string cosKey = await PutObject(cosXml, filePath,name);

            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx.Message);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
            }


            Console.WriteLine(" ======= Program End. ======");
            if(!Game.Instance.gameEntered)
            PanelManager.Instance.OpenFloatWindow("上传成功，请勿重复点击");
        }

        static async Task<String> PutObject(CosXmlServer cosXml, string path,string name)
        {
            string cosKey =name;
            //.cssg-snippet-body-start:[transfer-upload-file]
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            //对象在存储桶中的位置标识符，即称对象键
            String cosPath = cosKey;
            //本地文件绝对路径
            String srcPath = path;

            // 上传对象
            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, cosPath);
            uploadTask.SetSrcPath(srcPath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
            };

            try
            {
                COSXML.Transfer.COSXMLUploadTask.UploadTaskResult result = await
                    transferManager.UploadAsync(uploadTask);
                Console.WriteLine(result.GetResultInfo());
                string eTag = result.eTag;
            }
            catch (Exception e)
            {
                Debug.LogError("上传失败，请勿重复点击: " + e);
            }
            EventCenter.Broadcast(TheEventType.SuccessUploadArchive);

            return cosKey;
        }


        #region 本地文件上传到FTP服务器
        /// <summary>
        /// 文件上传到ftp
        /// </summary>
        /// <param name="ftpPath">存储上传文件的ftp路径</param>
        /// <param name="localPath">上传文件的本地目录</param>
        /// <param name="fileName">上传文件名称</param>
        /// <returns></returns>
        public static bool UploadFiles(string ftpPath, string localPath, string fileName)
        {
            //path = "ftp://" + UserUtil.serverip + path;
            string erroinfo = "";//错误信息
            float percent = 0;//进度百分比
            FileInfo f = new FileInfo(localPath);
            localPath = localPath.Replace("\\", "/");
            bool b = MakeDir(ftpPath);
            if (b == false)
            {
                return false;
            }
            localPath = FTPCONSTR + ftpPath + "/" + fileName;
            Debug.Log(localPath);
            FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(localPath));
            //reqFtp.UseBinary = true;//代表可以发送图片
            //reqFtp.Credentials = new NetworkCredential();
            //reqFtp.KeepAlive = false;//在请求完成之后是否关闭到 FTP 服务器的控制连接
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;//表示将文件上载到 FTP 服务器的 FTP STOR 协议方法
            reqFtp.ContentLength = f.Length;//本地上传文件的长度
            int buffLength = 2048;//缓冲区大小
            byte[] buff = new byte[buffLength];//缓冲区
            int contentLen;//存放读取文件的二进制流
            FileStream fs = f.OpenRead(); //以只读方式打开一个文件并从中读取。
                                          //用于计算进度
            int allbye = (int)f.Length;
            int startbye = 0;
            try
            {
                Stream strm = reqFtp.GetRequestStream();//将FtpWebRequest转换成stream类型
                contentLen = fs.Read(buff, 0, buffLength);//存放读取文件的二进制流
                                                          //进度条
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    startbye = contentLen + startbye;
                    percent = startbye / allbye * 100;
                    if (percent <= 100)
                    {
                        uploadProgress = percent;

                    }
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                //释放资源
                strm.Close();
                fs.Close();
                erroinfo = "完成";
                return true;
            }
            catch (Exception ex)
            {
                erroinfo = string.Format("无法完成上传" + ex.Message);
                Debug.Log(erroinfo);
                return false;
            }
        }
        #endregion

        #region 在ftp服务器上创建文件目录
        /// <summary>
        ///在ftp服务器上创建文件目录
        /// </summary>
        /// <param name="dirName">文件目录</param>
        /// <returns></returns>
        public static bool MakeDir(string dirName)
        {
            try
            {
                string uri = (FTPCONSTR + dirName);
                if (DirectoryIsExist(uri))
                {
                    return true;
                }

                string url = FTPCONSTR + dirName;
                FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(url));
                //reqFtp.UseBinary = true;

                // reqFtp.KeepAlive = false;
                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                //reqFtp.Credentials = new NetworkCredential();

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("因{0},无法下载" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 判断ftp上的文件目录是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>        
        public static bool DirectoryIsExist(string uri)
        {
            string[] value = GetFileList(uri);
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static string[] GetFileList(string uri)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                //uri = "ftp://101.35.87.105:21/test";
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(uri);


                //reqFTP.UseBinary = true;
                //reqFTP.UsePassive = false;
                //reqFTP.Credentials = new NetworkCredential();
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;

                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        #endregion


        /// <summary>
        /// 下载存档
        /// </summary>
        /// <param name="name"></param>
        public static void OnDownloadArchive(string path,string name)
        {
            if (UpLoadFiles.ret2 == null
                  || UpLoadFiles.ret2.IsCompleted)
            {
                ret2 = UpLoadFiles.OnDownloadAsync(path,name);
            }
        }

        /// <summary>
        /// 下载存档
        /// </summary>
        /// <returns></returns>
        public static async Task OnDownloadAsync(string path,string name)
        {
            // 腾讯云 SecretId
            string secretId = "AKIDxkHYYHitnQmrfCSRAcmfIeiV2IHyPZR2";
            // 腾讯云 SecretKey
            string secretKey = "LQElQqzrSnreAPO6XhaDZeT0GFxPaU7T";
            // 存储桶所在地域
            string region = "ap-guangzhou";

            // 普通初始化方式
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region)
                .SetDebugLog(true)
                .Build();


            long keyDurationSecond = 600;
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, keyDurationSecond);

            // service 初始化完成
            CosXmlServer cosXml = new CosXmlServer(config, qCloudCredentialProvider);

            try
            {


                // 上传对象
                Console.WriteLine(" ======= Get Object ======");
                  await GetObject(cosXml,name,path);

            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Debug.LogError("CosClientException: " + clientEx.Message);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Debug.LogError("CosServerException: " + serverEx.GetInfo());
            }


            Console.WriteLine(" ======= Program End. ======");
        }


        static async Task GetObject(CosXmlServer cosXml, string cosKey,string path)
        {
            bool success = true;
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            //对象在存储桶中的位置标识符，即称对象键
            String cosPath = cosKey;
            //本地文件夹
            string localDir = path;
            //指定本地保存的文件名
            string localFileName = "GameInfo.es3";

            // 下载对象
            COSXMLDownloadTask downloadTask = new COSXMLDownloadTask(bucket, cosPath,
                localDir, localFileName);

            downloadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
            };

            try
            {
                COSXML.Transfer.COSXMLDownloadTask.DownloadTaskResult result = await
                    transferManager.DownloadAsync(downloadTask);
                Console.WriteLine(result.GetResultInfo());
                string eTag = result.eTag;
            }
            catch (Exception e)
            {
                success = false;
                PanelManager.Instance.OpenFloatWindow("下载失败"+e);

                Console.WriteLine("CosException: " + e);
            }
            if (success)
            {
                PanelManager.Instance.OpenFloatWindow("下载成功，请勿重复点击");
                ArchiveManager.Instance.LoadAllArchive();
            }

            EventCenter.Broadcast(TheEventType.SuccessDownloadArchive);

        }
    }
}
