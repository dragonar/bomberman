using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cliens
{
    public partial class belepo : Form
    {
        Thread info_fogado;
        public belepo()
        {
            InitializeComponent();

            info_fogado = new Thread(new ThreadStart(info_fogado_szal));
            info_fogado.Start();
        }

        List<Szerver> SzerverLista = new List<Szerver>();

        void info_fogado_szal()
        {
            UdpClient c = new UdpClient(60001);
            IPEndPoint ep = null;

            while (true)
            {
                if(c.Available>0)
                {
                    byte[] info_csomag = c.Receive(ref ep);

                    Szerver s = null;
                    foreach(Szerver sz in SzerverLista)
                        if (ep.Address.Equals(sz.IPCim))
                            {
                            s = sz;
                            break;
                        }
                        if(s==null)
                    {
                        s = new Szerver();
                        s.IPCim = ep.Address;
                        SzerverLista.Add(s);
                    }
                    using (BinaryReader br = new BinaryReader(new MemoryStream(info_csomag)))
                    {
                      s.Neve=  br.ReadString();
                        s.Jatekban = br.ReadUInt16() > 0;
                        s.JatekosokSzama = br.ReadUInt16();
                        s.UtolsoPingIdeje = DateTime.Now;
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void belepo_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
          


                for(int i= listBox1.Items.Count; i < SzerverLista.Count; i++)
            {
                listBox1.Items.Add("");
            }

            listBox1.BeginUpdate();
            
            for (int i=0;i < SzerverLista.Count;i++)
            {
                listBox1.Items[i] = SzerverLista[i].ToString();
            }

            listBox1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex>=0)
            {
                Szerver s = SzerverLista[listBox1.SelectedIndex];

                Form1 f = new Form1();
                f.SzerverIPCime = s.IPCim;
                f.JatekosNev = textBox1.Text;
                f.Show();
                Hide();



            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                Szerver s = SzerverLista[listBox1.SelectedIndex];
                button1.Enabled = !s.Jatekban;
            }
            else
                button1.Enabled = false;
          
        }
    }
}
