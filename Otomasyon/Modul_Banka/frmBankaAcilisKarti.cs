using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;//tanımladık FirstOrdefault gibi metodların çıkması için
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Otomasyon.Modul_Banka
{
    public partial class frmBankaAcilisKarti : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();


        bool Edit = false;
        int SecimID = -1;


        public frmBankaAcilisKarti()
        {
            InitializeComponent();
            //yetki işlemi yapıyoruz
            if (AnaForm.Kullanici.KODU == "Normal")//static değişkenden kullanıcıya ulaştık
            {
                btnSil.Enabled = false;//silme yetkisini kapattık
            }
        }

        private void frmBankaAcilisKarti_Load(object sender, EventArgs e)
        {
            Listele();
        }


        void Temizle()
        {
            txtAdres.Text = "";
            txtBankaAdi.Text = "";
            txtHesapAdi.Text = "";//Hesap Adı
            txtHesapAdi.Text = "";
            txtIban.Text = "";
            txtSube.Text = "";
            txtTelefon.Text = "";
            txtTemsilci.Text = "";
            txtTemsilciEmail.Text = "";
            Edit = false;
            SecimID = -1;

            Listele();
        }
        void Listele()
        {

            //SQL DEKİ FIELDLER İLE grıddeki columnlardakki fıeldname isimleri aynı olmalı
            var lst = from s in DB.TBL_BANKALARs
                      select s;

            Liste.DataSource = lst;
        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_BANKALAR Banka = new Fonksiyonlar.TBL_BANKALAR();
                Banka.ADRES = txtAdres.Text;
                Banka.BANKAADI = txtBankaAdi.Text;
                Banka.HESAPADI = txtHesapAdi.Text;
                Banka.HESAPNO = txtHesapNo.Text;
                Banka.IBAN = txtIban.Text;
                Banka.SUBE = txtSube.Text;
                Banka.TEL = txtTelefon.Text;
                Banka.TEMSILCI = txtTemsilci.Text;
                Banka.TEMSILCIEMAIL = txtTemsilciEmail.Text;
                Banka.SAVEDATE = DateTime.Now;
                Banka.SAVEUSER = AnaForm.UserID;

                DB.TBL_BANKALARs.InsertOnSubmit(Banka);
                DB.SubmitChanges();

                Mesajlar.YeniKayit("Yeni Banka Kaydı Açılmıştır");
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
                //gridden seçtiğimiz değeri SecimID değişkenine atıp daha sonra veriyi çekiyoruz
                Fonksiyonlar.TBL_BANKALAR Banka = DB.TBL_BANKALARs.First(x => x.ID == SecimID);//getirip güncelledik
                Banka.ADRES = txtAdres.Text;
                Banka.BANKAADI = txtBankaAdi.Text;
                Banka.HESAPADI = txtHesapAdi.Text;
                Banka.HESAPNO = txtHesapNo.Text;
                Banka.IBAN = txtIban.Text;
                Banka.SUBE = txtSube.Text;
                Banka.TEL = txtTelefon.Text;
                Banka.TEMSILCI = txtTemsilci.Text;
                Banka.TEMSILCIEMAIL = txtTemsilciEmail.Text;

                Banka.EDITDATE = DateTime.Now;
                Banka.EDITUSER = AnaForm.UserID;


                DB.SubmitChanges();//güncelleme için yaptık

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
                //gridden seçtiğimiz değeri SecimID değişkenine atıp daha sonra veriyi çekiyoruz
                DB.TBL_BANKALARs.DeleteOnSubmit(DB.TBL_BANKALARs.First(x => x.ID == SecimID));
                DB.SubmitChanges();
                Temizle();
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }


        void Sec()
        {
            try
            {
                Edit = true;
                SecimID = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());//GRİDDEKİ SEÇİLİ DEĞERİ GETİRDİK (ID -->FİELDNAME)
                if (SecimID > 0) Ac();
            }
            catch (Exception ex)
            {
                Edit = false;
                SecimID = -1;
            }
        }

        void Ac()
        {
            try
            {
                Fonksiyonlar.TBL_BANKALAR Banka = DB.TBL_BANKALARs.First(x => x.ID == SecimID);
                txtAdres.Text = Banka.ADRES;
                txtBankaAdi.Text = Banka.BANKAADI;
                txtHesapAdi.Text = Banka.HESAPADI;
                txtHesapNo.Text = Banka.HESAPNO;
                txtIban.Text = Banka.IBAN;
                txtSube.Text = Banka.IBAN;
                txtTelefon.Text = Banka.TEL;
                txtTemsilci.Text = Banka.TEMSILCI;
                txtTemsilciEmail.Text = Banka.TEMSILCIEMAIL;

            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit == true && SecimID > 0 && Mesajlar.Guncelle() == DialogResult.Yes) Guncelle();
            else YeniKaydet();

            Listele();

        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit == true && SecimID > 0 && Mesajlar.Sil() == DialogResult.Yes) Sil();
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            Sec();
        }

    }
}