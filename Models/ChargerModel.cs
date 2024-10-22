using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCADA.Models
{
    class ChargerModel // 20140109
    {
        public int ChargerID { get; set; }
        public string ChargerNo { get; set; } //编码
        public string SoftVersion { get; set; } //软件版本
        public string Model { get; set; }//充电机型号
        public string V { get; set; }//直流电压
        public string A{ get; set; }//直流电流
        public string KW { get; set; }//实时功率
        public string KWH { get; set; }//累计输出电量
        public string Vac { get; set; }//
        public string Vab { get; set; }//
        public string Vbc { get; set; }//
        public string Lac { get; set; }//
        public string Lab { get; set; }//
        public string Lbc { get; set; }
        public string PowerFactor { get; set; }//功率因素
        public string ChargerErrStatus { get; set; }//故障状态

       //Col 1
        public string BatteryNo { get; set; }//电池编码
        public string BatteryModel { get; set; }//电池型号
        public string soh { get; set; }//
        public string CurrentSoc { get; set; }//
        public string CurrentKWH { get; set; }//当前电量
        public string CurrentAh { get; set; }//当前容量
        public string RemainMin { get; set; }//剩余充电电量
        public string MaxV { get; set; }//最高电压单体
        public string MinV { get; set; }//最低电压单体
        public string MaxC{ get; set; }//最高测点温度
        public string MinC{ get; set; }//最低测点温度
        public string BatteryErrStatus { get; set; }//电池故障状态
    }
}
