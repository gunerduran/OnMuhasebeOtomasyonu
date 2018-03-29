using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

//ekledik
using System.Data.Linq;
using System.Linq;

namespace Otomasyon.Modul_Stok
{
    public partial class frmStokGruplari : DevExpress.XtraEditors.XtraForm
    {
        //linq to sql eklemiştik
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();//contexti tanımladık
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        public bool Secim = false;

        int SecimID = -1;

        bool Edit = false;

        public frmStokGruplari()
        {
            InitializeComponent();
        }

        private void frmStokGruplari_Load(object sender, EventArgs e)
        {
            Listele();
        }



        void Listele()
        {
            var lst = from s in DB.TBL_STOKGRUPLARIs
                      select s;

            Liste.DataSource = lst;//gridcontrole verileri  bastık
        }

        void Temizle()
        {
            txtGrupAdi.Text="";
            txtGrupKodu.Text="";
            Edit = false; /////////////
            Listele();
        }


        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_STOKGRUPLARI Grup = new Fonksiyonlar.TBL_STOKGRUPLARI();
                Grup.GRUPADI = txtGrupAdi.Text;
                Grup.GRUPKODU = txtGrupKodu.Text;
                Grup.GRUPSAVEDATE = DateTime.Now;
                Grup.GRUPSAVEUSER = AnaForm.UserID;//static değişkeni atadık.
                DB.TBL_STOKGRUPLARIs.InsertOnSubmit(Grup);
                DB.SubmitChanges();
                Mesajlar.YeniKayit("Yeni Grup Kaydı Oluşturuldu.");
                Temizle();
               
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }

        void Guncelle()
        {
            try
            {
                Fonksiyonlar.TBL_STOKGRUPLARI Grup = DB.TBL_STOKGRUPLARIs.First(s => s.ID == SecimID);
                Grup.GRUPKODU = txtGrupKodu.Text;
                Grup.GRUPADI = txtGrupAdi.Text;
                Grup.GRUPEDITUSER = AnaForm.UserID; //static değeri atadık.çünkü bu değere heryerden erişebiliyoruz
                Grup.GRUPEDITDATE = DateTime.Now;

                DB.SubmitChanges();//değişiklikleri kaydettik.

                Mesajlar.Guncelle(true);
                Temizle();
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
                
        }
        void Sil()
        {
            try
            {
                DB.TBL_STOKGRUPLARIs.DeleteOnSubmit(DB.TBL_STOKGRUPLARIs.First(s => s.ID == SecimID));
                DB.SubmitChanges();
                Temizle();
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
                
            }
        }
       



        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();//bu formu kapat
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit==true && SecimID>0 && Mesajlar.Guncelle()==DialogResult.Yes)
            {
                Guncelle();
            }
            else
            {
                YeniKaydet();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit==true && SecimID>0 && Mesajlar.Sil()==DialogResult.Yes)
            {
                Sil();
            }
        }
        void Sec()
        {
            try
            {
                Edit = true;
                SecimID = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());//seçili satırdaki ID kolonundaki değeri aldık.(ID bizim belirlediğimiz fieldname)
                txtGrupAdi.Text = gridView1.GetFocusedRowCellValue("GRUPADI").ToString();//GRUPADI bizim belirlediğimiz fieldname
                txtGrupKodu.Text = gridView1.GetFocusedRowCellValue("GRUPKODU").ToString();
            }
            catch (Exception ex)
            {

                Edit = false;
                SecimID = -1;
            }
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            Sec();
            if (Secim==true && SecimID>0 )
            {
                AnaForm.Aktarma = SecimID; // Aktarma static olduğu için buna değeri atayınca heryerden bu değere ulaşabileceğiz.
                this.Close();//bu formu kapat
            }
        }
    }
}