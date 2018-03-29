using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ekledik.
using System.Windows.Forms;// MessageBox sınıfını kullanabilmek için


namespace Otomasyon.Fonksiyonlar
{
    public class Mesajlar
    {
        AnaForm MesajForm = new AnaForm();

        public void YeniKayit(string Mesaj)
        {
            MesajForm.Mesaj("Yeni Kayıt Girişi", Mesaj);//Alert Controlle mesaj yazdırma

            // MessageBox.Show(Mesaj,"Yeni Kayıt Girişi",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        public DialogResult Guncelle()
        {


            return MessageBox.Show("Seçili kalıcı olarak güncellenecektir\n Güncelleme işlemini onaylıyormusunuz?", "Güncelleme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        }
        public DialogResult Sil()
        {
            return MessageBox.Show("Seçili olan kayıt kalıcı olarak silinecektir\n Silme işlemini onaylıyormusunuz?", "Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public void Guncelle(bool Guncelleme)
        {
            MesajForm.Mesaj("Kayıt Güncelleme", "Kayıt güncellenmiştir.");//Alert Controlle mesaj yazdırma
            //MessageBox.Show("Kayıt güncellenmiştir.","Kayıt Güncelleme",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        public void Hata(Exception Hata)
        {

            MesajForm.Mesaj("Hata Oluştu", Hata.Message);//Alert Controlle mesaj yazdırma
            // MessageBox.Show(Hata.Message,"Hata Oluştu",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public void FormAcilis(string FormAdi)
        {
            MesajForm.Mesaj("", FormAdi + " formu açıldı. ");
        }

        public void FormKapanis(string FormAdi)
        {
            MesajForm.Mesaj("", FormAdi + " formu kapatıldı. ");
        }


    }
}
