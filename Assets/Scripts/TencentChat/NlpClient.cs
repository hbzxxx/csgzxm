/*
 * Copyright (c) 2018 THL A29 Limited, a Tencent company. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace TencentCloud.Nlp.V20190408
{

   using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;
   using TencentCloud.Common;
   using TencentCloud.Common.Profile;
   using TencentCloud.Nlp.V20190408.Models;

   public class NlpClient : AbstractClient{

       private const string endpoint = "nlp.tencentcloudapi.com";
       private const string version = "2019-04-08";

        /// <summary>
        /// Client constructor.
        /// </summary>
        /// <param name="credential">Credentials.</param>
        /// <param name="region">Region name, such as "ap-guangzhou".</param>
        public NlpClient(Credential credential, string region)
            : this(credential, region, new ClientProfile())
        {

        }

        /// <summary>
        /// Client Constructor.
        /// </summary>
        /// <param name="credential">Credentials.</param>
        /// <param name="region">Region name, such as "ap-guangzhou".</param>
        /// <param name="profile">Client profiles.</param>
        public NlpClient(Credential credential, string region, ClientProfile profile)
            : base(endpoint, version, credential, region, profile)
        {

        }

  

    

        /// <summary>
        /// 闲聊服务基于腾讯领先的NLP引擎能力、数据运算能力和千亿级互联网语料数据的支持，同时集成了广泛的知识问答能力，可实现上百种自定义属性配置，以及儿童语言风格及说话方式，从而让聊天变得更睿智、简单和有趣。
        /// 
        /// </summary>
        /// <param name="req"><see cref="ChatBotRequest"/></param>
        /// <returns><see cref="ChatBotResponse"/></returns>
        public async Task<ChatBotResponse> ChatBot(ChatBotRequest req)
        {
             JsonResponseModel<ChatBotResponse> rsp = null;
             try
             {
                 var strResp = await this.InternalRequest(req, "ChatBot");
                 rsp = JsonConvert.DeserializeObject<JsonResponseModel<ChatBotResponse>>(strResp);
             }
             catch (JsonSerializationException e)
             {
                 throw new Exception(e.Message);
             }
             return rsp.Response;
        }

        /// <summary>
        /// 闲聊服务基于腾讯领先的NLP引擎能力、数据运算能力和千亿级互联网语料数据的支持，同时集成了广泛的知识问答能力，可实现上百种自定义属性配置，以及儿童语言风格及说话方式，从而让聊天变得更睿智、简单和有趣。
        /// 
        /// </summary>
        /// <param name="req"><see cref="ChatBotRequest"/></param>
        /// <returns><see cref="ChatBotResponse"/></returns>
        public ChatBotResponse ChatBotSync(ChatBotRequest req)
        {
             JsonResponseModel<ChatBotResponse> rsp = null;
             try
             {
                 var strResp = this.InternalRequestSync(req, "ChatBot");
                 rsp = JsonConvert.DeserializeObject<JsonResponseModel<ChatBotResponse>>(strResp);
             }
             catch (JsonSerializationException e)
             {
                 throw new Exception(e.Message);
             }
             return rsp.Response;
        }

       
    }
}
