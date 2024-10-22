using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using Microsoft.VisualStudio.PlatformUI;
using ZXing.Common;
using System.Data.SqlTypes;
using DESCADA.Service;
using System.Timers;
using static System.Net.WebRequestMethods;
using System.Windows.Interop;
using MySqlConnector;
using System.Threading.Tasks;

namespace DESCADA
{
    public class DBTrans
    {

        //MySqlConnection connect = new MySqlConnection(this.ConectString);
        /// <summary>
        /// 数据库连接对象  
        /// </summary>
        public MySqlConnection Connection
        {
            get
            {
                if (this._Connection == null)
                    this._Connection = new MySqlConnection(this.ConectString);
                return this._Connection;
            }
            set { this._Connection = value; }
        }
        private MySqlConnection _Connection = null;

        /// <summary>
        /// 是否已连到数据库
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (this.Connection == null ||//连接未初始或未打开
                    Connection.State == ConnectionState.Closed)
                {
                    Global.DBConnStatus = -1;
                    return false;
                }
                else
                {
                    Global.DBConnStatus = 1;
                    return true;
                }
            }
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns>true 连接成功   --false 连接失败，请查看LastError</returns>
        public bool Open()
        {
            if (IsConnected)
                return true;

            try
            {
                this.Connection.Open();
                this._IsInTransaction = false;
                Global.DBConnStatus = 1;
                return true;
            }
            catch (Exception ex)
            {
                Global.DBConnStatus = -1;

                this.LastError = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 关闭连接,注：当数据库还处于事务中时，系统自动提交。
        /// </summary>
        public void Close()
        {
            try
            {
                if (this.IsInTransaction)
                    this.CommitTran();
                this.Connection.Close();
            }
            catch (Exception exp)
            {
                this.LastError = exp.ToString();
            }
            return;
        }

        #region 错误信息
        /// <summary>
        /// 错误信息
        /// </summary>
        public string LastError
        {
            get { return this._lastError; }
            set { this._lastError = value; }
        }
        private string _lastError = "";
        #endregion


        #region 处理数据库事务
        /// <summary>
        /// 数据库事务
        /// </summary>
        public MySqlTransaction Transaction
        {
            get { return this._Transaction; }

        }
        private MySqlTransaction _Transaction;

        /// <summary>
        /// 是否在一个事务中
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return _IsInTransaction;
            }
        }
        private bool _IsInTransaction = false;
        /// <summary>
        /// 方法CommitTran--提交当前事务
        /// </summary>
        /// <returns>
        ///	return --true 成功   --false 失败
        /// </returns>
        public bool CommitTran()
        {
            if (this.Transaction == null)//事物对象未初始化
            {
                LastError = "数据库事务未创建";
                return false;
            }
            if (_IsInTransaction)
            {
                try
                {
                    this.Transaction.Commit();
                    this._IsInTransaction = false;
                    return true;
                }
                catch (Exception exp)
                {
                    this.LastError = exp.ToString();
                    return false;
                }
            }
            else//不在事务中
            {
                LastError = "数据库事务未开始";
                return false;
            }
        }
        #endregion


        public void ProcessSqlCommand(MySqlCommand Command)
        {

            MySqlConnection connect = new MySqlConnection(this.ConectString);
            Command.Connection = connect;
            try
            {
                connect.Open();
                Command.ExecuteNonQuery();
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, Command.CommandText);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                connect.Close();
            }
        }

        public  async Task ProcessSqlCommandAsync(MySqlCommand Command)
        {

            MySqlConnection connect = new MySqlConnection(this.ConectString);
            Command.Connection = connect;
            try
            {
                connect.Open();
                Command.ExecuteNonQuery();

                //Global.WriteLog("ProcessSqlCommandAsync"+ Command.CommandText);
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, Command.CommandText);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                connect.Close();
            }
        }



        #region<<属性>>
        /// <summary>
        /// 数据库操作异常计数
        /// </summary>
        public int OperationError
        {
            get;
            protected set;
        }
        public void ResetError()
        {
            this.OperationError = 0;
        }
        public string ConectString
        {
            get { return this.conectstr; }
        }
        #endregion
        #region<<方法>>
        public static void WriteErrorLog(string errormsg)
        {
            string path = "DatabaseErrorLog.txt";
            FileStream fs = null;
            StreamWriter sw = null;
            lock (DBTrans.Locker)
            {
                #region<<写入日志>>
                try
                {
                    fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    if (fs.Length < 100000000)//日志文件最大100M
                    {
                        sw = new StreamWriter(fs);
                        sw.WriteLine(errormsg);
                    }


                }
                catch (Exception ex)
                {


                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();

                }
                #endregion
            }
        }
        public DBTrans() //DataServerConfig config, bool remoteenable
        {
            //this.conectstr = string.Format("Database={0}; Data Source={1}; UserID={2}; Password={3}", config.DataBaseName, config.DataServerName, config.UserName, config.PassWord);
            //this.conectstr = "Database = huamai; Data Source = localhost;port=3306; UserID = root; Password = P@ss1234;AllowLoadLocalInfile=True";
            //this.Config.NomalConfig.DataServerConfig.PassWord
            //this.conectstr = "Database = huamai; Data Source = localhost;port=3306; UserID = root; Password = "+ Global.config.NomalConfig.DataServerConfig.PassWord + ";AllowLoadLocalInfile=True";
            // this.conectstr = "Database = huamai; Data Source = localhost;port=3306; UserID = root; Password = P@ss1234;AllowLoadLocalInfile=True";

            this.conectstr = "Database = huamai; Data Source = localhost;port=3306; UserID = root; Password =QWER1234.qwer;AllowLoadLocalInfile=True";

            //this.RemoteEnable = remoteenable;
        }



        /// <summary>
        /// 批量更新https://mysqlconnector.net/api/mysqlconnector/mysqlbulkcopytype/
        /// </summary>
        /// <param name="dt">更新的内容</param>
        /// <param name="tableName">目标表</param>
        /// <returns></returns>
        public void AddTable(DataTable dt, string tableName)
        {
            MySqlConnector.MySqlConnection connect = null;
            try
            {
                connect = new MySqlConnector.MySqlConnection(this.ConectString);
                connect.Open();

                var bulkCopy = new MySqlConnector.MySqlBulkCopy(connect);

                bulkCopy.DestinationTableName = tableName;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    bulkCopy.ColumnMappings.Add(new MySqlConnector.MySqlBulkCopyColumnMapping(i, dt.Columns[i].ColumnName));
                }
                var result = bulkCopy.WriteToServer(dt); //await  mySqlbulkcopy.WriteToServer(dt) have ex; bulkCopy.WriteToServerAsync(dt) no ex
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                if (connect != null)
                    connect.Close();
            }
        }
        public bool ExcuteScript(object a)
        {
            Global.WriteDebugAsync("Veh edit ExcuteScript" + a, "Veh");

            string query = a as string;
            if (query == null)
                return false;

            MySqlConnection connect = null;
            try
            {
                connect = new MySqlConnection(this.ConectString);
                connect.Open();
                MySqlScript script = new MySqlScript(connect);
                script.Query = query;
                Global.WriteDebugAsync("Veh edit ExcuteScript query" + query, "Veh");

                int count = script.Execute();
                return true;

            }
            catch (Exception se)
            {

                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, query);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
                return false;

            }
            finally
            {
                if (connect != null)
                    connect.Close();
            }

        }



        //-1 error
        public void ExcuteScript0(object a)
        {
            string query = a as string;
            if (query == null)
                return;

            MySqlConnection connect=null;
            try
            {
                 connect = new MySqlConnection(this.ConectString);
                MySqlScript script = new MySqlScript(connect);
                script.Query = query;

                connect.Open();
                int count = script.Execute();
            }
            catch (Exception se)
            {
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, query);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;

#if DEBUG
                throw (se);
#endif

            }
            finally
            {
                if (connect!= null)
                     connect.Close();
            }


        }
        /// <summary>
        /// 处理一些不带返回数据的数据库操作，比如插入，更新删除等等
        /// </summary>
        /// <param name="a"></param>
        public void ProcessNoqurey(object a)
        {
            string cmdstr = a as string;
            if (cmdstr == null)
                return;
            MySqlConnection connect = new MySqlConnection(this.ConectString);
            MySqlCommand Command = new MySqlCommand(cmdstr);
            Command.Connection = connect;
            try
            {
                connect.Open();
                Command.ExecuteNonQuery();
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, cmdstr);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                connect.Close();
            }
        }

        /// <summary>
        /// 处理一些不带返回数据的数据库操作，比如插入，更新删除等等
        /// </summary>
        /// <param name="a"></param>
        public async void ProcessNoqureyAsync(object a)
        {
            string cmdstr = a as string;
            if (cmdstr == null)
                return;
            MySqlConnection connect = new MySqlConnection(this.ConectString);
            MySqlCommand Command = new MySqlCommand(cmdstr);
            Command.Connection = connect;
            try
            {
                connect.Open();
                Command.ExecuteNonQuery();
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, cmdstr);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                connect.Close();
            }
        }

        public object ExecuteScalar(object a)
        {
            string cmdstr = a as string;
            //if (cmdstr == null)
            //throw  "cmd is null" ;
            MySqlConnection connect = new MySqlConnection(this.ConectString);
            MySqlCommand Command = new MySqlCommand(cmdstr);
            Command.Connection = connect;
            try
            {
                connect.Open();
                return (Command.ExecuteScalar());
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, cmdstr);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
                connect.Close();
            }
            return "";
        }

        public object ExecuteScalar0(object a)
        {
            string cmdstr = a as string;
            //if (cmdstr == null)
            //throw  "cmd is null" ;
            MySqlConnection connect = new MySqlConnection(this.ConectString);
            MySqlCommand Command = new MySqlCommand(cmdstr);
            Command.Connection = connect;
            try
            {
                connect.Open();
                return (Command.ExecuteScalar());
            }
            catch (Exception se)
            {
#if DEBUG
                throw (se);
#endif 
                //此处应该写如错误日志
                string errormsg = string.Format("数据库操作异常 时间:{0},错误描述{1},操作语句：{2}", DateTime.Now.ToString(), se.Message, cmdstr);
                DBTrans.WriteErrorLog(errormsg);
                this.OperationError++;
            }
            finally
            {
               connect.Close();
            }
            return "";
        }
        /// <summary>
        /// 根据SQL得到DataTable，SQL语句如果生成多个表，返回第一个
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdParam"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                if (this.Connection.State == ConnectionState.Closed)
                    this.Connection.Open();
  
                MySqlDataAdapter sda = new MySqlDataAdapter(sql, this.Connection);
                sda.Fill(dt);
                Global.DBConnStatus = 1;
            }
            catch (Exception ex)
            {
                Global.DBConnStatus = -1;
                //MessageBox.Show(ex.Message);
                Global.PromptFail("[Error]DB-GetDataTable"+ ex.Message);
                Global.WriteLog("[Error]DB-GetDataTable" + ex.Message + "\r\n" + ex.StackTrace + sql);

            }
            return dt;
        }



        #endregion
        #region<<数据>>
        private string conectstr;
        private static readonly object Locker = new object();
        private bool RemoteEnable;
        #endregion


        #region Business
        //仓位记录 INSERT INTO table_name (column1, column2, column3) VALUES ('value1', 'value2', 'value3')
        //, ('value4', 'value5', 'value6')
        //, ('value7', 'value8', 'value9');
        public void CreateUnitRecord() //string VehInNO,
        {
            string PlateNO = Global.PlateNO;
            if (PlateNO == null || PlateNO == "")
            {
                return;
            }
           
            string VehInNO = Global.VehInNO;
            string sql = "Insert into UnitRecord(ChargerNo,EnableFlag,VIN,BattertySN,ChargerStatus,GunWorkStatus)";
            sql += " VALUES ";

            int k = 0;
            for (int i = 1; i < 8; i++)
            {
                if (Global.SqlCmd104[i] == null || Global.SqlCmd116[i] == null) break;
                else k++;
                string VIN = "";
                string GunWorkStatus = "";
                string strCreateTime = Global.SqlCmd104[i].Parameters["@CreateTime"].Value.ToString();
                if (DateTime.TryParse(strCreateTime, out DateTime CreateTime))
                {
                   if( Global.DiffSeconds(CreateTime, DateTime.Now)<=60)
                    { 
                        VIN = Global.SqlCmd104[i].Parameters["@VIN"].Value.ToString();
                        GunWorkStatus = Global.SqlCmd104[i].Parameters["@WorkStatus"].Value.ToString();
                    }
                }

                string BattertySN = "";
                string strCreateTime116 = Global.SqlCmd116[i].Parameters["@CreateTime"].Value.ToString();
                if (DateTime.TryParse(strCreateTime116, out DateTime CreateTime116))
                {
                    if (Global.DiffSeconds(CreateTime116, DateTime.Now) <= 60)
                        BattertySN = Global.SqlCmd104[i].Parameters["@BattertySN"].Value.ToString();
                }

                string ChargerStatus = Global.ChargerStatus[i].ToString();

                if (i!=1) { sql += ","; }
                sql += " ( ";
                sql += "'" + i +"',"+Global.ChargerEnableFlag[i] + ",'" + VIN + "','"+ BattertySN + "'," + ChargerStatus +"," + GunWorkStatus;
                sql += " ) ";
            }
            if(k>0) this.ExcuteScript0(sql);
        }

        //云端补发消息 SendFlag  0 未发送  1 已发送
        //protocol 1 Http 2 MQ 已测试通过
        static string msgHead = "Insert into MsgWait(protocol,MsgType,msg,error) VALUES";
        static StringBuilder sbMsgWait = new StringBuilder(msgHead);
        static int MsgWatiNum=0;
        public void CreateMsgWait(int protocol,string MsgType,string msg,string errorInfo) 
        {
            string sql = "";

            try
            { 
                string VehInNO = Global.VehInNO;
                MsgWatiNum++;
                if (Global.CloudConnStatus == 1)
                {
                    //立即执行堆积数据和本条新增；
                
                    if (sbMsgWait.ToString() != msgHead) sql += ",";
                    sql += "(" + protocol + ", '" + MsgType + "', '" + msg.Replace("'", "''") + "','" + errorInfo.Replace("'", "''") + "')";
                    sbMsgWait.Append(sql);
                    this.ExcuteScript0(sql);
                    sbMsgWait.Clear();
                    Global.WriteLog("[info]CloudConnStatus:" + MsgWatiNum);
                }
                else {
                    //批量处理500条
                
                    if (sbMsgWait.ToString() != msgHead) sql += ",";
                    sql += "(" + protocol + ", '" + MsgType + "', '" + msg.Replace("'", "''") + "','" + errorInfo.Replace("'", "''") + "')";
                    sbMsgWait.Append(sql);
                    if (MsgWatiNum == 500)
                    {
                        this.ExcuteScript0(sbMsgWait.ToString());
                        Global.WriteLog("[info]sql-" + sbMsgWait.ToString());
                        sbMsgWait.Clear();
                        sbMsgWait.Append(msgHead);
                        Global.WriteLog("[info]MsgWatiNum500-" + MsgWatiNum);
                    
                        MsgWatiNum = 0;
                    }
                    Global.WriteLog("[info]MsgWatiNum" + MsgWatiNum);
                }
                //string sql = "Insert into MsgWait(protocol,";
                //sql += "MsgType,";
                //sql += "msg,";
                //sql += "error) VALUES";
            }catch (Exception ex) {
                Global.WriteLog("[Error]sql" + sql+ ex.Message+ex.StackTrace );

            }
        }

        //换电记录 
        int k = 0;
        public void CreateSwitchRecord(string Timers="" ) //string VehInNO,
        {
            string PlateNO = Global.PlateNO;
            string VehInNO = Global.VehInNO;

            k++;
            VehInNO = "10110-" + Timers + "-" + k + "";

            if (PlateNO == null || VehInNO ==null || PlateNO == "" || VehInNO=="")
            {
                return;
            }

            
            string sql = "Insert into switchrecord(VehInNO,";
            sql += "thoroughfare,";
            sql += "PlateNO,";
            sql += "VehInTime,";
            sql += "RecordTime";
            sql += ") VALUES('"+ VehInNO + "', "+ VehInNO.Substring(3,1) + ", '"+ PlateNO.Replace("'","''") + "', '"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            this.ExcuteScript0(sql);
        }
        //归还
        public void UpdateSwitchRecordBatteryIn(int ChargerID) //string VehInNO,
        {
            if (Global.VehInNO == null || Global.VehInNO == "")
            {
                return;
            }
            
            string BattertySN = Global.SqlCmd116[ChargerID].Parameters["@BattertySN"].Value.ToString();
            string PackSoc = Global.SqlCmd116[ChargerID].Parameters["@PackSoc"].Value.ToString();
            string TtlDisChargeNum = Global.SqlCmd116[ChargerID].Parameters["@TtlDisChargeNum"].Value.ToString();

            string sql = "update switchrecord set";
            sql += "BatteryInSN = '"+ BattertySN + "',";
            sql += "BatteryInSOC = "+ PackSoc + ",";
            sql += "BatteryInKWH = "+ TtlDisChargeNum;
            sql += "where VehInNO = '" +Global.VehInNO+ "'";

            this.ExcuteScript0(sql);
        }



        //借出
        public void UpdateSwitchRecordBatteryOut(int ChargerID) //string VehInNO,
        {
            if (Global.VehInNO == null || Global.VehInNO == "")
            {
                return;
            }

            string BattertySN = Global.SqlCmd116[ChargerID].Parameters["@BattertySN"].Value.ToString();
            string PackSoc = Global.SqlCmd116[ChargerID].Parameters["@PackSoc"].Value.ToString();
            string TtlDisChargeNum = Global.SqlCmd116[ChargerID].Parameters["@TtlDisChargeNum"].Value.ToString();

            string sql = "update switchrecord set";
            sql += "BatteryOutSN = '" + BattertySN + "',";
            sql += "BatteryOutSOC = " + PackSoc + ",";
            sql += "BatteryOutKWH = " + TtlDisChargeNum;
            sql += "where VehInNO = '" + Global.VehInNO + "'";

            this.ExcuteScript0(sql);
        }

        //点开始换电确认，更新换电开始时间
        public void UpdateSwitchRecordStartTime() 
        {
            if (Global.VehInNO == null || Global.VehInNO == "")
            {
                return;
            }
            string sql = "update switchrecord set";
            sql += "StartTime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")  + "',";
            sql += "where VehInNO = '" + Global.VehInNO + "'";

            this.ExcuteScript0(sql);
        }
        //车离站时间暂时跟换电完成时间一致吧，没有直接采集的方案
        public void UpdateSwitchRecordeEndTime() 
        {
            if (Global.VehInNO == null || Global.VehInNO == "")
            {
                return;
            }
            string sql = "update switchrecord set";
            sql += "EndTime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
            sql += "VehOutime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") ;
            sql += "where VehInNO = '" + Global.VehInNO + "'";

            this.ExcuteScript0(sql);
        }

        public void SaveAlarm(int AlarmType, string DeviceNo, int FaultCode,string FaultContent,int FaultLevel, int  unitId=-1)
        {
            string AlarmNo = AlarmType+ DateTime.Now.ToString("yyyyMMddhhmmssfff");
            string sql = "Insert into alarm(AlarmType,AlarmNo,DeviceNo,FaultCode,FaultContent,FaultLevel) VALUES(";
            sql += AlarmType + ",'" + AlarmNo + "','" + DeviceNo + "'," + FaultCode + ",'" + FaultContent + "'," + FaultLevel +")";
            this.ExcuteScript(sql);
            string strUnitId = "";
            if (AlarmType == 2) strUnitId = DeviceNo;
            else if (unitId != -1) strUnitId = unitId.ToString();

            //上报云端
            Global.chargerServer.Publish3009(AlarmType, DeviceNo, FaultCode, strUnitId);
        }
        public void EndAlarm(int FaultCode, string DeviceNo)
        {
            string EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = " update alarm set EndTime='"+ EndTime + "'  where FaultCode=1 and EndTime is NULL";
            sql+= " and DeviceNo='"+ DeviceNo.Replace("'","") + "'";
            this.ExcuteScript(sql);
        }
        public void Save104Msg(string msgName, Type type, byte GunNo, Object objCmd, int ChargerID, Byte[] msg)
        {
            switch(GunNo)
            {
                case 1:
                    SaveMsg(msgName, type, ref Global.SqlCmd104_1[ChargerID], objCmd, ChargerID, msg);
                    break;
                case 2:
                    SaveMsg(msgName, type, ref Global.SqlCmd104_2[ChargerID], objCmd, ChargerID, msg);
                    break;
                case 3:
                     SaveMsg(msgName, type, ref Global.SqlCmd104_3[ChargerID], objCmd, ChargerID, msg);
                    break;
                case 4:
                    SaveMsg(msgName, type, ref Global.SqlCmd104_4[ChargerID], objCmd, ChargerID, msg);
                    break;
                default:
                    SaveMsg(msgName, type, ref Global.SqlCmd104_1[ChargerID], objCmd, ChargerID, msg);
                    break;
            }
        }

        //暂未启用（直接在222里转了） 根据cmd222的字段名和字段值，格式化为数据库或报文值
        //FieldName 字段名 fType 字段类型
        public void GetFieldValue(string FieldName,string fType)
        {
            string[] arrFieldname;
            bool isSpecialField = false;
            arrFieldname = FieldName.Split('_');
            string fieldValue = "";
            isSpecialField = false;
            if (arrFieldname.Length > 1)
            {
                isSpecialField = true;
            }

            //switch (fType)
            //{
            //    case "Byte[]":
            //        Byte[] FieldValue = (Byte[])fieldInfo.GetValue(objCmd);

            //        if (isSpecialField == true)
            //        {
            //            MarshalAsAttribute attribute = (MarshalAsAttribute)fieldInfo.GetCustomAttributes(typeof(MarshalAsAttribute), false)[0];
            //            int len = attribute.SizeConst;
            //            fieldValue = formatSpecialField(FieldName, arrFieldname[1], FieldValue, len);
            //        }
            //        else
            //        {
            //            fieldValue = BitConverter.ToString(FieldValue);
            //        }
            //        break;
            //    case "Int32":
            //    case "Int16":
            //    case "UInt16":
            //    case "Byte":
            //        fieldValue = (fieldInfo.GetValue(objCmd)).ToString();
            //        if (isSpecialField == true)
            //        {
            //            fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue);
            //        }
            //        break;
            //    case "UInt32":
            //        fieldValue = ((UInt32)fieldInfo.GetValue(objCmd)).ToString();
            //        if (isSpecialField == true)
            //        {
            //            if (FieldName.Split('_')[1].Substring(0, 1) == "3")
            //            {
            //                string varPara = fieldInfos[i - 1].GetValue(objCmd).ToString();
            //                fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue, varPara);
            //            }
            //            else
            //            {
            //                fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue);
            //            }
            //        }
            //        break;

            //}
           

            if (FieldName.Replace("_7", "") == FieldName) //20240222
            {
                fieldValue = fieldValue.Replace("-", "");
            }
              
           
        }
            public void SaveMsg(string msgName, Type type, ref MySqlCommand SqlCmd, Object objCmd, int ChargerID, Byte[] msg)
        {
            //SqlCmd.Parameters.Clear(); // 清除之前添加的参数
            bool isNewSqlCmd = false;
            if (SqlCmd == null) SqlCmd = new MySqlCommand();
            if (SqlCmd.Parameters.Count == 0) isNewSqlCmd = true;

            string Fields = "";
            string FieldsPara = "";
            FieldInfo[] fieldInfos = type.GetFields();
            int FieldCount = fieldInfos.Length;
            string FieldName = "";
            string[] arrFieldname;
            string DBFieldName = "";
            bool isSpecialField = false;

            for (int i = 0; i < FieldCount; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                FieldName = fieldInfo.Name;
                arrFieldname = FieldName.Split('_');
                DBFieldName = FieldName;

                string fType = fieldInfo.FieldType.Name.ToString();
                string fieldValue = "";
                isSpecialField = false;

                if (arrFieldname.Length > 1)
                {
                    isSpecialField = true;
                    DBFieldName = arrFieldname[0];
                }

                if (fieldInfo.GetValue(objCmd) != null)
                {
                    switch (fType)
                    {
                        case "Byte[]":
                            Byte[] FieldValue = (Byte[])fieldInfo.GetValue(objCmd);

                            if (isSpecialField == true)
                            {
                                MarshalAsAttribute attribute = (MarshalAsAttribute)fieldInfo.GetCustomAttributes(typeof(MarshalAsAttribute), false)[0];
                                int len = attribute.SizeConst;
                                fieldValue = formatSpecialField(FieldName, arrFieldname[1], FieldValue, len);
                            }
                            else
                            {
                                fieldValue = BitConverter.ToString(FieldValue);
                            }
                            break;
                        case "Int32":
                        case "Int16":
                        case "UInt16":
                        case "Byte":
                            fieldValue = (fieldInfo.GetValue(objCmd)).ToString();
                            if (isSpecialField == true)
                            {
                                fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue);
                            }
                            break;
                        case "UInt32":
                            fieldValue = ((UInt32)fieldInfo.GetValue(objCmd)).ToString();
                            if (isSpecialField == true)
                            {
                                if (FieldName.Split('_')[1].Substring(0, 1) == "3")
                                {
                                    string varPara = fieldInfos[i - 1].GetValue(objCmd).ToString();
                                    fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue, varPara);
                                }
                                else
                                {
                                    fieldValue = formatSpecialField(FieldName, arrFieldname[1], fieldValue);
                                }
                            }
                            break;

                    }
                }
                Fields += DBFieldName; if (i != FieldCount - 1) Fields += ",";
                FieldsPara += "@" + DBFieldName; if (i != FieldCount - 1) FieldsPara += ",";

                if (FieldName.Replace("_7", "") == FieldName) //20240222
                {
                    fieldValue = fieldValue.Replace("-", "");
                }
                if (isNewSqlCmd == true)
                {
                        SqlCmd.Parameters.AddWithValue("@" + DBFieldName, fieldValue);
                }
                else
                {
                    SqlCmd.Parameters["@" + DBFieldName].Value = fieldValue;
   
                }
            }
            if (isNewSqlCmd == true)
            {
                SqlCmd.Parameters.AddWithValue("@msg", BitConverter.ToString(msg));
                SqlCmd.Parameters.AddWithValue("@CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            }
            else
            {
                SqlCmd.Parameters["@msg"].Value = BitConverter.ToString(msg);//.ToString();
                SqlCmd.Parameters["@CreateTime"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            SqlCmd.CommandText = "INSERT INTO " + msgName + "(" + Fields + ",Msg" + ") VALUES(" + FieldsPara + ",@msg)";
            //this.ProcessSqlCommand(SqlCmd);
            //0411 试下异步插入，看能不能提升消息性能，不行就批量；确实能节约50ms左右；
            this.ProcessSqlCommandAsync(SqlCmd);
            
        }

        public void SaveCmd118(string BatterySN, string JsonData, int ChargerID, Byte[] msg)
        {
            string sql = "INSERT INTO cmd118 (ChargerNo,BattertySN,JsonData,Msg) VALUES(" + "'" + ChargerID + "'" + "," + "'" + BatterySN + "'" + "," + "'" + JsonData + "'" + "," + "'" + BitConverter.ToString(msg) + "'" + ")";
            this.ExcuteScript0(sql);
        }
        public void SaveCmd120(string BatterySN, string JsonData, int ChargerID, Byte[] msg)
        {
            string sql = "INSERT INTO cmd120 (ChargerNo,BattertySN,JsonData,Msg) VALUES(" + "'" + ChargerID + "'" + "," + "'" + BatterySN + "'" + "," + "'" + JsonData + "'" + "," + "'" + BitConverter.ToString(msg) + "'" + ")";
            this.ExcuteScript0(sql);
        }
        public string formatSpecialField(string FieldName, string FieldExtName, string FieldValue, string varPara)
        {
            string rtn = FieldValue;
            string strExt = FieldExtName.Substring(0, 1);

            if (strExt == "3")
            {
                //1:时间控制充电2:金额控制充电3:电量控制充电
                switch (varPara)
                {
                    case "2":
                    case "3":
                        try
                        {
                            rtn = (UInt32.Parse(rtn) * 0.01).ToString();
                        }
                        catch (Exception e) { Global.WriteLog("[Error]formatSpecialField,varPara," + e.Message); }
                        break;
                }

                rtn = FieldValue;

            }

            return rtn;
        }

        public string formatSpecialField(string FieldName, string FieldExtName, string FieldValue)
        {
            string rtn = FieldValue;
            string strExt = FieldExtName.Substring(0, 1);
            string strJD = FieldExtName.Substring(1, 1);
            string strPY = FieldExtName.Substring(2, 1);
            double JD = 1, PY = 0;

            if (strExt == "0")
            {
                try
                {
                    switch (strJD)
                    {
                        case "1":
                            JD = 0.1; break;
                        case "2":
                            JD = 0.4; break;
                        case "3":
                            JD = 0.01; break;
                        case "4":
                            JD = 0.001; break;
                    }
                    switch (strPY)
                    {
                        case "1":
                            PY = -40; break;
                        case "2":
                            PY = -50; break;
                        case "3":
                            PY = -1000; break;
                    }

                    rtn = (UInt32.Parse(rtn) * JD + PY).ToString();
                }
                catch (Exception e) { Global.WriteLog("[Error]formatSpecialField,varPara," + e.Message); }
            }

            return rtn;
        }

        public string formatSpecialField(string FieldName, string FieldExtName, Byte[] FieldValue, int byteLen)
        {
            string rtn = "";
            string strExt = FieldExtName.Substring(0, 1);

            switch (strExt)
            {
                case "1":
                    rtn = Encoding.ASCII.GetString(FieldValue, 0, byteLen).Replace("\0", "");
                    break;
                case "2":
                    rtn = BitConverter.ToString(FieldValue);
                    break;
                case "4":
                    rtn = FieldValue[0].ToString("X2") + FieldValue[1].ToString("x2") + "-" + FieldValue[2].ToString("x2") + "-" + FieldValue[3].ToString("x2") + " " + FieldValue[4].ToString("x2") + ":" + FieldValue[5].ToString("x2") + ":" + FieldValue[6].ToString("x2");
                    break;
                case "7":
                    rtn = FieldValue[0].ToString() + FieldValue[1].ToString() + "-" +FieldValue[2].ToString()+ "-" + FieldValue[3].ToString() + " " + FieldValue[4].ToString()+ ":" + FieldValue[5].ToString() + ":" +FieldValue[6].ToString();
                    break;
                case "5":
                    for (int i = 0; i < byteLen; i++)
                    {
                        rtn += FieldValue[i].ToString("X2");
                    }
                    break;
                case "6":
                    try
                    {
                        if (FieldValue.Length == 3)
                        {
                            byte[] zeroByteArray = new byte[1] { 0 };
                            FieldValue = FieldValue.Concat(zeroByteArray).ToArray();
                        }
                        rtn = BitConverter.ToInt32(FieldValue, 0).ToString();  //BitConverter.ToInt32(FieldValue).ToString();
                    }
                    catch (Exception ex)
                    {
                        Global.WriteLog("Error:formatSpecialField ToInt32" + "FieldName:" + FieldName + BitConverter.ToString(FieldValue)+ ex.Message + "\r\n" +ex.StackTrace);

                    }
                    break;
            }

            return rtn;
        }
        #endregion
    }
}
