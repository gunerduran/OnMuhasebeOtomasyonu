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
    public partial class frmCariListesi : DevExpress.XtraEditors.XtraForm
    {
        public bool Secim = false;
        int SecimID = -1;

        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        public frmCariListesi()
        {
            InitializeComponent();
        }

        private void frmCariListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }


        void Listele()
        {
            var lst = from s in DB.TBL_CARILERs
                      where s.CARIADI.Contains(txtCariAdi.Text) && s.CARIKODU.Contains(txtCariKodu.Text)
                      select s;

            Liste.DataSource = lst;//gridvontrole verileri bastık
        }

        void Sec()
        {
            try
            {
                SecimID = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());//SEÇİLİ SATIRDAKI DEĞER 
            }
            catch (Exception ex)
            {
                SecimID = -1;


            }
        }


        private void Liste_DoubleClick(object sender, EventArgs e)
        {
            Sec();
            if (Secim==true && SecimID>0 )
            {
                AnaForm.Aktarma = SecimID;//formlar arası veri transferi için Aktarma static değişkenini kullanıyoruz.böylece heryerden erişim sağlıyabileceğiz.
                this.Close();
            }
        }


    }
}