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

namespace Otomasyon.Modul_Cari
{
    public partial class frmCariAcilisKarti : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();
        Fonksiyonlar.Numara Numaralar = new Fonksiyonlar.Numara();

        bool Edit = false;
        int CariID = -1;
        int GrupID = -1;



        public frmCariAcilisKarti()
        {
            InitializeComponent();
        }

        private void frmCariAcilisKarti_Load(object sender, EventArgs e)
        {
            txtCariKodu.Text = Numaralar.CariKodNumarasi();
        }

        void Temizle()
        {
            //kısa yoldan temizleme 
            foreach (Control CT in groupControl1.Controls)
            {
                if (CT is TextEdit || CT is ButtonEdit)//tip karşılaştırması varsa is kullanılır.(CT TextEdit mi diye sorduk)
                {
                    CT.Text = "";
                }
            }

            //kısa yoldan temizleme 
            foreach (Control CE in groupControl2.Controls)
            {
                if (CE is TextEdit || CE is ButtonEdit || CE is MemoEdit)
                {
                    CE.Text = "";
                }
            }
            txtCariKodu.Text = Numaralar.CariKodNumarasi();
            Edit = false;
            CariID = -1;
            GrupID = -1;
            AnaForm.Aktarma = -1;


        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_CARILER Cari = new Fonksiyonlar.TBL_CARILER();
                Cari.ADRES = txtAdres.Text;
                Cari.CARIADI = txtCariAdi.Text;
                Cari.CARIKODU = txtCariKodu.Text;
                Cari.FAX1 = txtFax1.Text;
                Cari.FAX2 = txtFax2.Text;
                Cari.GRUPID = GrupID;
                Cari.ILCE = txtIlce.Text;
                Cari.MAILINFO = txtMailInfo.Text;
                Cari.SEHIR = txtSehir.Text;
                Cari.TELEFON1 = txtTelefon1.Text;
                Cari.TELEFON2 = txtTelefon2.Text;
                Cari.ULKE = txtUlke.Text;
                Cari.VERGIDAIRESI = txtVergiDairesi.Text;
                Cari.VERGINO = txtVergiNo.Text;
                Cari.WEBADRES = txtWebAdres.Text;
                Cari.YETKILI1 = txtYetkili1.Text;
                Cari.YETKILI2 = txtYetkili2.Text;
                Cari.YETKILIEMAIL1 = txtYetkiliEmail1.Text;
                Cari.YETKILIEMAIL2 = txtYetkiliEmail2.Text;

                Cari.SAVEDATE = DateTime.Now;
                Cari.SAVEUSER = AnaForm.UserID;//static değişkendeki değeri yazdırdık.

                DB.TBL_CARILERs.InsertOnSubmit(Cari);
                DB.SubmitChanges();//değişiklikleri kaydettik
                Mesajlar.YeniKayit("Yeni Cari Kaydı Oluşturulmuştur");
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
                Fonksiyonlar.TBL_CARILER Cari = DB.TBL_CARILERs.First(c => c.ID == CariID);
                Cari.ADRES = txtAdres.Text;
                Cari.CARIADI = txtCariAdi.Text;
                Cari.CARIKODU = txtCariKodu.Text;
                Cari.FAX1 = txtFax1.Text;
                Cari.FAX2 = txtFax2.Text;
                Cari.GRUPID = GrupID;
                Cari.ILCE = txtIlce.Text;
                Cari.MAILINFO = txtMailInfo.Text;
                Cari.SEHIR = txtSehir.Text;
                Cari.TELEFON1 = txtTelefon1.Text;
                Cari.TELEFON2 = txtTelefon2.Text;
                Cari.ULKE = txtUlke.Text;
                Cari.VERGIDAIRESI = txtVergiDairesi.Text;
                Cari.VERGINO = txtVergiNo.Text;
                Cari.WEBADRES = txtWebAdres.Text;
                Cari.YETKILI1 = txtYetkili1.Text;
                Cari.YETKILI2 = txtYetkili2.Text;
                Cari.YETKILIEMAIL1 = txtYetkiliEmail1.Text;
                Cari.YETKILIEMAIL2 = txtYetkiliEmail2.Text;

                Cari.EDITDATE = DateTime.Now;
                Cari.EDITUSER = AnaForm.UserID;//static değişkendeki değeri yazdırdık.


                DB.SubmitChanges();//değişiklikleri kaydettik
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
                DB.TBL_CARILERs.DeleteOnSubmit(DB.TBL_CARILERs.First(x => x.ID == CariID));
                DB.SubmitChanges();
                Temizle();
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }

        public void Ac(int ID)
        {
            try
            {
                Edit = true;
                CariID = ID;
                Fonksiyonlar.TBL_CARILER Cari = DB.TBL_CARILERs.First(s => s.ID == CariID);
                txtAdres.Text = Cari.ADRES;
                txtCariAdi.Text = Cari.CARIADI;
                txtCariKodu.Text = Cari.CARIKODU;
                txtFax1.Text = Cari.FAX1;
                txtFax2.Text = Cari.FAX2;
                txtIlce.Text = Cari.ILCE;
                txtMailInfo.Text = Cari.MAILINFO;
                txtSehir.Text = Cari.SEHIR;
                txtTelefon1.Text = Cari.TELEFON1;
                txtTelefon2.Text = Cari.TELEFON2;
                txtUlke.Text = Cari.ULKE;
                txtVergiDairesi.Text = Cari.VERGIDAIRESI;
                txtVergiNo.Text = Cari.VERGIDAIRESI;
                txtWebAdres.Text = Cari.WEBADRES;
                txtYetkili1.Text = Cari.YETKILI1;
                txtYetkili2.Text = Cari.YETKILI2;
                txtYetkiliEmail1.Text = Cari.YETKILIEMAIL1;
                txtYetkiliEmail2.Text = Cari.YETKILIEMAIL2;


                GrupAc(Cari.GRUPID.Value);//int istiyor ama Cari.GRUPID nullable int (int?) olduğu için .Value diyerek yazdık


            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);

            }
        }


        void GrupAc(int ID)
        {
            try
            {
                GrupID = ID;
                Fonksiyonlar.TBL_CARIGRUPLARI Grup = DB.TBL_CARIGRUPLARIs.First(s => s.ID == GrupID);
                txtGrupAdi.Text = Grup.GRUPADI;
                txtGrupKodu.Text = Grup.GRUPKODU;

            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);

            }
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit == true && CariID > 0 && Mesajlar.Sil() == DialogResult.Yes)// dialogu açar eğer yese basılmışsa diye kontrol eder 
            {
                Sil();

            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit == true && CariID > 0 && Mesajlar.Guncelle() == DialogResult.Yes)
            {
                Guncelle();

            }
            else
            {
                YeniKaydet();
            }
        }

        private void txtGrupKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int ID = Formlar.CariGruplari(true);

            if (ID>0)
            {
                GrupAc(ID);
            }


            AnaForm.Aktarma = -1;
        }


        public int CariListesi(bool Secim = false)//defaul değeri false yani metoda parametresine değer vermezsek default olarak false verir.
        {

            Modul_Cari.frmCariListesi frm = new frmCariListesi();
            if (Secim==true)
            {
                frm.Secim = Secim;//frmCariListesi formundaki Secim değerine değer atadık(formlar arası veri transferi)
                frm.ShowDialog();
            }
            else
            {
                frm.MdiParent = AnaForm.ActiveForm;//bu formun baba formu AnaForm olsun dedik
                frm.Show();
            }

            return AnaForm.Aktarma;//static değişkendeki değeri dönderdik.

        }

        private void txtCariKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int ID = Formlar.CariListesi(true);
            if (ID>0)
            {
                Ac(ID);
            }
            AnaForm.Aktarma = -1;


        }


    }
}