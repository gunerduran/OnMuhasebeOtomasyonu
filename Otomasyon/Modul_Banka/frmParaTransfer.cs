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

namespace Otomasyon.Modul_Banka
{
    public partial class frmParaTransfer : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();


        bool Edit = false;
        int BankaID = -1; //ButtonEdite basınca form açtırabilmek için aşağıda yaptık işlemleri
        int CariID = -1;//ButtonEdite basınca form açtırabilmek için aşağıda yaptık işlemleri
        int IslemID = -1;


        public frmParaTransfer()
        {
            InitializeComponent();
        }



        private void txtTransferTuru_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtTransferTuru.SelectedIndex == 0)//HAVALE İSE
            {
                rbtnGelen.Text = "Gelen Havale";
                rbtnGiden.Text = "Giden Havale";

            }
            else if (txtTransferTuru.SelectedIndex == 1)//EFT İSE
            {
                rbtnGelen.Text = "Gelen EFT";
                rbtnGiden.Text = "Giden EFT";

            }
        }

        private void frmParaTransfer_Load(object sender, EventArgs e)
        {
            txtTarih.Text = DateTime.Now.ToShortDateString();
        }


        void Temizle()
        {
            txtAciklama.Text = "";
            txtBelgeNo.Text = "";
            txtCariAdi.Text = "";
            txtCariKodu.Text = "";
            txtHesapAdi.Text = "";
            txtHesapNo.Text = "";
            txtTarih.Text = DateTime.Now.ToShortDateString();
            txtTutar.Text = "0";

            Edit = false;
            BankaID = -1;
            CariID = -1;
            IslemID = -1;

            AnaForm.Aktarma = -1;
        }

        void BankaAc(int ID)
        {
            try
            {
                BankaID = ID;
                txtHesapAdi.Text = DB.TBL_BANKALARs.First(s => s.ID == BankaID).HESAPADI;
                txtHesapNo.Text = DB.TBL_BANKALARs.First(s => s.ID == BankaID).HESAPNO;
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }


        void CariAc(int ID)
        {
            try
            {
                CariID = ID;
                txtCariKodu.Text = DB.TBL_CARILERs.First(s => s.ID == CariID).CARIKODU;
                txtCariAdi.Text = DB.TBL_CARILERs.First(s => s.ID == CariID).CARIADI;

            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_BANKAHAREKETLERI Banka = new Fonksiyonlar.TBL_BANKAHAREKETLERI();
                Banka.ACIKLAMA = txtAciklama.Text;
                Banka.BANKAID = BankaID;
                Banka.BELGENO = txtBelgeNo.Text;
                Banka.CARIID = CariID;
                Banka.EVRAKTURU = txtTransferTuru.SelectedItem.ToString();
                if (rbtnGelen.Checked) Banka.GCKODU = "G";//Giriş
                if (rbtnGiden.Checked) Banka.GCKODU = "C";//Çıkış

                Banka.TARIH = DateTime.Parse(txtTarih.Text.ToString());//Parse işlemi string değerlerde geçerli
                Banka.TUTAR = decimal.Parse(txtTutar.Text);
                Banka.SAVEDATE = DateTime.Now;
                Banka.SAVEUSER = AnaForm.UserID;//static değişken
                DB.TBL_BANKAHAREKETLERIs.InsertOnSubmit(Banka);
                DB.SubmitChanges();

                //cari hareketide ekliyoruz borç ve alacak olarak
                Fonksiyonlar.TBL_CARIHAREKETLERI Cari = new Fonksiyonlar.TBL_CARIHAREKETLERI();
                Cari.ACIKLAMA = txtAciklama.Text;
                if (rbtnGelen.Checked) Cari.ALACAK = decimal.Parse(txtTutar.Text);//
                if (rbtnGiden.Checked) Cari.BORC = decimal.Parse(txtTutar.Text);
                Cari.CARIID = CariID;
                Cari.EVRAKID = Banka.ID;
                Cari.EVRAKTURU = txtTransferTuru.SelectedItem.ToString();
                Cari.TARIH = DateTime.Parse(txtTarih.Text);
               if(txtTransferTuru.SelectedIndex==0) Cari.TIPI="BH";//BANKA hAVALE
               if (txtTransferTuru.SelectedIndex == 1) Cari.TIPI = "EFT";//BANKA EFT

               Cari.SAVEDATE = DateTime.Now;
               Cari.SAVEUSER = AnaForm.UserID;

               DB.TBL_CARIHAREKETLERIs.InsertOnSubmit(Cari);
               DB.SubmitChanges();
               Mesajlar.YeniKayit("Para Transfer Kaydı İşlendi");
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
                Fonksiyonlar.TBL_BANKAHAREKETLERI Banka = DB.TBL_BANKAHAREKETLERIs.First(s=>s.ID==IslemID);
                Banka.ACIKLAMA = txtAciklama.Text;
                Banka.BANKAID = BankaID;
                Banka.BELGENO = txtBelgeNo.Text;
                Banka.CARIID = CariID;
                Banka.EVRAKTURU = txtTransferTuru.SelectedItem.ToString();
                if (rbtnGelen.Checked) Banka.GCKODU = "G";//Giriş
                if (rbtnGiden.Checked) Banka.GCKODU = "C";//Çıkış

                Banka.TARIH = DateTime.Parse(txtTarih.Text.ToString());//Parse işlemi string değerlerde geçerli
                Banka.TUTAR = decimal.Parse(txtTutar.Text);
                Banka.EDITDATE = DateTime.Now;
                Banka.EDITUSER = AnaForm.UserID;//static değişken
                
                DB.SubmitChanges();

                //cari hareketide ekliyoruz borç ve alacak olarak
                Fonksiyonlar.TBL_CARIHAREKETLERI Cari = DB.TBL_CARIHAREKETLERIs.First(s=>s.EVRAKTURU==txtTransferTuru.SelectedItem.ToString() && s.EVRAKID==IslemID);
                Cari.ACIKLAMA = txtAciklama.Text;
                if (rbtnGelen.Checked) Cari.ALACAK = decimal.Parse(txtTutar.Text);//
                if (rbtnGiden.Checked) Cari.BORC = decimal.Parse(txtTutar.Text);
                Cari.CARIID = CariID;
                Cari.EVRAKID = Banka.ID;
                Cari.EVRAKTURU = txtTransferTuru.SelectedItem.ToString();
                Cari.TARIH = DateTime.Parse(txtTarih.Text);
                if (txtTransferTuru.SelectedIndex == 0) Cari.TIPI = "BH";//BANKA hAVALE
                if (txtTransferTuru.SelectedIndex == 1) Cari.TIPI = "EFT";//BANKA EFT

                Cari.EDITDATE = DateTime.Now;
                Cari.EDITUSER = AnaForm.UserID;

              
                DB.SubmitChanges();
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
                //ilişkili tablolar olduğu için sırayla sildik.Yani en alttan en başa doğru sildik.Yavrudan anaya doğru gibi.Yoksa Foreign Key Hatası alırız.
                DB.TBL_CARIHAREKETLERIs.DeleteOnSubmit(DB.TBL_CARIHAREKETLERIs.First(s=>s.EVRAKTURU==txtTransferTuru.SelectedItem.ToString() && s.EVRAKID==IslemID));
                DB.TBL_BANKAHAREKETLERIs.DeleteOnSubmit(DB.TBL_BANKAHAREKETLERIs.First(s=>s.ID==IslemID));
                
                DB.SubmitChanges();//iki işlemi aynı anda submit ediyoruz

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
                IslemID = ID;
                Fonksiyonlar.TBL_BANKAHAREKETLERI Banka = DB.TBL_BANKAHAREKETLERIs.First(s=>s.ID==IslemID);
                BankaAc(Banka.BANKAID.Value);
                CariAc(Banka.CARIID.Value);
                txtAciklama.Text = Banka.ACIKLAMA;
                txtBelgeNo.Text = Banka.BELGENO;
                txtTarih.Text = Banka.TARIH.Value.ToShortDateString();
                txtTransferTuru.Text = Banka.EVRAKTURU;
                txtTutar.Text = Banka.TUTAR.ToString();
                if (Banka.GCKODU == "G") rbtnGelen.Checked = true;
                if (Banka.GCKODU == "C") rbtnGiden.Checked = true;

            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void txtHesapAdi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int Id = Formlar.BankaListesi(true);
            if (Id > 0) BankaAc(Id);
            AnaForm.Aktarma = -1;//static değişkene atadık.s
        }

        private void txtCariKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int Id = Formlar.CariListesi(true);
            if (Id > 0) CariAc(Id);
            AnaForm.Aktarma = -1;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit && CariID > 0 && BankaID > 0 && IslemID > 0 && Mesajlar.Guncelle() == DialogResult.Yes) Guncelle();
            else YeniKaydet();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit && CariID > 0 && BankaID > 0 && IslemID > 0 && Mesajlar.Sil() == DialogResult.Yes) Sil();
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}