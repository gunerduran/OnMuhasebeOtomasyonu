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

//ekledik
using System.Reflection;
using DevExpress.XtraReports.UI;

namespace Otomasyon.Modul_Fatura
{
    public partial class frmSatisFaturasi : DevExpress.XtraEditors.XtraForm
    {
        //NOT:Gridview'in datasource'una bizim oluşsturduğumuz Dataseti(SatisFaturasi datatable) verdik.Çünkü grid üzerinde işlem(grid üzerinde ekleme güncelleme gibi yazı yazma işlemleri başka formdan veriyi getirip gridde gösterme işlemleri gibi) yapabilmek için başta datasource vermek zorundayız.(satisFaturasibindinSource)


        Fonksiyonlar.Formlar Formlar = new Fonksiyonlar.Formlar();
        Fonksiyonlar.DatabaseDataContext DB = new Fonksiyonlar.DatabaseDataContext();
        Fonksiyonlar.Mesajlar Mesajlar = new Fonksiyonlar.Mesajlar();


        int CariID = -1;
        int OdemeID = -1;
        int FaturaID = -1;
        int IrsaliyeID = -1;
        string OdemeYeri = "";
        bool Edit = false;
        bool IrsaliyeAc = false;






        public frmSatisFaturasi(bool Ac, int ID, bool Irsaliye)
        {
            InitializeComponent();
            Edit = Ac;
            if (Irsaliye == true) IrsaliyeID = ID;
            else FaturaID = ID;

            IrsaliyeAc = Irsaliye;

            this.Shown += frmSatisFaturasi_Shown;//bu metodu kullanmamızın amacı sayfadaki tüm veriler getirildikten sonra bu metod çalışır.böylece FaturaGetir metodu çağırılırken gridview1 gibi nesneler daha önceden yüklendiği için bir sıkıntı çıkmayacaktır.

        }

        //sayfa gösterildikten sonra bu metod çalışır(Yani frmSatisFaturasi_Load metodundan sonra buraya düşer.)
        void frmSatisFaturasi_Shown(object sender, EventArgs e)
        {
            if (Edit == true) FaturaGetir();//edit olarak açılmış ise faturayı getir.
        }

        private void frmSatisFaturasi_Load(object sender, EventArgs e)
        {
            txtFaturaTarihi.Text = DateTime.Now.ToShortDateString();
            txtIrsaliyeTarihi.Text = DateTime.Now.ToShortDateString();
        }



        void StokGetir(int StokID)
        {
            try
            {
                Fonksiyonlar.TBL_STOKLAR Stok = DB.TBL_STOKLARs.First(s => s.ID == StokID);

                //getirdiğimiz stoğu gridde gösteriyoruz
                gridView1.AddNewRow();  //F11 yapınca aşağıdaki gridView1_CellValueChanged metoduna gider.çünkü hücre değişikliği algılıyor ve metoda gidiyor
                gridView1.SetFocusedRowCellValue("MIKTAR", 0);  //F11 yapınca aşağıdaki gridView1_CellValueChanged metoduna gider.çünkü hücre değişikliği algılıyor ve metoda gidiyor
                gridView1.SetFocusedRowCellValue("BARKOD", Stok.STOKBARKOD);
                gridView1.SetFocusedRowCellValue("STOKKODU", Stok.STOKKODU);
                gridView1.SetFocusedRowCellValue("STOKADI", Stok.STOKADI);
                gridView1.SetFocusedRowCellValue("BIRIM", Stok.STOKBIRIM);
                gridView1.SetFocusedRowCellValue("BIRIMFIYAT", Stok.STOKSATISFIYAT);
                gridView1.SetFocusedRowCellValue("KDV", Stok.STOKSATISKDV);
            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }


        private void btnStokSec_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //Stoğu getiriyoruz
            int StokID = Formlar.StokListesi(true);

            if (StokID > 0)
            {
                StokGetir(StokID);

            }
            AnaForm.Aktarma = -1;

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                decimal Miktar = 0, BirimFiyat = 0, Toplam = 0;

                //if koşulunu sonsuz döngüyü kırmak için yapıyoruz(An unhandled exception of type 'System.StackOverflowException' occurred in  hatasını gidermek için)
                if (e.Column.Name != "colTOPLAM")//kolon Toplam sutunu değilse
                {
                    Miktar = decimal.Parse(gridView1.GetFocusedRowCellValue("MIKTAR").ToString());//seçili satırdaki Mıktar kolonundaki değeri aldık
                    if (gridView1.GetFocusedRowCellValue("BIRIMFIYAT").ToString() != "") BirimFiyat = decimal.Parse(gridView1.GetFocusedRowCellValue("BIRIMFIYAT").ToString());
                    Toplam = Miktar * BirimFiyat;


                    gridView1.SetFocusedRowCellValue("TOPLAM", Toplam);//F11 yapınca  gridView1_CellValueChanged metoduna gider.yani sürekli metot içinde dolaşır.çünkü hücre değişikliği algılıyor ve metoda gidiyor.İf koşulunu bu yüzden koyduk yoksa sonsuz döngüye girerdi.


                    Hesapla();
                }
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        void Hesapla()
        {
            try
            {
                //griddeki satırlarda gezip hesaplamayı yapcaz
                decimal BirimFiyat = 0, Miktar = 0, GenelToplam = 0, AraToplam = 0, KDV = 0;

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    BirimFiyat = decimal.Parse(gridView1.GetRowCellValue(i, "BIRIMFIYAT").ToString());
                    Miktar = decimal.Parse(gridView1.GetRowCellValue(i, "MIKTAR").ToString());
                    KDV = decimal.Parse(gridView1.GetRowCellValue(i, "KDV").ToString()) / 100 + 1;
                    AraToplam += Miktar * BirimFiyat;
                    GenelToplam += decimal.Parse(gridView1.GetRowCellValue(i, "TOPLAM").ToString()) * KDV;
                }

                txtAraToplam.Text = AraToplam.ToString("0.00");//formatlı yazım
                txtKDV.Text = (GenelToplam - AraToplam).ToString("0.00");
                txtGenelToplam.Text = GenelToplam.ToString("0.00");


            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }

        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            Hesapla();//satır her değiştiğinde hesaplamayı yapcak
        }

        private void txtCariKodu_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int ID = Formlar.CariListesi(true);
            if (ID > 0) CariSec(ID);

            AnaForm.Aktarma = -1;
        }



        void CariSec(int ID)
        {
            try
            {
                CariID = ID;
                Fonksiyonlar.TBL_CARILER Cari = DB.TBL_CARILERs.First(s => s.ID == CariID);
                txtCariKodu.Text = Cari.CARIKODU;
                txtCariAdi.Text = Cari.CARIADI;

            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void txtFaturaTuru_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtFaturaTuru.SelectedIndex == 0)//Açık Fatura
            {
                pnlOdemeYerleri.Enabled = false;
                txtOdemeYeri.Enabled = false;
            }
            else//Kapalı Fatura ise
            {
                pnlOdemeYerleri.Enabled = true;
                txtOdemeYeri.Enabled = true;
            }
        }

        private void txtOdemeYeri_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtOdemeYeri.SelectedIndex == 0)//Kasa ise
            {
                txtHesapAdi.Enabled = false;
                txtHesapNo.Enabled = false;
                txtKasaAdi.Enabled = true;
                txtKasaKodu.Enabled = true;
                OdemeYeri = txtOdemeYeri.Text;
            }


            if (txtOdemeYeri.SelectedIndex == 1)//banka ise
            {
                txtHesapAdi.Enabled = true;
                txtHesapNo.Enabled = true;
                txtKasaAdi.Enabled = false;
                txtKasaKodu.Enabled = false;
                OdemeYeri = txtOdemeYeri.Text;
            }

        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        void YeniFaturaKaydet()
        {
            try
            {


                //Faturayı kaydediyoruz
                Fonksiyonlar.TBL_FATURALAR Fatura = new Fonksiyonlar.TBL_FATURALAR();
                Fatura.ACIKLAMA = txtAciklama.Text;
                Fatura.CARIKODU = txtCariKodu.Text;
                Fatura.FATURANO = txtFaturaNo.Text;
                Fatura.FATURATURU = "Satış Faturası";
                Fatura.GENELTOPLAM = decimal.Parse(txtGenelToplam.Text);
                Fatura.IRSALIYEID = IrsaliyeID;
                Fatura.ODEMEYERI = OdemeYeri;
                Fatura.ODEMEYERIID = OdemeID;
                Fatura.TARIHI = DateTime.Parse(txtFaturaTarihi.Text);
                Fatura.SAVEDATE = DateTime.Now;
                Fatura.SAVEUSER = AnaForm.UserID;



                DB.TBL_FATURALARs.InsertOnSubmit(Fatura);
                DB.SubmitChanges();

                FaturaID = Fatura.ID;//submit changesdan sonra Faturanın ID değeri oluşur ve onuda değişkene atadık

                if (IrsaliyeID < 0)//faturaya ait irsaliye yoksa
                {
                    Fonksiyonlar.TBL_IRSALIYELER Irsaliye = new Fonksiyonlar.TBL_IRSALIYELER();
                    Irsaliye.ACIKLAMA = txtAciklama.Text;
                    Irsaliye.CARIKODU = txtCariKodu.Text;
                    Irsaliye.FATURAID = Fatura.ID;//yukarıdaki submit changesdan sonra Faturanın IDsi oluşuyor ve o ID'yide buraya veriyoruz.
                    Irsaliye.IRSALIYENO = txtIrsaliyeNo.Text;
                    Irsaliye.TARIHI = DateTime.Parse(txtIrsaliyeTarihi.Text);
                    Irsaliye.SAVEDATE = DateTime.Now;
                    Irsaliye.SAVEUSER = AnaForm.UserID;//static değişken



                    DB.TBL_IRSALIYELERs.InsertOnSubmit(Irsaliye);
                    DB.SubmitChanges();

                    IrsaliyeID = Irsaliye.ID;//submit changesdan sonra Irsaliyenin ID değeri oluşur ve onuda değişkene atadık


                    Fatura.IRSALIYEID = IrsaliyeID;//faturaya Irsaliye ıdsinide veriyoruz.irsaliye ıdsini güncelliyoruz.
                }


                //Fatura kalemlerini(detaylarını ) kaydedicez.(Stok hareketlerini kaydedicez)
                Fonksiyonlar.TBL_STOKHAREKETLERI[] StokHareketi = new Fonksiyonlar.TBL_STOKHAREKETLERI[gridView1.RowCount]; //class dizisi
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    StokHareketi[i] = new Fonksiyonlar.TBL_STOKHAREKETLERI();//null reference hatası almamak için new 'ledik
                    StokHareketi[i].BIRIMFIYAT = decimal.Parse(gridView1.GetRowCellValue(i, "BIRIMFIYAT").ToString());
                    StokHareketi[i].FATURAID = Fatura.ID;
                    StokHareketi[i].GCKODU = "C";//SATIŞ FATURASI OLDUĞU İÇİN CIKIŞ YAPTIK
                    StokHareketi[i].IRSALIYEID = IrsaliyeID;
                    StokHareketi[i].KDV = decimal.Parse(gridView1.GetRowCellValue(i, "KDV").ToString());
                    StokHareketi[i].MIKTAR = int.Parse(gridView1.GetRowCellValue(i, "MIKTAR").ToString());
                    StokHareketi[i].STOKKODU = gridView1.GetRowCellValue(i, "STOKKODU").ToString();
                    StokHareketi[i].TIPI = "Satış Faturası";
                    StokHareketi[i].SAVEDATE = DateTime.Now;
                    StokHareketi[i].SAVEUSER = AnaForm.UserID;//static değişşkeni aktardık


                    DB.TBL_STOKHAREKETLERIs.InsertOnSubmit(StokHareketi[i]);


                }

                DB.SubmitChanges();//stok hareketlerini kaydediyoruz

                //Cari hareketide ekliyoruz
                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = new Fonksiyonlar.TBL_CARIHAREKETLERI();
                CariHareket.ACIKLAMA = txtFaturaNo.Text + " nolu satış faturası tutarı";
                if (txtFaturaTuru.SelectedIndex == 0)//Açık Fatura ise
                {
                    CariHareket.ALACAK = 0;
                    CariHareket.BORC = decimal.Parse(txtGenelToplam.Text); //Cariyi Borçlandırıyoruz
                }
                else if (txtFaturaTuru.SelectedIndex == 1)//Kapalı Fatura ise 
                {
                    CariHareket.ALACAK = decimal.Parse(txtGenelToplam.Text); //Cariyi Alacaklandırıyoruz
                    CariHareket.BORC = decimal.Parse(txtGenelToplam.Text); //Cariyi Borçlandırıyoruz
                }


                CariHareket.CARIID = CariID;
                CariHareket.TARIH = DateTime.Now;
                CariHareket.TIPI = "SF";//sATIŞ FATURASI
                CariHareket.EVRAKTURU = "Satış Faturası";
                CariHareket.EVRAKID = Fatura.ID;
                CariHareket.SAVEDATE = DateTime.Now;
                CariHareket.SAVEUSER = AnaForm.UserID;//static değişkeni atadık


                DB.TBL_CARIHAREKETLERIs.InsertOnSubmit(CariHareket);
                DB.SubmitChanges();

                Mesajlar.YeniKayit("Yeni Fatura kaydı başarı ile yapılmıştır.");
                Temizle();

            }
            catch (Exception ex)
            {

                Mesajlar.Hata(ex);
            }
        }


        void Temizle()
        {
            CariID = -1;
            OdemeID = -1;
            FaturaID = -1;
            IrsaliyeID = -1;
            OdemeYeri = "";
            Edit = false;
            IrsaliyeAc = false;
            txtAciklama.Text = "";
            txtAciklama.Text = "0.00";
            txtCariAdi.Text = "";
            txtCariKodu.Text = "";
            txtFaturaNo.Text = "";
            txtFaturaTarihi.Text = DateTime.Now.ToShortDateString();
            txtFaturaTuru.SelectedIndex = 0;
            txtGenelToplam.Text = "0.00";
            txtHesapAdi.Text = "";
            txtHesapNo.Text = "";
            txtIrsaliyeNo.Text = "";
            txtIrsaliyeTarihi.Text = DateTime.Now.ToShortDateString();
            txtKasaAdi.Text = "";
            txtKasaKodu.Text = "";
            txtKDV.Text = "";
            txtOdemeYeri.SelectedIndex = 0;
            AnaForm.Aktarma = -1;

            //gridide boşaltıyoruz
            for (int i = gridView1.RowCount; i > -1; i--)
            {
                gridView1.DeleteRow(i);//satırı sil.


            }

        }


        //güncelleme amaçlı getiriyoruz
        void FaturaGetir()
        {
            try
            {
                Fonksiyonlar.TBL_FATURALAR Fatura = DB.TBL_FATURALARs.First(s => s.ID == FaturaID);

                IrsaliyeID = Fatura.IRSALIYEID.Value;

                txtAciklama.Text = Fatura.ACIKLAMA;
                txtFaturaNo.Text = Fatura.FATURANO;
                if (Fatura.ODEMEYERIID > 0)
                {
                    txtFaturaTuru.SelectedIndex = 1;//Kapalı fatura
                    if (Fatura.ODEMEYERI == "Kasa")
                    {
                        txtOdemeYeri.SelectedIndex = 0;
                        OdemeYeri = Fatura.ODEMEYERI;
                        txtKasaAdi.Text = DB.TBL_KASALARs.First(s => s.ID == Fatura.ODEMEYERIID.Value).KASAADI;
                        txtKasaKodu.Text = DB.TBL_KASALARs.First(s => s.ID == Fatura.ODEMEYERIID.Value).KASAKODU;
                    }
                    else if (Fatura.ODEMEYERI == "Banka")
                    {
                        txtOdemeYeri.SelectedIndex = 1;
                        OdemeYeri = Fatura.ODEMEYERI;
                        txtHesapAdi.Text = DB.TBL_BANKALARs.First(s => s.ID == Fatura.ODEMEYERIID.Value).HESAPADI;
                        txtHesapNo.Text = DB.TBL_BANKALARs.First(s => s.ID == Fatura.ODEMEYERIID.Value).HESAPNO;
                    }
                    OdemeID = Fatura.ODEMEYERIID.Value;//Fatura.ODEMEYERIID nullable int olduğu için Value diyerek değeri aktardık
                }

                else if (Fatura.ODEMEYERIID < 1)
                {
                    txtFaturaTuru.SelectedIndex = 0;
                }

                txtIrsaliyeNo.Text = DB.TBL_IRSALIYELERs.First(s => s.ID == Fatura.IRSALIYEID.Value).IRSALIYENO;
                txtIrsaliyeTarihi.EditValue = DB.TBL_IRSALIYELERs.First(s => s.ID == Fatura.IRSALIYEID.Value).TARIHI.Value.ToShortDateString();



                txtCariAdi.Text = DB.TBL_CARILERs.First(s => s.CARIKODU == Fatura.CARIKODU).CARIADI;
                txtCariKodu.Text = Fatura.CARIKODU;
                txtFaturaTarihi.EditValue = Fatura.TARIHI.Value.ToShortDateString();//nullable olduğu için value olarak alıyoruz


                //Faturanın kalemlerini(detaylarını) getirecez(stok hareketlerini getircez)
                //View kullandık
                var srg = from s in DB.VW_KALEMLERs
                          where s.FATURAID == FaturaID
                          select s;


                foreach (Fonksiyonlar.VW_KALEMLER k in srg)//her bir kalemi(detayı) alıyoru
                {
                    //detayları gride ekliyoruz
                    gridView1.AddNewRow();
                    gridView1.SetFocusedRowCellValue("MIKTAR", k.MIKTAR);
                    gridView1.SetFocusedRowCellValue("BIRIMFIYAT", k.BIRIMFIYAT);
                    gridView1.SetFocusedRowCellValue("KDV", k.KDV);
                    gridView1.SetFocusedRowCellValue("BARKOD", k.STOKBARKOD);
                    gridView1.SetFocusedRowCellValue("STOKKODU", k.STOKKODU);
                    gridView1.SetFocusedRowCellValue("STOKADI", k.STOKADI);
                    gridView1.SetFocusedRowCellValue("BIRIM", k.STOKBIRIM);

                    gridView1.UpdateCurrentRow();//satırı günceller.


                }




            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (Edit == true && FaturaID > 0) FaturaGuncelle();
            else YeniFaturaKaydet();


        }

        private void txtOdemeYeri_EnabledChanged(object sender, EventArgs e)
        {
            if (txtOdemeYeri.Enabled)
            {
                OdemeYeri = txtOdemeYeri.Text;

            }
            else if (!txtOdemeYeri.Enabled)
            {
                OdemeYeri = "";
            }

        }



        void FaturaGuncelle()
        {
            try
            {
                Fonksiyonlar.TBL_FATURALAR Fatura = DB.TBL_FATURALARs.First(s => s.ID == FaturaID);
                Fatura.FATURANO = txtFaturaNo.Text;
                Fatura.ACIKLAMA = txtAciklama.Text;
                Fatura.CARIKODU = txtCariKodu.Text;
                Fatura.GENELTOPLAM = decimal.Parse(txtGenelToplam.Text);
                Fatura.ODEMEYERI = OdemeYeri;
                Fatura.ODEMEYERIID = OdemeID;
                Fatura.EDITDATE = DateTime.Now;
                Fatura.EDITUSER = AnaForm.UserID;

                DB.SubmitChanges();//değişiklikleri güncelliyoruz


                //İrsaliyeleride güncelliyoruz
                Fonksiyonlar.TBL_IRSALIYELER Irsaliye = DB.TBL_IRSALIYELERs.First(s => s.ID == IrsaliyeID);
                Irsaliye.IRSALIYENO = txtIrsaliyeNo.Text;
                Irsaliye.TARIHI = DateTime.Parse(txtIrsaliyeTarihi.Text);
                Irsaliye.EDITDATE = DateTime.Now;
                Irsaliye.EDITUSER = AnaForm.UserID;


                //STOK HAREKETLERİNİ GÜNCELLİYORUZ.(faturaya ait olan stok hareketlerini silip daha sonra ekleme yapıcaz)
                DB.TBL_STOKHAREKETLERIs.DeleteAllOnSubmit(DB.TBL_STOKHAREKETLERIs.Where(s => s.FATURAID == FaturaID));//tüm sok hareketlerini sil

                DB.SubmitChanges();//irsaliyeyi güncelledik. ve stok hareketlerini sildik.



                //Fatura kalemlerini(detaylarını ) kaydedicez.(Stok hareketlerini kaydedicez)
                Fonksiyonlar.TBL_STOKHAREKETLERI[] StokHareketi = new Fonksiyonlar.TBL_STOKHAREKETLERI[gridView1.RowCount]; //class dizisi
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    StokHareketi[i] = new Fonksiyonlar.TBL_STOKHAREKETLERI();//null reference hatası almamak için new 'ledik
                    StokHareketi[i].FATURAID = FaturaID;
                    StokHareketi[i].BIRIMFIYAT = decimal.Parse(gridView1.GetRowCellValue(i, "BIRIMFIYAT").ToString());
                    StokHareketi[i].GCKODU = "C";//SATIŞ FATURASI OLDUĞU İÇİN CIKIŞ YAPTIK
                    StokHareketi[i].IRSALIYEID = IrsaliyeID;
                    StokHareketi[i].KDV = decimal.Parse(gridView1.GetRowCellValue(i, "KDV").ToString());
                    StokHareketi[i].MIKTAR = int.Parse(gridView1.GetRowCellValue(i, "MIKTAR").ToString());
                    StokHareketi[i].STOKKODU = gridView1.GetRowCellValue(i, "STOKKODU").ToString();
                    StokHareketi[i].TIPI = "Satış Faturası";
                    StokHareketi[i].SAVEDATE = DateTime.Now;
                    StokHareketi[i].SAVEUSER = AnaForm.UserID;//static değişşkeni aktardık


                    DB.TBL_STOKHAREKETLERIs.InsertOnSubmit(StokHareketi[i]);


                }

                DB.SubmitChanges();//stok hareketlerini kaydediyoruz.böylece stok hareketlerini güncellemiş oluyoruz

                //Cari hareketlerinide güncelliyoruz
                Fonksiyonlar.TBL_CARIHAREKETLERI CariHareket = DB.TBL_CARIHAREKETLERIs.First(s => s.EVRAKTURU == "Satış Faturası" && s.EVRAKID == FaturaID);
                if (txtFaturaTuru.SelectedIndex == 0)//Açık Fatura
                {
                    CariHareket.ALACAK = 0;
                    CariHareket.BORC = decimal.Parse(txtGenelToplam.Text);
                }
                else if (txtFaturaTuru.SelectedIndex == 1)
                {
                    CariHareket.BORC = decimal.Parse(txtGenelToplam.Text);
                    CariHareket.ALACAK = decimal.Parse(txtGenelToplam.Text);
                }

                CariHareket.EDITDATE = DateTime.Now;
                CariHareket.EDITUSER = AnaForm.UserID;

                DB.SubmitChanges();//değişiklikleri kaydediyoruz

                Mesajlar.Guncelle(true);
                Temizle();
            }
            catch (Exception ex)
            {
                Mesajlar.Hata(ex);
            }
        }

        private void btnYazdir_Click(object sender, EventArgs e)
        {
            //View'i kullanıyoruz
            var srg = DB.VW_FATURALARs.Where(s => s.FATURANO == txtFaturaNo.Text);//fatura noya işlem yapıyoruz

            DataSet ds = new DataSet();
            ds.Tables.Add(LINQToDataTable(srg));


            //fatura noya raporu görüntüledik.
            rprSatisFaturasi rpr = new rprSatisFaturasi();
            rpr.DataSource = ds;  //bunu yorum satırı yaparsak o zaman bütün stok kalemleri gelir.ama böyle yaptığımız zaman yukarıda fatura noya göre stok kalemlerini getirmiş oluyoruz.
            rpr.ShowPreview();//raporun görüntülenmesini sağlar.
            //   rpr.ShowDesigner(); //faturayı düzenleme ekranı açar 
        }


        //linq sorgusunu datasete dönüştüren metod
        public DataTable LINQToDataTable<T>(IEnumerable<T> Lnqlst)
        {
            DataTable dt = new DataTable();


            PropertyInfo[] columns = null;

            if (Lnqlst == null) return dt;

            foreach (T Record in Lnqlst)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }





    }
}