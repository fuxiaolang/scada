using DESCADA.Service;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using MQTTnet.Extensions.ManagedClient;
using System.Windows.Markup;

namespace DESCADA
{
    public class MqttClientService
    {
        public static IMqttClient _mqttClient;
        private MqttClientOptions clientOptions;

        //测试managedclient，不用测试managedclient，断网恢复也会自动重连
        public async Task MqttClientStart_new()
        {
            var mqttFactory = new MqttFactory();

            using (var managedMqttClient = mqttFactory.CreateManagedMqttClient())
            {
                clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("1.2.3.4", 1883)  // 39.106.254.86 172.16.11.78 //.WithTcpServer("59.110.166.219", 1883) // 要访问的mqtt服务端ecocloud.huamaitech.com 59.110.166.219的 ip 和 端口号  .WithTcpServer("192.168.10.78", 1883) 172.16.11.78
                    .WithCredentials("admin", "public") // 要访问的mqtt服务端的用户名和密码
                    .WithClientId("testclient02") // 设置客户端id
                    .WithCleanSession()
                    .Build();

                //var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                //    .WithClientOptions(clientOptions)
                //    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                //    .Build();

                //await managedMqttClient.StartAsync(managedMqttClientOptions);

                // The application message is not sent. It is stored in an internal queue and
                // will be sent when the client is connected. Payload:消息体
                // managedMqttClient.EnqueueAsync("Topic", "Payload");
                //Console.WriteLine("The managed MQTT client is connected.");
                // Wait until the queue is fully processed.
                //System.Threading.SpinWait.SpinUntil(() => managedMqttClient.PendingApplicationMessagesCount == 0, 10000);
                // Console.WriteLine($"Pending messages = {managedMqttClient.PendingApplicationMessagesCount}");
            }

            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync; // 客户端连接成功事件
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync; // 客户端连接关闭事件
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync; // 收到消息事件
            _mqttClient.ConnectAsync(clientOptions);


        }

        public void MqttClientStart()
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("1.2.3.4", 1883)  // 39.106.254.86 172.16.11.78
                                                       //.WithTcpServer("59.110.166.219", 1883) // 要访问的mqtt服务端ecocloud.huamaitech.com 59.110.166.219的 ip 和 端口号  .WithTcpServer("192.168.10.78", 1883) 172.16.11.78
                .WithCredentials("admin", "public") // 要访问的mqtt服务端的用户名和密码
                .WithClientId("testclient02") // 设置客户端id
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false  // 是否使用 tls加密
                });

            clientOptions = optionsBuilder.Build();
            _mqttClient = new MqttFactory().CreateMqttClient();

            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync; // 客户端连接成功事件
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync; // 客户端连接关闭事件
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync; // 收到消息事件



            _mqttClient.ConnectAsync(clientOptions);

            //System.Windows.Forms.MessageBox.Show(_mqttClient.IsConnected.ToString());


        }

        //订阅主题 暂未用,  客户端连接成功事件 =_mqttClient.SubscribeAsync
        public async Task SubscribeAsync(string topic)
        {
            //string payload = "hello,from pascalming!";
            //string mqttTopic = "/topic/pascalming/v1";
            //var applicationMessage = new MqttApplicationMessageBuilder()
            //    .WithTopic(mqttTopic)
            //    .WithPayload(payload)
            //    .Build();
            //Task<MqttClientSubscribeResult> task = mymqtt.mqttClient.SubscribeAsync(mymqtt.topicStr);
            //task.Wait();


            var subResult = await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            //if (subResult.ReasonString != MQTTnet.Protocol.MqttSubscribeReasonCode.GrantedQoS0)
            //{
            //    throw new InvalidOperationException("Subscription failed with return code " + subResult.ReasonCode);
            //}
            Console.WriteLine("Subscribed to topic: " + topic);
        }



        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine($"客户端已断开与服务端的连接……");
            // return Task.CompletedTask;
            Global.CloudConnStatus = -1;
            return _mqttClient.ConnectAsync(clientOptions);
        }

        /// <summary>
        /// 客户端连接成功事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine($"客户端已连接服务端……");
            Global.CloudConnStatus = 1;
            // 订阅消息主题
            // MqttQualityOfServiceLevel: （QoS）:  0 最多一次，接收者不确认收到消息，并且消息不被发送者存储和重新发送提供与底层 TCP 协议相同的保证。
            // 1: 保证一条消息至少有一次会传递给接收方。发送方存储消息，直到它从接收方收到确认收到消息的数据包。一条消息可以多次发送或传递。
            // 2: 保证每条消息仅由预期的收件人接收一次。级别2是最安全和最慢的服务质量级别，保证由发送方和接收方之间的至少两个请求/响应（四次握手）。

            var rtn=_mqttClient.SubscribeAsync("station_downstream/+/+", MqttQualityOfServiceLevel.AtLeastOnce); //topic_02

            return Task.CompletedTask;
        }

        /// <summary>
        /// 收到消息事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string topic = arg.ApplicationMessage.Topic;
            string msg = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            //string msgs = "{\"data\":\"{\\\"cmdType\\\":1,\\\"policyNo\\\":\\\"1234\\\",\\\"reqId\\\":\\\"1111333322201\\\",\\\"reqTime\\\":\\\"20240320170955\\\",\\\"uniqId\\\":\\\"llop1111101\\\"}\",\"deviceCode\":\"333-cc\",\"encryp\":0,\"id\":\"b11344f25b29ab427fe34ac5becf3ace\",\"sendTime\":\"20240326163601\",\"seq\":0,\"type\":\"4004\"}";

            try
            {
                dynamic datas = JsonConvert.DeserializeObject(msg);
                string data = datas.data;
                string deviceCode = datas.deviceCode;
                string encryp = datas.encryp;
                string id = datas.id;
                string sendTime = datas.sendTime;
                string seq = datas.seq;
                string type = datas.type; 
                //4开头的都是云端下发的
                switch (type)
                {
                    //换电允许指令
                    case "4001":
                        handle4001(data);
                        break;
                    //充电时段策略指令 20240412 不处理了， 不管上位机是否设置分时段计费策略（CMD1103/1105），CMD202/222上报充电记录时都会按固定的48时段上报每半小时内的充电电量统计明细。
                    case "4003":
                        //handle4003(data);
                        break;
                    //插枪充电请求（4004，downstream）
                    case "4004":
                        handle4004(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("Error MQTT Rec: "+ ex.Message + "\r\n" +ex.StackTrace);
            }

            Global.WriteLog("[Error  _mqttClient]" + "ApplicationMessageReceivedAsync：客户端ID=【" + arg.ClientId + "】接收到消息。 Topic主题=【" + arg.ApplicationMessage.Topic + " 】 消息=【" + Encoding.UTF8.GetString(arg.ApplicationMessage.Payload) + "】 qos等级=【" + arg.ApplicationMessage.QualityOfServiceLevel + "】");
            return Task.CompletedTask;
        }

        //换电允许指令（4001，downstream）
        public void handle4001(string data)
        {
            dynamic detail = JsonConvert.DeserializeObject(data);
            string entryUniqueCode = detail.entryUniqueCode;  //进站唯一编号
            string thoroughfare = detail.thoroughfare;        //换电站通道编号  pending 多通道时需要修改
            string lpn = detail.lpn;    //车牌号
            string resCode = detail.resCode;      //反馈码，200：允许换电；其他：异常

            if (Global.plateNO == lpn && Global.VehInNO == entryUniqueCode && resCode=="200")
            {
                Global.AllowSwitch=true;
            }

            //回复300D
            string rtnCode = "200";
            if (Global.PLCMsg5.RobotStatus == 4 || Global.PlcConnStatus == -1) rtnCode = "异常";
            Publish300D(rtnCode);
        }


        //转发云端充电时段策略指令（4003，downstream）到7个充电机cmd1103
        public void handle4003(string data)
        {
            //string msgs = "{\"data\":\"{\\\"periodStrategyTimes\\\":[{\\\"endTime\\\":\\\"00:30\\\",\\\"startTime\\\":\\\"02:30\\\"},{\\\"endTime\\\":\\\"03:30\\\",\\\"startTime\\\":\\\"04:30\\\"}],\\\"policyNo\\\":\\\"asjdksja1\\\"}\",\"deviceCode\":\"321-c\",\"encryp\":0,\"id\":\"ad5782db0b8f10a5cdfe3441a64b4d3e\",\"sendTime\":\"20240327174759\",\"seq\":0,\"type\":\"4003\"}";
            dynamic detail = JsonConvert.DeserializeObject(data);
            string policyNo = detail.policyNo;
            Global.ChargerPolicyNo = policyNo; //固定10位
            var periodStrategyTimes = (JArray)detail.periodStrategyTimes;

            Byte[] MsgData = new byte[60]; //12个费率段 每段5个字节数据；默认值0
            int k = 0;

            bool formatChecked = true;

            try
            {
                for (var i = 0; i < periodStrategyTimes.Count; i++)
                {
                    var tmpObj = (JObject)periodStrategyTimes[i];
                    string startTime = (string)tmpObj["startTime"].ToString();
                    string endTime = (string)tmpObj["endTime"].ToString();

                    //格式校验
                    string[] parts1 = startTime.Split(':');
                    int hours1 = int.Parse(parts1[0]);
                    int minutes1 = int.Parse(parts1[1]);
                    string[] parts2 = endTime.Split(':');
                    int hours2 = int.Parse(parts2[0]);
                    int minutes2 = int.Parse(parts2[1]);
                    if (!(hours1 >= 0 && hours1 <= 24 && (minutes1 == 0 || minutes1 == 30) &&
                        hours2 >= 0 && hours2 <= 24 && (minutes2 == 0 || minutes2 == 30)
                        ))
                    {
                        formatChecked = false; break;
                    }
                    TimeSpan t1 = new TimeSpan(0, hours1, minutes1, 0);
                    TimeSpan t2 = new TimeSpan(0, hours2, minutes2, 0);
                    if (t1 >= t2)
                    {
                        formatChecked = false;break;
                    }

                    MsgData[k] = Convert.ToByte(startTime.Substring(0, 2));
                    MsgData[k + 1] = Convert.ToByte(startTime.Substring(3,2));
                    MsgData[k + 2] = Convert.ToByte(endTime.Substring(0, 2));
                    MsgData[k + 3] = Convert.ToByte(endTime.Substring(3,2));
                    MsgData[k + 4] = 0; //费率平台控，填0
                    k += 5;

                }
            }
            catch (Exception ex)
            {
                formatChecked = false;
                Global.WriteLog("[Error]handle4003:"+ ex.Message + "\r\n" +ex.StackTrace);
            }

            if (formatChecked)
            {
                //到7个充电机cmd1103
                for (int j = 1; j < 8; j++)
                {
                    Global.chargerServer.send1103(j, MsgData);
                }
            } else{
                //直接回复云端校验失败
                Global.chargerServer.Publish300C(-2, "格式错误");

            }
        }

        //转发云端启动停止充电机指令
        public void handle4004(string data)
        {
            // string msgs = "{\"data\":\"{\\\"cmdType\\\":1,\\\"policyNo\\\":\\\"1234\\\",\\\"reqId\\\":\\\"1111333322201\\\",\\\"reqTime\\\":\\\"20240320170955\\\",\\\"uniqId\\\":\\\"llop1111101\\\"}\",\"deviceCode\":\"333-cc\",\"encryp\":0,\"id\":\"b11344f25b29ab427fe34ac5becf3ace\",\"sendTime\":\"20240326163601\",\"seq\":0,\"type\":\"4004\"}";
            dynamic detail = JsonConvert.DeserializeObject(data);
            int cmdType = detail.cmdType;    //命令类型，1-启动；2-停止；
            string policyNo = detail.policyNo;  //时段策略编号
            string reqId = detail.reqId;        //请求充电订单号
            string reqTime = detail.reqTime;    //请求时间
            string uniqId = detail.uniqId;      //铭牌编码  站端需维护映射表
            byte GunId = detail.connectorId;      //枪口号 test
            int chargerID = Global.transChargeID(uniqId); 



            if (chargerID == -1)
            {
                //云端不能启动内枪1、2，否则直接回执错误；
                Global.chargerServer.Publish300A_Err(cmdType, chargerID, GunId, "铭牌码不匹配");
                return;

            }
            if (chargerID == 1 || chargerID == 2)
            {
                //云端不能启动内枪1、2，否则直接回执错误；
                Global.chargerServer.Publish300A_Err(cmdType, chargerID, GunId, "不允许启动1号枪2号枪");
                return;

            }
            //如仓内已在充电则不允许启动插枪充电（300A回复充电占用）
            if (Global.chargerServer.IsUsing12(chargerID))
            {
                Global.chargerServer.Publish300A_Err(cmdType, chargerID, GunId, "充电占用");
                return;
            }

            Global.ChargerReqId[chargerID, GunId] = reqId;//请求充电订单号 string 启停使用同一个订单号
            Global.ChargerReqpolicyNo[chargerID, GunId] = policyNo;


            //下发执行指令（充电CMD5/停充CMD7）给充电机
            if (cmdType == 1)
            {
                //1-启动
                Global.chargerServer.StartByCloudCharge(chargerID, GunId, reqId);
                Global.ChargerStartApplyFlag[chargerID, GunId] = 1; //0 未申请 1 已申请 2 已回执
                Global.ChargerStartApplyFlagB[chargerID, GunId] = 0;   //   0 未回执B  1 已回执B 
                Global.ChargerStartApplyType[chargerID, GunId] = 1; //1 启动 2 停止
                Global.ChargerStartApplyTime[chargerID, GunId] = DateTime.Now;

            }
            else if (cmdType == 2)
            {
                //2-停止
                Global.chargerServer.Stop(chargerID, GunId);
                Global.ChargerStartApplyFlag[chargerID, GunId] = 1; //0 未申请 1 已申请 2 已回执
                Global.ChargerStartApplyFlagB[chargerID, GunId] = 0;
                Global.ChargerStartApplyType[chargerID, GunId] = 2; //1 启动 2 停止
                Global.ChargerStartApplyTime[chargerID, GunId] = DateTime.Now;

            }


            //15S 300a超时回 excuteFlg=3
        }

        int k = 0;
        //上发云端消息，失败（断电Exception或有错误回执-1)的话，要落库，并补发20240407
        public void Publish(string msgType,string data,int passk=0)
        {
            string OperatorID= Global.config.NomalConfig.OperatorNo; //"48CQCDC3N"
            //k++;
            //Global.WriteLog("msgType:"+ msgType + "[info k]" + k +" passk:"+ passk);
            var message = new MqttApplicationMessage
            {
                Topic = "station_upstream/${"+ OperatorID + "}",
                Payload = Encoding.Default.GetBytes(data),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                Retain = true  // 服务端是否保留消息。true为保留，如果有新的订阅者连接，就会立马收到该消息。
            };
            try
            {
                if (Global.CloudConnStatus == 1)
                {
                    Global.WriteLog("[info 正常发送]" + k + " passk:" + passk);
                    _mqttClient.PublishAsync(message);
                }
                else {
                    Global.WriteLog("[info 失连发送]" + k + " passk:" + passk);
                    Global.dbTrans.CreateMsgWait(2, msgType, data, "云端未连接");
                }
              
            }
            catch (Exception ex)
            {
                Global.WriteLog("[info 异常发送]" + k + " passk:" + passk+ ex.Message + ex.StackTrace);
                Global.dbTrans.CreateMsgWait(2, msgType, data,ex.Message+ex.StackTrace);
            }
        }


        //300C 换电允许指令站端接收应答（300D，upstream）
        //站端收到4001指令后应返回应答消息，平台在超时时间内未收到300D时应考虑重发4001，若收到异常状态应答应考虑合适的处理应对；
        public void Publish300D(string resCode="200")
        {
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string msgID = Guid.NewGuid().ToString();
                //MQTT send to cloud
                string stepDescribe = Global.PLCMsg5.ExchangeStatus.ToString();
                string msg = "  { ";
                msg += "\"header\":{";
                msg += "\"id\":\"" + msgID + "\",";
                msg += "\"type\":\"300D\",";
                msg += "\"deviceCode\":\"102A\",";
                msg += "\"sendTime\":\"" + timeStamp + "\",";
                msg += "\"seq\":0,";
                msg += "\"encryp\":0";
                msg += "},";

                msg += "\"data\":{";

                msg += "\"entryUniqueCode\": \"" + Global.VehInNO + "\",";
                msg += "\"resCode\": \"" + resCode + "\"";

                msg += "                        },";

                msg += "\"sig\":\"  \" ";
                msg += "}";

                Global.mgttClientService.Publish( "300D", msg);
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error Publish300D]"+ ex.Message + "\r\n" +ex.StackTrace);
            }
        }


        //消防监控（300E，upstream）
        public void Publish300E(Byte[] FireMsgA)
        {
            try
            {
                int probeId = FireMsgA[6];
                int alarmLevel = FireMsgA[7];
                double co = FireMsgA[8]*0.1;
                int temp = FireMsgA[9]-40;
                int somkeState = FireMsgA[10];
                int workState = FireMsgA[11];
                string strMsg = BitConverter.ToString(FireMsgA);

                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string msgID = Guid.NewGuid().ToString();
                //MQTT send to cloud
                string stepDescribe = Global.PLCMsg5.ExchangeStatus.ToString();
                string msg = "  { ";
                msg += "\"header\":{";
                msg += "\"id\":\"" + msgID + "\",";
                msg += "\"type\":\"300E\",";
                msg += "\"deviceCode\":\"102A\",";
                msg += "\"sendTime\":\"" + timeStamp + "\",";
                msg += "\"seq\":0,";
                msg += "\"encryp\":0";
                msg += "},";

                msg += "\"data\":{";

                msg += "\"probeId\": \"" + probeId + "\",";
                msg += "\"probeName\": \"" + "" + "\",";
                msg += "\"alarmLevel\": \"" + alarmLevel + "\",";
                msg += "\"co\": \"" + co + "\",";
                msg += "\"temp\": \"" + temp + "\",";
                msg += "\"somkeState\": \"" + somkeState + "\",";
                msg += "\"workState\": \"" + workState + "\",";
                msg += "\"updateTime\": \"" + timeStamp + "\"";
                msg += "                        },";

                msg += "\"sig\":\"  \" ";
                msg += "}";

                Global.mgttClientService.Publish("300D", msg);
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error Publish300D]" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        public void Publish3010(int process, int objectType = 1)
        {
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string msgID = Guid.NewGuid().ToString();
                //MQTT send to cloud
                string stepDescribe = Global.PLCMsg5.ExchangeStatus.ToString();
                string msg = "  { ";
                msg += "\"header\":{";
                msg += "\"id\":\"" + msgID + "\",";
                msg += "\"type\":\"3010\",";
                msg += "\"deviceCode\":\"" + Global.config.NomalConfig.RemoteConfig.RemoteCode + "\",";

                msg += "\"sendTime\":\"" + timeStamp + "\",";
                msg += "\"seq\":0,";
                msg += "\"encryp\":0";
                msg += "},";

                msg += "\"data\":{";

                msg += "\"objectType\":\"" + objectType + "\",";
                msg += "\"process\":\"" + process + "\"";

                msg += "                        },";

                msg += "\"sig\":\"\"";
                msg += "}";
                Global.WriteDebugAsync("Publish3010:" + msg, "MQTT");

                Global.mgttClientService.Publish("3010", msg);
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error Publish300E]" + ex.Message + "\r\n" + ex.StackTrace, "MQTT");
            }
        }

        public void Publish300F(int objectType = 1)
        {
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string msgID = Guid.NewGuid().ToString();
                //MQTT send to cloud
                string stepDescribe = Global.PLCMsg5.ExchangeStatus.ToString();
                string msg = "  { ";
                msg += "\"header\":{";
                msg += "\"id\":\"" + msgID + "\",";
                msg += "\"type\":\"300F\",";
                msg += "\"deviceCode\":\"" + Global.config.NomalConfig.RemoteConfig.RemoteCode + "\",";

                msg += "\"sendTime\":\"" + timeStamp + "\",";
                msg += "\"seq\":0,";
                msg += "\"encryp\":0";
                msg += "},";

                msg += "\"data\":{";

                msg += "\"objectType\":\"" + objectType + "\"";
                msg += "                        },";

                msg += "\"sig\":\"\"";
                msg += "}";
                Global.WriteDebugAsync("Publish300F:" + msg, "MQTT");

                Global.mgttClientService.Publish("300F", msg);
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error Publish300E]" + ex.Message + "\r\n" + ex.StackTrace, "MQTT");
            }
        }
        //json 消息体 1:成功 -1失败
        public string PublishMQMsgWait(string msg, string msgType,  string msgID)
        {
            string OperatorID = Global.config.NomalConfig.OperatorNo; //"48CQCDC3N"


            var message = new MqttApplicationMessage
            {
                Topic = "station_upstream/${" + OperatorID + "}",
                Payload = Encoding.Default.GetBytes(msg),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                Retain = true  // 服务端是否保留消息。true为保留，如果有新的订阅者连接，就会立马收到该消息。
            };
            try
            {
                _mqttClient.PublishAsync(message);

                string sql = "update msgwait set SendFlag=1,sendtime=NOW() where id=" + msgID;
                Global.dbTrans.ExcuteScript0(sql);
                return "1";
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error:]PublishMQMsgWait,msgID:"+ msgID+" ," + ex.Message.ToString() + ex.StackTrace);
                return "-1";
            }
        }
    }
}
