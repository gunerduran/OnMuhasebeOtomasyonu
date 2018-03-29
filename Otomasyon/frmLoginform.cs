using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;//Firs ,FirsorDefault,Where gibi komutların gözükmesi için
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Otomasyon
{
    public partial class frmLoginform : DevExpress.XtraEditors.XtraForm
    {
        public frmLoginform()
        {
            InitializeComponent();
            txtKullanici.Focus();//focuslanır
        }
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();

        private void btnGiris_Click(object sender, EventArgs e)
        {
            try
            {
                Fonksiyonlar.TBL_KULLANICILAR Kullanici = DB.TBL_KULLANICILARs.First(s=>s.KULLANICI==txtKullanici.Text.Trim() && s.SIFRE==txtSifre.Text.Trim());
                Kullanici.LASTLOGIN = DateTime.Now; //Kullanıcı boş gelirse zaten burda null reference hatasına düşer(yada kullanıcıyı bulamadı ise Sıra hiçbir öğer içermiyor hatası verir.).böylece giriş yapılamadı hatası verir.
                DB.SubmitChanges();//giriş tarihini güncelliyoruz

                this.Hide();
                AnaForm frm = new AnaForm(Kullanici);//giriş yapan kullanıcıyı yolladık
                frm.Show();
            
            }
            catch (Exception ex)//null reference veya kullanıcı bulunamadı ise sıra hiçbir öğe içermiyor hatası verir.
            {
                MessageBox.Show("Giriş yapılamadı .Kullanıcı adı yada şifreniz geçersiz olabilir .Ltfen düzeltip tekrar deneyiniz");
                return;
            }


           
        }

        private void btnAyar_Click(object sender, EventArgs e)
        {
            frmAyar frm = new frmAyar();
            frm.ShowDialog();
        }
    }
}