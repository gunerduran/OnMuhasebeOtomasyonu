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
    public partial class frmKullaniciYonetimi : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();


        int secim = -1;
        public frmKullaniciYonetimi()
        {
            InitializeComponent();
            this.Shown += frmKullaniciYonetimi_Shown;  
        }

        //Load eventinden sonra bu event çalışır.Yani tüm kontroller yüklendikten sonra
        void frmKullaniciYonetimi_Shown(object sender, EventArgs e)
        {
            Listele();
        }

        void Listele()
        {
            var lst = from s in DB.TBL_KULLANICILARs
                      select s;

            gridControl1.DataSource = lst;
        }

        private void btnYeniKullanici_Click(object sender, EventArgs e)
        {
            Formlar.KullaniciPanel();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
           
            Formlar.KullaniciPanel(true,secim);
            Listele();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (Mesajlar.Sil() == DialogResult.Yes)
            {
                DB.TBL_KULLANICILARs.DeleteOnSubmit(DB.TBL_KULLANICILARs.First(s => s.ID == secim));
                DB.SubmitChanges();
            }

          
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            secim = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());//seçili satırdaki ID kolonundaki değeri aldık
        }
       
    }
}