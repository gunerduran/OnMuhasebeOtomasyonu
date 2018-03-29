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

namespace Otomasyon.Modul_Kasa
{
    public partial class frmKasaTahsilatOdeme : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();

        bool Edit = false;
        int IslemID = -1;
        string IslemTuru = "Kasa Tahsilat";
        int KasaID = -1;
        int CariHareketID = -1;
        int CariID = -1;

        public frmKasaTahsilatOdeme()
        {
            InitializeComponent();
        }

        private void frmKasaTahsilatOdeme_Load(object sender, EventArgs e)
        {
            txtTarih.Text = DateTime.Now.ToShortDateString();

        }

        private void txtIslemTuru_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void Temizle()
        {
            txtAciklama.Text = "";
            txtBelgeNo.Text = "";
            txtCariAdi.Text = "";
            txtCariKodu.Text = "";
            txtIslemTuru.SelectedIndex = 0;
            txtKasaAdi.Text = "";
            txtKasaKodu.Text = "";
            txtTarih.Text = DateTime.Now.ToShortDateString();
            txtTutar.Text = "0";
            Edit = false;
            IslemID = -1;
            KasaID = -1;
            CariID = -1;
            CariHareketID = -1;

            AnaForm.Aktarma = -1;

        }


        void KasaAc(int ID)
        {
            try
            {
                KasaID = ID;

                txtKasaAdi.Text = DB.TBL_KASALARs.First(s => s.ID == KasaID).KASAADI;
                txtKasaKodu.Text = DB.TBL_KASALARs.First(s => s.ID == KasaID).KASAKODU;
            }
            catch (Exception ex)
            {
                KasaID = -1;
            }
        }

        void CariAc(int ID)
        {
            try
            {
                CariID = ID;

                txtCariAdi.Text = DB.TBL_CARILERs.First(s => s.ID == KasaID).CARIADI;
                txtCariKodu.Text = DB.TBL_CARILERs.First(s => s.ID == KasaID).CARIKODU;
            }
            catch (Exception ex)
            {
                CariID = -1;
            }
        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_KASAHAREKETLERI KasaHareketi = new Fonksiyonlar.TBL_KASAHAREKETLERI();
                KasaHareketi.ACIKLAMA = txtAciklama.Text;
                KasaHareketi.BELGENO = txtBelgeNo.Text;
                KasaHareketi.CARIID = CariID;
                KasaHareketi.EVRAKTURU = txtIslemTuru.SelectedItem.ToString();
                if (txtIslemTuru.SelectedIndex == 0) KasaHareketi.GCKODU = "G";//Tahsilatsa Giriş
                if (txtIslemTuru.SelectedIndex == 1) KasaHareketi.GCKODU = "C";//Ödemeyse Çıkış
                KasaHareketi.KASAID = KasaID;
                KasaHareketi.SAVEDATE = DateTime.Now;
                KasaHareketi.SAVEUSER = AnaForm.UserID;//static değişkeni atadık
                KasaHareketi.TARIH = DateTime.Parse(txtTarih.Text);
                KasaHareketi.TUTAR = decimal.Parse(txtTutar.Text);
                DB.TBL_KASAHAREKETLERIs.InsertOnSubmit(KasaHareketi);
                DB.SubmitChanges();
                Mesajlar.YeniKayit(txtIslemTuru.SelectedItem.ToString() + " Yeni Kasa Hareketi olarak İşlenmiştir ");
                //cARİ hAREKETEDE KAYIT ATIYORUZ
                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = new Fonksiyonlar.TBL_CARIHAREKETLERI();
                CariHareket.ACIKLAMA = txtBelgeNo.Text + " belge numaralı  " + txtIslemTuru.SelectedItem.ToString() + " işlemi ";
                if (txtIslemTuru.SelectedIndex == 0) CariHareket.ALACAK = decimal.Parse(txtTutar.Text);//Kasaya Giriş ise Cariyi borçlandırıyoruz
                if (txtIslemTuru.SelectedIndex == 1) CariHareket.BORC = decimal.Parse(txtTutar.Text);//Kasadan Cıkış ise Cariye borçlanıyoruz
                CariHareket.CARIID = CariID;
                CariHareket.EVRAKID = KasaHareketi.ID;
                CariHareket.EVRAKTURU = txtIslemTuru.SelectedItem.ToString();
                CariHareket.TARIH = DateTime.Parse(txtTarih.Text);
                if (txtIslemTuru.SelectedIndex == 0) CariHareket.TIPI = "KT";//Kasa Tahsilat
                if (txtIslemTuru.SelectedIndex == 1) CariHareket.TIPI = "KÖ";//kASA öDEME
                CariHareket.SAVEDATE = DateTime.Now;
                CariHareket.SAVEUSER = AnaForm.UserID;
                DB.TBL_CARIHAREKETLERIs.InsertOnSubmit(CariHareket);
                DB.SubmitChanges();
                Mesajlar.YeniKayit(txtIslemTuru.SelectedItem.ToString() + " Yeni Cari Hareketi olarak İşlenmiştir ");

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
                Fonksiyonlar.TBL_KASAHAREKETLERI KasaHareketi = DB.TBL_KASAHAREKETLERIs.First(s => s.ID == IslemID);
                
                KasaHareketi.ACIKLAMA = txtAciklama.Text;
                KasaHareketi.BELGENO = txtBelgeNo.Text;
                KasaHareketi.CARIID = CariID;
                KasaHareketi.EVRAKTURU = txtIslemTuru.SelectedItem.ToString();
                if (txtIslemTuru.SelectedIndex == 0) KasaHareketi.GCKODU = "G";//Tahsilatsa Giriş
                if (txtIslemTuru.SelectedIndex == 1) KasaHareketi.GCKODU = "C";//Ödemeyse Çıkış
                KasaHareketi.KASAID = KasaID;
                KasaHareketi.EDITDATE = DateTime.Now;
                KasaHareketi.EDITUSER = AnaForm.UserID;//static değişkeni atadık
                KasaHareketi.TARIH = DateTime.Parse(txtTarih.Text);
                KasaHareketi.TUTAR = decimal.Parse(txtTutar.Text);

                DB.SubmitChanges();
                Mesajlar.Guncelle(true);
                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = DB.TBL_CARIHAREKETLERIs.First(s =>s.ID==CariHareketID);
                //cARİ hAREKETEDE KAYIT ATIYORUZ

                CariHareket.ACIKLAMA = txtBelgeNo.Text + " belge numaralı  " + txtIslemTuru.SelectedItem.ToString() + " işlemi ";
                if (txtIslemTuru.SelectedIndex == 0) CariHareket.ALACAK = decimal.Parse(txtTutar.Text);//Kasaya Giriş ise Cariyi borçlandırıyoruz
                if (txtIslemTuru.SelectedIndex == 1) CariHareket.BORC = decimal.Parse(txtTutar.Text);//Kasadan Cıkış ise Cariye borçlanıyoruz
                CariHareket.CARIID = CariID;
                CariHareket.EVRAKID = KasaHareketi.ID;
                CariHareket.EVRAKTURU = txtIslemTuru.SelectedItem.ToString();
                CariHareket.TARIH = DateTime.Parse(txtTarih.Text);
                if (txtIslemTuru.SelectedIndex == 0) CariHareket.TIPI = "KT";//Kasa Tahsilat
                if (txtIslemTuru.SelectedIndex == 1) CariHareket.TIPI = "KÖ";//kASA öDEME
                CariHareket.EDITDATE = DateTime.Now;
                CariHareket.EDITUSER = AnaForm.UserID;

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
            DB.TBL_KASAHAREKETLERIs.DeleteOnSubmit(DB.TBL_KASAHAREKETLERIs.First(s => s.ID == IslemID));
            DB.TBL_CARIHAREKETLERIs.DeleteOnSubmit(DB.TBL_CARIHAREKETLERIs.First(s => s.ID == CariHareketID));
            DB.SubmitChanges();//değişiklikleri kaydediyoruz
            Temizle();
        }

        public void Ac(int HareketID)
        {
            try
            {
                Edit = true;//güncelleme için
                IslemID = HareketID;
                Fonksiyonlar.TBL_KASAHAREKETLERI KasaHareketi = DB.TBL_KASAHAREKETLERIs.First(s => s.ID == IslemID);
                CariHareketID = DB.TBL_CARIHAREKETLERIs.First(s => s.EVRAKTURU == KasaHareketi.EVRAKTURU && s.EVRAKID == IslemID).ID;
                MessageBox.Show("Cari Hareket ID: "+CariHareketID.ToString());
                txtAciklama.Text = KasaHareketi.ACIKLAMA;
                txtBelgeNo.Text = KasaHareketi.BELGENO;
                if (KasaHareketi.EVRAKTURU == "Kasa Tahsilat")
                {
                    txtIslemTuru.SelectedIndex = 0;
                }
                if (KasaHareketi.EVRAKTURU == "Kasa Ödeme")
                {
                    txtIslemTuru.SelectedIndex = 1;
                }
                txtTarih.Text = KasaHareketi.TARIH.Value.ToShortDateString();
                txtTutar.Text = KasaHareketi.TUTAR.Value.ToString();
                KasaAc(KasaHareketi.KASAID.Value);
                CariAc(KasaHareketi.CARIID.Value);





            }
            catch (Exception ex)
            {
                Edit = false;
                IslemID = -1;
                KasaID = -1;
                CariID=-1;
                CariHareketID = -1;
                Mesajlar.Hata(ex);

            }
        }

        private void txtKasaKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int Id = Formlar.KasaListesi(true);

            if (Id>0)
            {
                KasaAc(Id);
                AnaForm.Aktarma = -1;
            }
        }

        private void txtCariKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int Id = Formlar.CariListesi(true);
            if (Id>0)
            {
                CariAc(Id);
                AnaForm.Aktarma=-1;
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit && IslemID > 0 && CariHareketID > 0 && CariHareketID > 0 && Mesajlar.Guncelle() == DialogResult.Yes)
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
            if (Edit && IslemID > 0 && CariHareketID>0 && CariHareketID>0 && Mesajlar.Sil() == DialogResult.Yes)
            {
                Sil();
            }
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}