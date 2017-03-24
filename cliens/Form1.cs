using Bomberman.KozosKod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpClient tcpc;

        private void Form1_Load(object sender, EventArgs e)
        {
         
            Thread t = new Thread(new ThreadStart(fogadoszal));
            t.Start();
        }

        private void Log(string Massage)
        {
            listBox1.Invoke((MethodInvoker)(() =>
             {
                 listBox1.Items.Add(Massage);
                 listBox1.SelectedIndex = listBox1.Items.Count - 1;
             }));

        }


        public IPAddress SzerverIPCime { get; set; }
        public String JatekosNev { get; set; }

        BinaryWriter bw;

        private void fogadoszal()
        {
            tcpc = new TcpClient();          
            tcpc.Connect(SzerverIPCime, 60000);


            bw = new BinaryWriter(tcpc.GetStream());


            bw.Write((byte)Jatekos_Uzi_Tipusok.Bemutatkozik);
            bw.Write(JatekosNev);
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((byte)153);
            bw.Flush();

            using (BinaryReader br = new BinaryReader(tcpc.GetStream()))
            {


                while (true)
                {

                   if (tcpc.GetStream().DataAvailable)
                    {
                        int uzi_tipus = br.ReadByte();
                        switch ((Server_Uzi_Tipusok)uzi_tipus)
                        {
                            case Server_Uzi_Tipusok.Jatekosok_Pozicioja:


                                while (true)
                                {
                                    uint id = br.ReadUInt32();  // ID
                                    if (id == 0)
                                        break;
                                    Jatekos j;

                                    if (!jatekTer1.JatekosLista.TryGetValue(id, out j))
                                        j = new Jatekos();

                                    j.Nev = br.ReadString(); // Nev
                                    j.Ele= br.ReadBoolean();//Ele

                                    byte r = br.ReadByte(); // R
                                    byte g = br.ReadByte(); // G
                                    byte b = br.ReadByte(); // B


                                    j.Szin = Color.FromArgb(r, g, b);
                                    j.x = br.ReadUInt32(); // x
                                    j.y = br.ReadUInt32(); // y

                                   jatekTer1.JatekosLista[id] = j;
                                }




                                break;
                            case Server_Uzi_Tipusok.Palyakep:

                                uint tmp_palya_szelesseg = br.ReadUInt32();
                                uint tmp_palya_magassag = br.ReadUInt32();
                                byte[] t = br.ReadBytes((int)(tmp_palya_magassag * tmp_palya_szelesseg));

                                jatekTer1.Palyakep(tmp_palya_szelesseg, tmp_palya_magassag, t);

                                break;
                            case Server_Uzi_Tipusok.Meghaltal:
                                break;
                            case Server_Uzi_Tipusok.Chat:
                                String s = br.ReadString();
                                Console.WriteLine(s);
                                Log(s);
                                break;
                        }
                    }
                }
            }
        }

        private void textBox1_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bw.Write((byte)Jatekos_Uzi_Tipusok.Chat);
                bw.Write(textBox1.Text);
                bw.Flush();
                textBox1.Text = "";
            }
        }


        private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    {
                        bw.Write((byte)Jatekos_Uzi_Tipusok.Lep_Balra);                       
                        bw.Flush();
                        break;
                    }
                case Keys.S:
                    {
                        bw.Write((byte)Jatekos_Uzi_Tipusok.Lep_Le);                      
                        bw.Flush();
                        break;
                    }
                case Keys.D:
                    {
                        bw.Write((byte)Jatekos_Uzi_Tipusok.Lep_Jobbra);                       
                        bw.Flush();
                        break;
                    }
                case Keys.W:
                    {

                        bw.Write((byte)Jatekos_Uzi_Tipusok.Lep_Fel);                       
                        bw.Flush();
                        break;
                    }
                case Keys.Space:
                    {
                        bw.Write((byte)Jatekos_Uzi_Tipusok.Bombat_rak);                      
                        bw.Flush();
                        break;
                    }
            }


        }

        private void panel1_Click(object sender, EventArgs e)
        {
           jatekTer1.Focus();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void jatekTer1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            jatekTer1.Refresh();
        }
    }
}


