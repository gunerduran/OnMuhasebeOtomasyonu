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

namespace Otomasyon.Modul_Fatura
{
    public partial class frmFaturaListesi : DevExpress.XtraEditors.XtraForm
    {
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();


        bool Secimm = false;
        public frmFaturaListesi(bool Secim)
        {
            InitializeComponent();
            Secimm = Secim;

        }

        private void frmFaturaListesi_Load(object sender, EventArgs e)
        {
            Listele();
        }


        void Listele()
        {
            var lst = from s in DB.TBL_FATURALARs
                      where s.FATURATURU.Contains(txtFaturaTuru.Text) && s.FATURANO.Contains(txtFaturaNo.Text)
                      select s;

            Liste.DataSource = lst;
        }

        private void txtFaturaTuru_SelectedIndexChanged(object sender, EventArgs e)
        {
            Listele();
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int ID = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());

            if (ID > 0)
            {
                Formlar.Fatura(true,ID,false);
            }


        }

        private void SagTikYenile_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void SagTik_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}