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

namespace Otomasyon.Modul_Kullanici
{
    public partial class frmKullaniciPanel : DevExpress.XtraEditors.XtraForm
    {

        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();



        bool Ac = false;
        int KullaniciID = -1;

        public frmKullaniciPanel(int ID, bool Acc)
        {
            InitializeComponent();

            Ac = Acc;
            KullaniciID = ID;
            if (Ac == true)
            {
                KullaniciGetir(ID);
                txtKullanici.Enabled = false;
            }



        }


        void Temizle()
        {
            txtIsim.Text = "";
            txtKullanici.Text = "";
            txtKullaniciKodu.SelectedIndex = 1;
            txtSifre.Text = "";
            txtSifreT.Text = "";
            txtSoyIsim.Text = "";
            rbtnPasif.Checked = true;
            Ac = false;
            KullaniciID = -1;
        }


        void KullaniciGetir(int ID)
        {
            KullaniciID = ID;

            try
            {
                Fonksiyonlar.TBL_KULLANICILAR Kullanici = DB.TBL_KULLANICILARs.First(s => s.ID == KullaniciID);
                txtIsim.Text = Kullanici.ISIM;
                txtSoyIsim.Text = Kullanici.SOYISIM;
                txtKullanici.Text = Kullanici.KULLANICI;
                txtSifre.Text = Kullanici.SIFRE;
                txtSifreT.Text = Kullanici.SIFRE;
                if (Kullanici.KODU == "Normal") txtKullaniciKodu.SelectedIndex = 1;
                if (Kullanici.KODU == "Yönetici") txtKullaniciKodu.SelectedIndex = 0;
                if (Kullanici.AKTIF.Value == true) rbtnAktif.Checked = true;//nullable olduğu için Value olarak yazıyoruz
                if (Kullanici.AKTIF.Value == false) rbtnPasif.Checked = true;//nullable olduğu için Value olarak yazıyoruz



            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtSifre.Text.Trim() == txtSifreT.Text.Trim())
            {
                if (txtIsim.Text == "")
                {
                    MessageBox.Show("Bir isim girişi yapmak zorundasınız");
                    return;//direk metoddan çık
                }
                else if (txtSoyIsim.Text == "")
                {
                    MessageBox.Show("Bir Soyisim girişi yapmak zorundasınız");
                    return;
                }
                else if (txtKullanici.Text == "")
                {
                    MessageBox.Show("Bir kullanıcı adı girişi yapmak zorundasınız");
                    return;
                }
                else if (txtSifre.Text == "")
                {
                    MessageBox.Show("Bir şifre  girişi yapmadan kullanıcı  açamazsınız");
                    return;
                }


                DialogResult DR = MessageBox.Show(txtKullanici.Text + " türünde bir kullanıcı açmak üzeresiniz.Emin misiniz? ", "Kullanıcı Kaydı tamamlama", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DR == DialogResult.Yes)
                {
                    try
                    {
                        if (!Ac)
                        {
                            if (DB.TBL_KULLANICILARs.Where(s => s.KULLANICI == txtKullanici.Text).Count() > 0)//kullanıcı varsa
                            {
                                MessageBox.Show("Böyle bir kullanıcı zaten açılmış.Aynı kullanıcı adını tekrar kullanamazsınız!!");
                                return;
                            }
                        }


                        Fonksiyonlar.TBL_KULLANICILAR Kullanici;
                        if (!Ac) Kullanici = new Fonksiyonlar.TBL_KULLANICILAR();//güncelleme işlemi için açılmamışsa
                        else Kullanici = DB.TBL_KULLANICILARs.First(s => s.ID == KullaniciID);//güncelleme işlemi için açıldıysa
                        if (rbtnAktif.Checked) Kullanici.AKTIF = true;
                        if (rbtnPasif.Checked) Kullanici.AKTIF = false;
                        Kullanici.ISIM = txtIsim.Text;
                        Kullanici.SOYISIM = txtSoyIsim.Text;
                        Kullanici.KULLANICI = txtKullanici.Text;
                        Kullanici.KODU = txtKullaniciKodu.Text;
                        if (Ac) Kullanici.EDITDATE = DateTime.Now;
                        else Kullanici.SAVEDATE = DateTime.Now;
                        Kullanici.SIFRE = txtSifre.Text;
                        if (!Ac) DB.TBL_KULLANICILARs.InsertOnSubmit(Kullanici);
                        DB.SubmitChanges();//KAYDEDİYORUZ

                        if (Ac) Mesajlar.Guncelle(true);
                        else Mesajlar.YeniKayit(txtKullanici.Text + " kullanıcı kaydı açılmıştır ");
                        Temizle();



                    }
                    catch (Exception ex)
                    {
                        Mesajlar.Hata(ex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Şifreler Aynı Değil");
            }
        }







    }
}