using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Linq;//ekledik
namespace Otomasyon.Fonksiyonlar
{
   public class Numara
    {
       DatabaseDataContext DB = new DatabaseDataContext();
       Mesajlar Mesajlar = new Mesajlar();

       //Veritabanındaki son kod numarasına göre bir artırarak kod getirme

       public string StokKodNumarasi()
       {
           try
           {
               int Numara = int.Parse((from s in DB.TBL_STOKLARs
                                           orderby s.ID descending
                                           select s).First().STOKKODU);
               Numara++;
               string Num = Numara.ToString().PadLeft(7, '0');//sol tarafından 7 hane 0 olacak hale getir.
               return Num;

           }
           catch (Exception ex)
           {
               
               return "0000001";

               
           }
       }

       public string CariKodNumarasi()
       {
           try
           {
               int Numara = int.Parse((from s in DB.TBL_CARILERs
                                       orderby s.ID descending
                                       select s).First().CARIKODU);
               Numara++;
               string Num = Numara.ToString().PadLeft(7, '0');//sol tarafından 7 hane 0 olacak hale getir.
               return Num;

           }
           catch (Exception ex)
           {

               return "0000001";


           }
       }

       public string KasaKodNumarasi()
       {
           try
           {
               int Numara = int.Parse((from s in DB.TBL_KASALARs
                                       orderby s.ID descending
                                       select s).First().KASAKODU);
               Numara++;
               string Num = Numara.ToString().PadLeft(7, '0');//sol tarafından 7 hane 0 olacak hale getir.
               return Num;

           }
           catch (Exception ex)
           {

               return "0000001";


           }
       }

    }
}
