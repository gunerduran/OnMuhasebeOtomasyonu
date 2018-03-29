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
    public partial class frmBankaHareketleri : DevExpress.XtraEditors.XtraForm
    {


        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();
        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();

        int BankaID = -1;
        int IslemID = -1;
        string EvrakTURU = "";


        public frmBankaHareketleri()
        {
            InitializeComponent();
        }

        private void frmBankaHareketleri_Load(object sender, EventArgs e)
        {

        }


        void Listele()
        {
            //View kullandık
            var lst = from s in DB.VW_BANKAHAREKETLERIs
                      where s.BANKAID==BankaID
                      select s;
            Liste.DataSource = lst;
        }


      public  void BankaAc(int ID)
        {
            try
            {
                //View kullandık
                BankaID = ID;
                Fonksiyonlar.VW_BANKALISTESI Banka = DB.VW_BANKALISTESIs.First(s => s.ID == BankaID);
                txtHesapAdi.Text = Banka.HESAPADI;
                txtHesapNo.Text = Banka.HESAPNO;
                txtCikis.Text = Banka.CIKIS.Value.ToString();//nullable decimal olduğu için Value değeriyle getirdik.
                txtGiris.Text = Banka.GIRIS.Value.ToString();
                txtBakiye.Text = Banka.BAKIYE.Value.ToString();

                Listele();
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
            AnaForm.Aktarma = -1;
        }

        void Sec()
        {
            try
            {
                 IslemID = int.Parse(gridView1.GetFocusedRowCellValue("ID").ToString());
                 EvrakTURU = gridView1.GetFocusedRowCellValue("EVRAKTURU").ToString();
            }
            catch (Exception ex)
            {
                IslemID = -1;
                EvrakTURU = "";

            }
        }



        //context menu strip açılırken
        private void SagTik_Opening(object sender, CancelEventArgs e)
        {
            Sec();
            if (IslemID>0)
            {
                if (EvrakTURU == "Banka İşlem")
                {
                    BankaIslemiDuzenle.Enabled = true;
                    ParaTransferiDuzenle.Enabled = false;
                }
                else if (EvrakTURU == "Banka EFT" || EvrakTURU == "Banka Havale")
                {
                    BankaIslemiDuzenle.Enabled = false;
                    ParaTransferiDuzenle.Enabled = true;
                }  
            }

        }

        private void BankaIslemiDuzenle_Click(object sender, EventArgs e)
        {
            Formlar.BankaIslem(true,IslemID);
            Listele();
        }

        private void ParaTransferiDuzenle_Click(object sender, EventArgs e)
        {
            Formlar.BankaParaTransfer(true, IslemID);
            Listele();
        }

    }
}