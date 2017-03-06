using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArrayTypedQueue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int IslemSuresiHesapla()
        {
            int IslemSuresi;
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            IslemSuresi = rnd.Next(60, 600);
            return IslemSuresi;
        }
        int CreateMusteriNo()
        {
            int MusteriNo;
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            MusteriNo = rnd.Next(1, 100);
            return MusteriNo;
        }

        public double Karsilastirma(Musteri FIFO,Musteri Priority)
        {
            return ((FIFO.ToplamSure - Priority.ToplamSure )
                / ((double)((FIFO.ToplamSure < Priority.ToplamSure) ? FIFO.ToplamSure: Priority.ToplamSure  )) * 100 );

        }
        #region make it global
        DataTable daireselDt = new DataTable("Dairesel Müşteri Kuyruğu");
        DataTable priorityKucuktenDt = new DataTable("Priority Müşteri kuyruğu(k-->b");
        DataTable priorityBuyuktenDt = new DataTable("Priority Müşteri kuyruğu(b-->k");
        DataTable daireselPriorityKBKiyas = new DataTable("Dairesel ve Priority(k-->b) Kıyaslaması");
        DataTable daireselPriorityBKKiyas = new DataTable("Dairesel ve Priority(b-->k) Kıyaslaması");

        float oncelikOrt = 0, oncelikTop = 0;
        float oncelikOrtBK = 0, oncelikTopBK = 0;
        float daireselOrtalama = 0, daireselToplam = 0;

        Musteri[] musteri = new Musteri[20];
        Musteri[] m2 = new Musteri[20];
        Musteri[] m3 = new Musteri[20];
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
           
            #region tanımlamalar

            PriorityQueue prSira = new PriorityQueue(20);
            PriorityQueue tersprSira = new PriorityQueue(20);
            CircularArrayTypedQueue Sira = new CircularArrayTypedQueue(20);
            Stack<object> temp = new Stack<object>(20);

            daireselDt.Columns.Add("Sıra No", typeof(int));
            daireselDt.Columns.Add("Musteri No", typeof(int));
            daireselDt.Columns.Add("İşlem Süresi", typeof(int));
            daireselDt.Columns.Add("Toplam Süre", typeof(int));

            priorityKucuktenDt.Columns.Add("Sıra No", typeof(int));
            priorityKucuktenDt.Columns.Add("Musteri No", typeof(int));
            priorityKucuktenDt.Columns.Add("İşlem Süresi", typeof(int));
            priorityKucuktenDt.Columns.Add("Toplam Süre", typeof(int));

            priorityBuyuktenDt.Columns.Add("Sıra No", typeof(int));
            priorityBuyuktenDt.Columns.Add("Musteri No", typeof(int));
            priorityBuyuktenDt.Columns.Add("İşlem Süresi", typeof(int));
            priorityBuyuktenDt.Columns.Add("Toplam Süre", typeof(int));

            daireselPriorityKBKiyas.Columns.Add("Musteri No", typeof(int));
            daireselPriorityKBKiyas.Columns.Add("Toplam Süre Dairesel", typeof(int));
            daireselPriorityKBKiyas.Columns.Add("Toplam Süre Oncelik (k-->b)", typeof(int));
            daireselPriorityKBKiyas.Columns.Add("Yüzdesel Fark", typeof(float));

            daireselPriorityBKKiyas.Columns.Add("Musteri No", typeof(int));
            daireselPriorityBKKiyas.Columns.Add("Toplam Süre Dairesel", typeof(int));
            daireselPriorityBKKiyas.Columns.Add("Toplam Süre Oncelik (b-->k)", typeof(int));
            daireselPriorityBKKiyas.Columns.Add("Yüzdesel Fark", typeof(float));
            #endregion 
            #region musteri oluşturma

            for (int i = 0; i < 20; i++)
            {
                int mNo = CreateMusteriNo();
                int mIslemSuresi = IslemSuresiHesapla();
                musteri[i] = new Musteri(mNo, mIslemSuresi);
                m2[i] = new Musteri(mNo, mIslemSuresi);
                m3[i] = new Musteri(mNo, mIslemSuresi);
                Sira.Insert(musteri[i]);
                prSira.Insert(m2[i]);
                tersprSira.tersineInsert(m3[i]);
            }

            #endregion
            #region daireselKuyruk

           
            Musteri tempMusteri = new Musteri(0,0);
            for (int i = 0; i < musteri.Length; i++)
            {

                if (i==0)
                {
                    var firstClient= (Musteri)Sira.Remove();
                    firstClient.ToplamSure=firstClient.IslemSuresi;
                    firstClient.siraNo = i + 1;
                    tempMusteri = firstClient;//prev client in tutulması için
                    daireselToplam = firstClient.ToplamSure;
                    daireselDt.Rows.Add(firstClient.siraNo,firstClient.MusteriNo, firstClient.IslemSuresi, firstClient.ToplamSure);
                    continue;
                }
                var o = (Musteri)Sira.Remove();
                o.siraNo = i + 1;
                o.ToplamSure = o.KuyruktaKalmaSuresi(tempMusteri.ToplamSure, o.IslemSuresi); 
                daireselToplam += o.ToplamSure;
                daireselDt.Rows.Add(o.siraNo,o.MusteriNo, o.IslemSuresi, o.ToplamSure);
                tempMusteri=o;       
            }
            daireselOrtalama = daireselToplam / musteri.Length;

            dgv_dongusel.DataSource = daireselDt;
            #endregion
            #region öncelikKuyrugu

            
            for (int i = 0; i < musteri.Length ; i++)
            {

                if (i == 0)
                {
                    var firstClient = (Musteri)prSira.Remove();
                    firstClient.siraNo = i + 1;
                    firstClient.ToplamSure = firstClient.IslemSuresi;
                    tempMusteri = firstClient;//prev client in tutulması için
                    oncelikTop = firstClient.ToplamSure;
                    priorityKucuktenDt.Rows.Add(firstClient.siraNo, firstClient.MusteriNo, firstClient.IslemSuresi, firstClient.ToplamSure);
                    continue;
                }
                var o = (Musteri)prSira.Remove();
                o.siraNo = i + 1;
                o.ToplamSure = o.KuyruktaKalmaSuresi(tempMusteri.ToplamSure, o.IslemSuresi);
                oncelikTop += o.ToplamSure;
                priorityKucuktenDt.Rows.Add(o.siraNo, o.MusteriNo, o.IslemSuresi, o.ToplamSure);
                tempMusteri = o;
            }
            oncelikOrt = oncelikTop / musteri.Length;
            dgv_priorityKb.DataSource = priorityKucuktenDt;
            #endregion

            #region öncelikKuyruguBuyuktenKucuge

            for (int i = 0; i < musteri.Length; i++)
            {

                if (i == 0)
                {
                    var firstClient = (Musteri)tersprSira.Remove();
                    firstClient.siraNo = i + 1;
                    firstClient.ToplamSure = firstClient.IslemSuresi;
                    tempMusteri = firstClient;//prev client in tutulması için
                    oncelikTopBK = firstClient.ToplamSure;
                    priorityBuyuktenDt.Rows.Add(firstClient.siraNo, firstClient.MusteriNo, firstClient.IslemSuresi, firstClient.ToplamSure);
                    continue;
                }
                var o = (Musteri)tersprSira.Remove();
                o.siraNo = i + 1;
                o.ToplamSure = o.KuyruktaKalmaSuresi(tempMusteri.ToplamSure, o.IslemSuresi);
                oncelikTopBK += o.ToplamSure;
                priorityBuyuktenDt.Rows.Add(o.siraNo, o.MusteriNo, o.IslemSuresi, o.ToplamSure);
                tempMusteri = o;
            }
            oncelikOrtBK = oncelikTopBK / musteri.Length;
            dgv_bkPriority.DataSource = priorityBuyuktenDt;
            
            #endregion

            txt_crOrtalama.Text = daireselOrtalama.ToString();
            txt_crOrtalama.Enabled = false;
            txt_prOrtalama.Text = oncelikOrt.ToString();
            txt_prOrtalama.Enabled = false;
            txt_bkPriority.Text = oncelikOrtBK.ToString();
            txt_bkPriority.Enabled = false;
        }

        private void btn_Karsilastir_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 20 ; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                   if( musteri[i].MusteriNo==m2[j].MusteriNo)
                   {
                       daireselPriorityKBKiyas.Rows.Add(musteri[i].MusteriNo, musteri[i].ToplamSure, m2[j].ToplamSure, Karsilastirma(musteri[i], m2[j]));
                       break;
                   }
                }
                for (int j = 0; j < 20; j++)
                {
                    if (musteri[i].MusteriNo == m3[j].MusteriNo)
                    {
                        daireselPriorityBKKiyas.Rows.Add(musteri[i].MusteriNo, musteri[i].ToplamSure, m3[j].ToplamSure, Karsilastirma(musteri[i], m3[j]));
                        break;
                    }
                }
                
                
            }




            dgv_kiyas1.DataSource = daireselPriorityKBKiyas;
            dgv_kiyas2.DataSource = daireselPriorityBKKiyas;
        }
        
     


    }    
}
