using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayTypedQueue
{
    public class Musteri
    {
        
        public int MusteriNo { get; private set; }
        public int IslemSuresi { get; set; }
        public int ToplamSure { get; set; }
        public int siraNo { get; set; }
        
        public Musteri(int musteriNo,int islemSuresi)
        {
            this.MusteriNo = musteriNo;
            this.IslemSuresi = islemSuresi;
        }
        public int KuyruktaKalmaSuresi(int oncekibeklemesuresi,int islemsuresi)
        {
            ToplamSure = oncekibeklemesuresi + islemsuresi;
            return ToplamSure;
        }
       
    }
}
