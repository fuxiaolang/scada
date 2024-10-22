using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
//using SpeechLib;

namespace DESCADA.Service
{
    public  class Speech
    {
        SoundPlayer myplayer;
        public Speech()
        {
            myplayer = new SoundPlayer();
        }

        ///<summary
        /// 播放声音文件
        /// </summary>
        /// <paramname="FileName">文件全名</param>
        //async public static Task PlaySound(string FileName)
         public void PlaySound(string FileName)
        {
            if (FileName == "12") return; //没有欢迎换电的提示语音
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"Resources\voice\" + FileName + ".wav"; //把wav音频文件放在exe同目录下就行
            player.Load();//加载
            player.Play();//播放
        }



    }
}
