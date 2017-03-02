using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

/*This program was developed to make easier to obtain the sum of extra hours.
 * Input: CSV file lines
 * It's mainly designed regarding manipulation of TimeSpan objects.
 * Author: Caio Moraes
 * GitHub: MoraesCaio
 * Email: caiomoraes@msn.com
 **/
namespace Horas_Extras
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Check if file exists.
            //Warns the user if it doesn't, while stoping the execution
            string csvFile = textBox1.Text;
            if (!csvFile.EndsWith(".csv"))
            {
                csvFile += ".csv";
            }
            if (!File.Exists(csvFile)){
                MessageBox.Show("Arquivo não existe!",
                    "Aviso!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            //Setting oficial hours of the schedule
            ArrayList texts = new ArrayList();
            texts.Add(textBox2.Text);
            texts.Add(textBox3.Text);
            texts.Add(textBox4.Text);
            texts.Add(textBox5.Text);

            for(int i = 0; i < texts.Count; i++){
                if (!Schedule.SetOfficialHour(texts[i].ToString(), i)){
                    MessageBox.Show("O horário " + texts[i] + " não está no formato hh:mm. " +
                         "Por favor, insira um valor válido",
                        "Aviso!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            //Reading file
            string[] lines = File.ReadAllLines(csvFile);

            //Adding extra hours
            TimeSpan total = TimeSpan.Zero;
            total = Schedule.AddExtraHours(lines);

            //Printing to the user
            string result = string.Format("{0}{1} dias {2} horas {3} minutos",
                (total < TimeSpan.Zero) ? "-" : "+",
                total.Days,
                total.Hours,
                total.Minutes
                );
            MessageBox.Show(result+
                "\n\nLegenda:"+
                /*"\n   Formato: dias.h:m"+*/
                "\n   +: horas extras disponíveis"+
                "\n   - : horas extras em débito"
                ,"Total"
                );
        }
    }
}
