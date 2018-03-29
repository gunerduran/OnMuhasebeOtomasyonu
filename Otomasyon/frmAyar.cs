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

namespace Otomasyon
{
    public partial class frmAyar : DevExpress.XtraEditors.XtraForm
    {
        public frmAyar()
        {
            InitializeComponent();
        }

        private void chkYeniGiris_CheckedChanged(object sender, EventArgs e)
        {
            //false ise true,true ise false yapar.
            txtDatabase.Enabled = !txtDatabase.Enabled;
            txtPassword.Enabled = !txtPassword.Enabled;
            txtSunucu.Enabled = !txtSunucu.Enabled;
            txtUserID.Enabled = !txtUserID.Enabled;
            btnYeniAyarlariKaydet.Enabled = !btnYeniAyarlariKaydet.Enabled;
        }

        private void btnYeniAyarlariKaydet_Click(object sender, EventArgs e)
        {
            //Projeye sağ tıklayıp Properties kısmına basarsan bu ayarları görürsün
            Properties.Settings.Default.cs_Sunucu = txtSunucu.Text.Trim() + ";";
            Properties.Settings.Default.cs_Database = txtDatabase.Text.Trim() + ";";
            Properties.Settings.Default.cs_UserID = txtUserID.Text.Trim() + ";";
            Properties.Settings.Default.cs_Password = txtPassword.Text.Trim() + ";";
            Properties.Settings.Default.Database = txtDatabase.Text.Trim();
            Properties.Settings.Default.Save();//ayarları kaydediyoruz.

            //Application.Restart();//uygulamayı restart yapmamız gerekiyor.Ama debug modda restart işlemi yapmıyor.Yani kapandığı zaman tekrar açmıyor.



            this.Close();



        }

        private void frmAyar_Load(object sender, EventArgs e)
        {
            //connection string gösteriyoruz.
            labelControl1.Text = Properties.Settings.Default.cs1 + Properties.Settings.Default.cs_Sunucu + Properties.Settings.Default.cs2 + Properties.Settings.Default.cs_Database + Properties.Settings.Default.cs3 + Properties.Settings.Default.cs_UserID + Properties.Settings.Default.cs4 + Properties.Settings.Default.cs_Password;
        
        
        
        
        }
    }
}