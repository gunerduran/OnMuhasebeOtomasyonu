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

namespace Otomasyon.Modul_Stok
{
    public partial class frmStokKarti : DevExpress.XtraEditors.XtraForm
    {
        bool Edit = false;
        bool Resim = false;

        int GrupID = -1;
        int StokID = -1;


        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Numara Numaralar = new Fonksiyonlar.Numara();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();
        Fonksiyonlar.Resimleme Resimleme = new Fonksiyonlar.Resimleme();

        public frmStokKarti()
        {
            InitializeComponent();
        }

        private void frmStokKarti_Load(object sender, EventArgs e)
        {
            txtStokKodu.Text = Numaralar.StokKodNumarasi();

            Mesajlar.FormAcilis(this.Text);//alert control ile mesaj
        }


        void Temizle()
        {
            txtStokKodu.Text = Numaralar.StokKodNumarasi();
            txtStokAdi.Text = "";
            txtSatisKDV.Text = "";
            txtSatisFiyat.Text = "";
            txtGrupKodu.Text = "";
            txtGrupAdi.Text = "";
            txtBirim.SelectedIndex = 0;
            txtBarkod.Text = "";
            txtAlisKDV.Text = "0";
            txtAlisFiyat.Text = "0";
            pictureBox1.Image = null;
            Edit = false;
            Resim = false;
            GrupID = -1;
            StokID = -1;
            AnaForm.Aktarma = -1;

        }
        OpenFileDialog Dosya = new OpenFileDialog();
        void ResimSec()
        {
            Dosya.Filter = "Jpg(*.jpg)|*.jpg|Jpeg(*.jpeg)|*.jpeg";
            if (Dosya.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = Dosya.FileName;//pictureboxa resmi verdik.
                Resim = true;//resmin seçildiğini belirtmek için
            }
        }
        private void btnResimSec_Click(object sender, EventArgs e)
        {
            ResimSec();
        }
        public void Ac(int ID)
        {
            Edit = true;
            StokID = ID;
            Fonksiyonlar.TBL_STOKLAR Stok = DB.TBL_STOKLARs.First(s => s.ID == StokID);
            GrupAc(Stok.STOKGRUPID.Value);//nullable int'i(int?) normal int yapıyoruz(.Value diyerek)
            if (Stok.STOKRESIM != null)
            {
                pictureBox1.Image = Resimleme.ResimGetirme(Stok.STOKRESIM.ToArray());//Stok.STOKRESIM binary tipindedir.To Array yapınca binary[] array şeklinde oluyor.//byte[] arrayi verip Image tipinde değeri alıyoruz.(yani resmi)
            }
            else
            {
                pictureBox1.Image = null;
            }
            txtAlisFiyat.Text = Stok.STOKALISFIYAT.ToString();
            txtAlisKDV.Text = Stok.STOKALISKDV.ToString();
            txtBarkod.Text = Stok.STOKBARKOD;
            txtBirim.Text = Stok.STOKBIRIM;
            txtSatisFiyat.Text = Stok.STOKSATISFIYAT.ToString();
            txtSatisKDV.Text = Stok.STOKSATISKDV.ToString();
            txtStokAdi.Text = Stok.STOKADI;
            txtStokKodu.Text = Stok.STOKKODU;

        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_STOKLAR Stok = new Fonksiyonlar.TBL_STOKLAR();
                Stok.STOKADI = txtStokAdi.Text;
                Stok.STOKALISFIYAT = decimal.Parse(txtAlisFiyat.Text);
                Stok.STOKALISKDV = decimal.Parse(txtAlisKDV.Text);
                Stok.STOKBARKOD = txtBarkod.Text;
                Stok.STOKBIRIM = txtBirim.Text;
                Stok.STOKGRUPID = GrupID;
                Stok.STOKKODU = txtStokKodu.Text;

                if (Resim)
                    Stok.STOKRESIM = new System.Data.Linq.Binary(Resimleme.ResimYukleme(pictureBox1.Image));//byte arrayi istiyordu bizden byte[] arrayi olarak  bizde verdik. (Veritabanındaki image tipi c# tarafında byte[] arrayi şeklindedir.)

                Stok.STOKSATISFIYAT = decimal.Parse(txtSatisFiyat.Text);
                Stok.STOKSATISKDV = decimal.Parse(txtSatisKDV.Text);
                Stok.STOKSAVEDATE = DateTime.Now;
                Stok.STOKSAVEUSER = AnaForm.UserID;
                DB.TBL_STOKLARs.InsertOnSubmit(Stok);
                DB.SubmitChanges();//değişiklikleri kaydettik.
                Mesajlar.YeniKayit("Yeni Stok Kaydı Oluşturuldu");
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
                Fonksiyonlar.TBL_STOKLAR Stok = DB.TBL_STOKLARs.First(s => s.ID == StokID);
                Stok.STOKADI = txtStokAdi.Text;
                Stok.STOKALISFIYAT = decimal.Parse(txtAlisFiyat.Text);
                Stok.STOKALISKDV = decimal.Parse(txtAlisKDV.Text);
                Stok.STOKBARKOD = txtBarkod.Text;
                Stok.STOKBIRIM = txtBirim.Text;
                Stok.STOKGRUPID = GrupID;
                Stok.STOKKODU = txtStokKodu.Text;
                if (Resim == true)//resim seçildiyse güncelle seçilmediyse güncelleme
                {
                    Stok.STOKRESIM = new System.Data.Linq.Binary(Resimleme.ResimYukleme(pictureBox1.Image));//byte arrayi istiyordu bizden byte[] arrayi olarak  bizde verdik. (Veritabanındaki image tipi c# tarafında byte[] arrayi şeklindedir.)
                }
                Stok.STOKSATISFIYAT = decimal.Parse(txtSatisFiyat.Text);
                Stok.STOKSATISKDV = decimal.Parse(txtSatisKDV.Text);
                Stok.STOKEDITDATE = DateTime.Now;
                Stok.STOKEDITUSER = AnaForm.UserID;

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
                DB.TBL_STOKLARs.DeleteOnSubmit(DB.TBL_STOKLARs.First(s => s.ID == StokID));
                DB.SubmitChanges();//değişiklikleri kaydettik.
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }


        void GrupAc(int ID)
        {
            GrupID = ID;
            txtGrupAdi.Text = DB.TBL_STOKGRUPLARIs.First(s => s.ID == GrupID).GRUPADI;
            txtGrupKodu.Text = DB.TBL_STOKGRUPLARIs.First(s => s.ID == GrupID).GRUPKODU;
        }

        private void txtStokKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int ID = Formlar.StokListesi(true);

            if (ID > 0)
            {
                Ac(ID);

            }

            AnaForm.Aktarma = -1;
        }

        private void txtGrupKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int Id = Formlar.StokGruplari(true);

            if (Id > 0)
            {
                GrupAc(Id);
            }

            AnaForm.Aktarma = -1; // yine eski haline çevirdik static değişkeni 
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit == true && StokID > 0 && Mesajlar.Guncelle() == DialogResult.Yes)
            {
                Guncelle();
            }
            else
            {
                YeniKaydet();
            }

        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();//formu kapat
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit == true && StokID > 0 && Mesajlar.Sil() == DialogResult.Yes)
            {
                Sil();
            }

        }

        private void frmStokKarti_FormClosed(object sender, FormClosedEventArgs e)
        {
            Mesajlar.FormKapanis(this.Text);
        }







    }
}