using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static DESCADA.Service.Charger;

namespace DESCADA.Service
{
    public class Local
    {
        public StationConfig Config;

        public Local(StationConfig config)
        {
            this.Config = config;
        }

        //借出电池鉴权(借用电池鉴权请求)
        ///emqx-interaction/station/v0.4.1/newbatt_auth_req
        /// 返回：
        //authResult 车辆鉴权结果,1：成功，0：失败
        //failReason  失败原因，成功时为空

        /// </summary>
        /// <returns></returns>
        public string newbatt_auth_req()
        {
            try
            {
                return "1";
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error:]newbatt_auth_req" + ex.Message.ToString());
                return "-1";
            }
        }


        //车载电池鉴权(归还电池鉴权请求)
        ///emqx-interaction/station/v0.4.1/oldbatt_auth_req <summary>
        /// emqx-interaction/station/v0.4.1/oldbatt_auth_req           
        /// 返回：
        //authResult 车辆鉴权结果,1：成功，0：失败
        //failReason  失败原因，成功时为空

        /// </summary>
        /// <returns></returns>
        public string oldbatt_auth_req()
        {
            try
            {
                return "1";
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error:]oldbatt_auth_req" + ex.Message.ToString());
                return "-1";
            }
        }


        //车辆请求进站
        /*
            deviceCode	设备编码		false	
            entryUniqueCode	进站唯一编号，由statioCode+swapChannel+time拼接而成		true	
            lpn	车牌号		true	
            stationCode	换电站编码		false	
            thoroughfare	换电站通道编号		true	
            time	车牌识别时间戳, yyyyMMddHHmmss

        返回：
        authResult	车辆鉴权结果,1：成功，0：失败
        failReason	失败原因，成功时为空

        pending  从本地车库中取验证
         */
        public string vehEnterReq()
        {
            //return "-1"; //debug
            string sql;
            try
            {
                sql = "select VIN from veh   WHERE 1=1";
                sql += "  and PlateNO<>'" + Global.plateNO + "'";
                string VIN = Global.dbTrans.ExecuteScalar0(sql).ToString();
                if (VIN != null && VIN != "")
                {
                    Global.VIN = VIN;
                    return "1";
                }
                else {
                    return "-1";
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error:]vehEnterReq" + ex.Message.ToString());
                return "-1";
            }
        }



    }
}
