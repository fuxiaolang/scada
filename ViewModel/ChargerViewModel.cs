using DESCADA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCADA
{
    public class ChargerViewModel : INotifyPropertyChanged
    {
        private ChargerModel charger;

        public ChargerViewModel()
        {
            charger = new ChargerModel();
        }

        public int ChargerID
        {
            get { return charger.ChargerID; }
            set
            {
                if (charger.ChargerID != value)
                {
                    charger.ChargerID = value;
                    OnPropertyChanged("ChargerID");
                }
            }
        }


        public string ChargerNo
        {
            get { return charger.ChargerNo; }
            set
            {
                if (charger.ChargerNo != value)
                {
                    charger.ChargerNo = value;
                    OnPropertyChanged("ChargerNo");
                }
            }
        }

        public string SoftVersion
        {
            get { return charger.SoftVersion; }
            set
            {
                if (charger.SoftVersion != value)
                {
                    charger.SoftVersion = value;
                    OnPropertyChanged("SoftVersion");
                }
            }
        }

        public string Model
        {
            get { return charger.Model; }
            set
            {
                if (charger.Model != value)
                {
                    charger.Model = value;
                    OnPropertyChanged("Model");
                }
            }
        }

        public string V
        {
            get { return charger.V; }
            set
            {
                if (charger.V != value)
                {
                    charger.V = value;
                    OnPropertyChanged("V");
                }
            }
        }

        public string A
        {
            get { return charger.A; }
            set
            {
                if (charger.A != value)
                {
                    charger.A = value;
                    OnPropertyChanged("A");
                }
            }
        }
        public string KW
        {
            get { return charger.KW; }
            set
            {
                if (charger.KW != value)
                {
                    charger.KW = value;
                    OnPropertyChanged("KW");
                }
            }
        }
        public string KWH
        {
            get { return charger.KWH; }
            set
            {
                if (charger.KWH != value)
                {
                    charger.KWH = value;
                    OnPropertyChanged("KWH");
                }
            }
        }
        public string Vac
        {
            get { return charger.Vac; }
            set
            {
                if (charger.Vac != value)
                {
                    charger.Vac = value;
                    OnPropertyChanged("Vac");
                }
            }
        }
        public string Vab
        {
            get { return charger.Vac; }
            set
            {
                if (charger.Vab != value)
                {
                    charger.Vab = value;
                    OnPropertyChanged("Vab");
                }
            }
        }
        public string Vbc
        {
            get { return charger.Vbc; }
            set
            {
                if (charger.Vbc != value)
                {
                    charger.Vbc = value;
                    OnPropertyChanged("Vbc");
                }
            }
        }
        public string Lac
        {
            get { return charger.Lac; }
            set
            {
                if (charger.Lac != value)
                {
                    charger.Lac = value;
                    OnPropertyChanged("Lac");
                }
            }
        }
        public string Lab
        {
            get { return charger.Lab; }
            set
            {
                if (charger.Lab != value)
                {
                    charger.Lab = value;
                    OnPropertyChanged("Lab");
                }
            }
        }
        public string Lbc
        {
            get { return charger.Lbc; }
            set
            {
                if (charger.Lbc != value)
                {
                    charger.Lbc = value;
                    OnPropertyChanged("Lbc");
                }
            }
        }
        public string PowerFactor
        {
            get { return charger.PowerFactor; }
            set
            {
                if (charger.PowerFactor != value)
                {
                    charger.PowerFactor = value;
                    OnPropertyChanged("PowerFactor");
                }
            }
        }
        public string ChargerErrStatus
        {
            get { return charger.ChargerErrStatus; }
            set
            {
                if (charger.ChargerErrStatus != value)
                {
                    charger.ChargerErrStatus = value;
                    OnPropertyChanged("ChargerErrStatus");
                }
            }
        }
        public string BatteryNo
        {
            get { return charger.BatteryNo; }
            set
            {
                if (charger.BatteryNo != value)
                {
                    charger.BatteryNo = value;
                    OnPropertyChanged("BatteryNo");
                }
            }
        }
        public string BatteryModel
        {
            get { return charger.BatteryModel; }
            set
            {
                if (charger.BatteryModel != value)
                {
                    charger.BatteryModel = value;
                    OnPropertyChanged("BatteryModel");
                }
            }
        }
        public string soh
        {
            get { return charger.soh; }
            set
            {
                if (charger.soh != value)
                {
                    charger.soh = value;
                    OnPropertyChanged("soh");
                }
            }
        }
        public string CurrentSoc
        {
            get { return charger.CurrentSoc; }
            set
            {
                if (charger.CurrentSoc != value)
                {
                    charger.CurrentSoc = value;
                    OnPropertyChanged("CurrentSoc");
                }
            }
        }
        public string CurrentKWH
        {
            get { return charger.CurrentKWH; }
            set
            {
                if (charger.CurrentKWH != value)
                {
                    charger.CurrentSoc = value;
                    OnPropertyChanged("CurrentKWH");
                }
            }
        }
        public string CurrentAh
        {
            get { return charger.CurrentAh; }
            set
            {
                if (charger.CurrentAh != value)
                {
                    charger.CurrentSoc = value;
                    OnPropertyChanged("CurrentAh");
                }
            }
        }
        public string RemainMin
        {
            get { return charger.RemainMin; }
            set
            {
                if (charger.RemainMin != value)
                {
                    charger.RemainMin = value;
                    OnPropertyChanged("RemainMin");
                }
            }
        }
        public string MaxV
        {
            get { return charger.MaxV; }
            set
            {
                if (charger.MaxV != value)
                {
                    charger.MaxV = value;
                    OnPropertyChanged("MaxV");
                }
            }
        }

        public string MinV
        {
            get { return charger.MinV; }
            set
            {
                if (charger.MinV != value)
                {
                    charger.MinV = value;
                    OnPropertyChanged("MinV");
                }
            }
        }
        public string MaxC
        {
            get { return charger.MaxC; }
            set
            {
                if (charger.MaxC != value)
                {
                    charger.MaxC = value;
                    OnPropertyChanged("MaxC");
                }
            }
        }
        public string MinC
        {
            get { return charger.MinC; }
            set
            {
                if (charger.MinC != value)
                {
                    charger.MinC = value;
                    OnPropertyChanged("MinC");
                }
            }
        }
        public string BatteryErrStatus
        {
            get { return charger.BatteryErrStatus; }
            set
            {
                if (charger.BatteryErrStatus != value)
                {
                    charger.BatteryErrStatus = value;
                    OnPropertyChanged("BatteryErrStatus");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
