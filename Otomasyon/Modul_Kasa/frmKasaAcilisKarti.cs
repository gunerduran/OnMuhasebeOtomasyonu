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
    public partial class frmKasaAcilisKarti : DevExpress.XtraEditors.XtraForm
    {

        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();//linq to sql
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Numara Numaralar = new Fonksiyonlar.Numara();



        bool Edit = false;//Güncellememi yapılacak yoksa kaydetme mi yapılacak onun bilgisi
        int SecimID = -1;//satır seçmek için gerekli.Ne seçildiğini bulabilmek için////griddeki seçili satırdaki ID bilgisi

        public frmKasaAcilisKarti()
        {
            InitializeComponent();
        }

        private void frmKasaAcilisKarti_Load(object sender, EventArgs e)
        {
            txtKasaKodu.Text = Numaralar.KasaKodNumarasi();//otomatik kasa kod numarasını aldık
            Listele();
        }



        void Temizle()
        {
            txtKasaKodu.Text = Numaralar.KasaKodNumarasi();
            txtKasaAdi.Text = "";
            txtAciklama.Text = "";
            Edit = false;
            SecimID = -1;
            Listele();
        }

        void YeniKaydet()
        {
            try
            {
                Fonksiyonlar.TBL_KASALAR Kasa = new Fonksiyonlar.TBL_KASALAR();
                Kasa.ACIKLAMA = txtAciklama.Text;
                Kasa.KASAADI = txtKasaAdi.Text;
                Kasa.KASAKODU = txtKasaKodu.Text;
                Kasa.SAVEDATE = DateTime.Now;
                Kasa.SAVEUSER = AnaForm.UserID;//static değişkenden geldi.
                DB.TBL_KASALARs.InsertOnSubmit(Kasa);
                DB.SubmitChanges();
                Mesajlar.YeniKayit("Yeni Kasa Kaydı Oluşturulmuştur");
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
                Fonksiyonlar.TBL_KASALAR Kasa = DB.TBL_KASALARs.First(s=>s.ID==SecimID);//önce kasayı getirtip sonra değerlerini değiştirip güncelleme yapıyoruz
                Kasa.ACIKLAMA = txtAciklama.Text;
                Kasa.KASAADI = txtKasaAdi.Text;
                Kasa.KASAKODU = txtKasaKodu.Text;
                Kasa.EDITDATE = DateTime.Now;
                Kasa.EDITUSER = AnaForm.UserID;//static değişkenden geldi.
          
                DB.SubmitChanges();//güncelleme işlemini yapıyoruz
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
                DB.TBL_KASALARs.DeleteOnSubmit(DB.TBL_KASALARs.First(s => s.ID == SecimID));
                DB.SubmitChanges();//değişiklikleri kaydediyoruz

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
                Edit = true;//güncelleme olduğunu belirtmek için

                SecimID =int.Parse( gridView1.GetFocusedRowCellValue("ID").ToString());//griddeki seçili satırdaki ıd bilgisi
                txtKasaAdi.Text = gridView1.GetFocusedRowCellValue("KASAADI").ToString();
                txtKasaKodu.Text = gridView1.GetFocusedRowCellValue("KASAKODU").ToString();
                txtAciklama.Text = gridView1.GetFocusedRowCellValue("ACIKLAMA").ToString();
                
            }
            catch (Exception ex)//birşey seçilemezse
            {
                Edit = false;
                SecimID = -1;
            }
        }

        void Listele()
        {
            var lst = from s in DB.TBL_KASALARs
                      select s;
            Liste.DataSource = lst;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {

            if (txtKasaAdi.Text!="" && txtAciklama.Text!="")
            {

                if (Edit == true && SecimID > 0 && Mesajlar.Guncelle() == DialogResult.Yes)
                {
                    Guncelle();
                }
                else
                {
                    YeniKaydet();
                }
            }
            else
            {
                MessageBox.Show("Kasa Adı ve Açıklama Girilmesi Gereklidir!","İşlem Hatası",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }


        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Edit == true && SecimID > 0 && Mesajlar.Sil() == DialogResult.Yes)
            {
                Sil();
            }
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