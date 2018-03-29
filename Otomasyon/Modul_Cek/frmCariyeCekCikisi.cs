using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;//FirsOrdefault gibi metodların çıkması için
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Otomasyon.Modul_Cek
{
    public partial class frmCariyeCekCikisi : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();


        //açılır buttoneditler için
        int CariID = -1;
        int CekID = -1;

        bool Edit = false;//güncellememi yoksa kaydetmemi için

        public frmCariyeCekCikisi()
        {
            InitializeComponent();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit == true && Mesajlar.Guncelle() == DialogResult.Yes && CariID > 0 && CekID > 0) Guncelle();
            else if (CariID > 0 && CekID > 0) YeniKaydet();
            else MessageBox.Show("Çek veya Cari Seçimi yapmamışsınız");
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            try
            {
                if (Edit == true && Mesajlar.Sil() == DialogResult.Yes && CariID > 0 && CekID > 0)
                {
                    //Cari Hareketi siliyoruz
                    DB.TBL_CARIHAREKETLERIs.DeleteOnSubmit(DB.TBL_CARIHAREKETLERIs.First(s => s.EVRAKTURU == "Cariye Çek" && s.EVRAKID == CekID));




                    Fonksiyonlar.TBL_CEKLER Cek = DB.TBL_CEKLERs.First(s => s.ID == CekID);

                    Cek.VERILENCARI_BELGENO = "";
                    Cek.VERILENCARIID = -1;



                    DB.SubmitChanges();//değişiklikleri kaydetmemiz gerekiyor(güncelleme ve silme birarada)
                }
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

        private void frmCariyeCekCikisi_Load(object sender, EventArgs e)
        {
            txtTarih.Text = DateTime.Now.ToShortDateString();
            txtVadeTarihi.Text = DateTime.Now.ToShortDateString();
        }



        void CariAc(int ID)
        {
            try
            {
                CariID = ID;
                Fonksiyonlar.TBL_CARILER Cari = DB.TBL_CARILERs.First(s => s.ID == CariID);
                txtCariAdi.Text = Cari.CARIADI;
                txtCariKodu.Text = Cari.CARIKODU;
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        void Temizle()
        {
            txtBanka.Text = "";
            txtBelgeNo.Text = "";
            txtCariAdi.Text = "";
            txtCariKodu.Text = "";
            txtCekNo.Text = "";
            txtSube.Text = "";
            txtTarih.Text = DateTime.Now.ToShortDateString();
            txtTutar.Text = "";
            txtVadeTarihi.Text = DateTime.Now.ToShortDateString();
            CariID = -1;
            CekID = -1;
            Edit = false;
            AnaForm.Aktarma = -1;
        }


        void CekGetir(int ID)
        {
            try
            {
                CekID = ID;
                Fonksiyonlar.TBL_CEKLER Cek = DB.TBL_CEKLERs.First(s => s.ID == CekID);
                txtBanka.Text = Cek.BANKA;
                txtCekNo.Text = Cek.CEKNO;
                txtSube.Text = Cek.SUBE;
                txtVadeTarihi.Text = Cek.VADETARIHI.Value.ToShortDateString();//datetime? nullable olduğu için value değerinden gittik.
                txtTutar.Text = Cek.TUTAR.Value.ToString();
                if (Cek.VERILENCARIID != null)
                {
                    if (Cek.VERILENCARIID.Value > 0)
                    {
                        CariAc(Cek.VERILENCARIID.Value);
                        txtBelgeNo.Text = Cek.VERILENCARI_BELGENO;
                        txtTarih.Text = Cek.VERILENCARI_TARIHI.Value.ToShortDateString();//nullable datetime olduğu için value ile değeri aldık.
                    }
                }

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
                Fonksiyonlar.TBL_CEKLER Cek = DB.TBL_CEKLERs.First(s => s.ID == CekID);
                Cek.VERILENCARIID = CariID;
                Cek.VERILENCARI_TARIHI = DateTime.Parse(txtTarih.Text);
                Cek.VERILENCARI_BELGENO = txtBelgeNo.Text;
                Cek.DURUMU = "Caride";//çeki cariye gönderdik
                Cek.EDITDATE = DateTime.Now;
                Cek.EDITUSER = AnaForm.UserID;//static değişken
                DB.SubmitChanges();//güncelliyoruz



                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = new Fonksiyonlar.TBL_CARIHAREKETLERI();//Cari Hareket'de gireceğiz
                CariHareket.ACIKLAMA = txtCekNo.Text + " çek numaralı ve " + txtBelgeNo.Text + " belge numaralı çek";
                CariHareket.BORC = decimal.Parse(txtTutar.Text);  //Çeki cariye çıktığımız için Carinin borçlandırılması lazım
                CariHareket.CARIID = CariID;
                CariHareket.EVRAKID = CekID;
                CariHareket.EVRAKTURU = "Cariye Çek";
                CariHareket.TARIH = DateTime.Now;
                CariHareket.TIPI = "Çek İşlemi";
                CariHareket.SAVEDATE = DateTime.Now;
                CariHareket.SAVEUSER = AnaForm.UserID;

                DB.TBL_CARIHAREKETLERIs.InsertOnSubmit(CariHareket);
                DB.SubmitChanges();


                Mesajlar.YeniKayit("Cariye Çek Çıkışı İşleminin hareket kaydı ve çek kaydı güncellemesi yapılmıştır.");
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
                Fonksiyonlar.TBL_CEKLER Cek = DB.TBL_CEKLERs.First(s => s.ID == CekID);
                Cek.VERILENCARIID = CariID;
                Cek.VERILENCARI_TARIHI = DateTime.Parse(txtTarih.Text);
                Cek.VERILENCARI_BELGENO = txtBelgeNo.Text;
                Cek.DURUMU = "Caride";//çeki cariye gönderdik
                Cek.EDITDATE = DateTime.Now;
                Cek.EDITUSER = AnaForm.UserID;//static değişken
                DB.SubmitChanges();//güncelliyoruz


                //Cari Hareket'de güncellicez
                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = DB.TBL_CARIHAREKETLERIs.First(s => s.EVRAKTURU == "Cariye Çek" && s.EVRAKID == CekID);
                CariHareket.ACIKLAMA = txtCekNo.Text + " çek numaralı ve " + txtBelgeNo.Text + " belge numaralı çek";
                CariHareket.BORC = decimal.Parse(txtTutar.Text);  //Çeki cariye çıktığımız için Carinin borçlandırılması lazım
                CariHareket.CARIID = CariID;
                CariHareket.EVRAKID = CekID;
                CariHareket.EVRAKTURU = "Cariye Çek";
                CariHareket.TARIH = DateTime.Now;
                CariHareket.TIPI = "Çek İşlemi";
                CariHareket.EDITDATE = DateTime.Now;
                CariHareket.EDITUSER = AnaForm.UserID;


                DB.SubmitChanges();//güncelleme yapıyoruz


                Mesajlar.Guncelle(true);
                Temizle();
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }


        public void Ac(int CekinIDsi)
        {
            try
            {
                CekID = CekinIDsi;
                CekGetir(CekID);
                Edit = true;//güncelleme için
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
                Temizle();
            }
        }

        private void txtBelgeNo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void txtCariKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int id = Formlar.CariListesi(true);
            if (id > 0) CariAc(id);
            AnaForm.Aktarma = -1;
        }

        private void txtCekNo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int id = Formlar.CekListesi(true);
            if (id > 0) CekGetir(id);
            AnaForm.Aktarma = -1;
        }

    }
}