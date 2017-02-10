using Bomberman.KozosKod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        Bitmap buffer;
        Graphics bufferg;

        int palya_szelesseg = 10;
        int palya_magassag = 20;

        CellaTipus[,] Palya;

        void palya_init(int szelesseg, int magassag)
        {

            palya_szelesseg = szelesseg;
            palya_magassag = magassag;

            Palya = new CellaTipus[szelesseg, magassag];

            Palya[0, 0] = CellaTipus.Fal;
            Palya[1, 1] = CellaTipus.Robbanthato_Fal;
            Palya[2, 2] = CellaTipus.Lang_Kartya;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (buffer == null)
                return;

            int cell_width = (buffer.Width - 50) / palya_szelesseg;
            int cell_height = (buffer.Height - 50) / palya_magassag;
            int cell_size = (cell_width < cell_height) ? (cell_width) : (cell_height);

            int offset_x = (buffer.Width - cell_size * palya_szelesseg) / 2;
            int offset_y = (buffer.Height - cell_height * palya_magassag) / 2;

            //vízszintes
            for (int y = 0; y < (palya_magassag + 1); y++)
                bufferg.DrawLine(Pens.Red,
                    offset_x,
                    offset_y + cell_size * y,
                    offset_x + cell_size * palya_szelesseg,
                    offset_y + cell_size * y);

            for (int x = 0; x < (palya_szelesseg + 1); x++)
                bufferg.DrawLine(Pens.Red,
                    offset_x + cell_size * x,
                    offset_y,
                    offset_x + cell_size * x,
                    offset_y + cell_size * palya_magassag);


            for (int i = 0; i < palya_magassag; i++)
                for (int j = 0; j < palya_szelesseg; j++)
                    switch (Palya[j, i])
                    {
                        case CellaTipus.Fal:
                            {
                                bufferg.FillRectangle(Brushes.Gray,
                                    offset_x + cell_size * j,
                                    offset_y + cell_size * i,
                                    cell_size,
                                    cell_size);
                                break;
                             }

                        case CellaTipus.Robbanthato_Fal:
                            {
                                bufferg.FillRectangle(new HatchBrush(HatchStyle.DiagonalBrick,Color.Gray,Color.Red),//HorizontalBrick
                                    offset_x + cell_size * j,
                                    offset_y + cell_size * i,
                                    cell_size,
                                    cell_size);

                                break;
                            }


                }



            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (buffer != null)
                buffer.Dispose();

            buffer = new Bitmap(panel1.Width, panel1.Height);
            bufferg = Graphics.FromImage(buffer);


            panel1.Invalidate();
        }

        TcpClient tcpc;

        private void Form1_Load(object sender, EventArgs e)
        {
            palya_init(10, 20);
            Thread t = new Thread(new ThreadStart(fogadoszal));
            //t.Start();
            panel1.Invalidate();
        }

        private void Log(string Massage)
        {
            listBox1.Invoke((MethodInvoker)(() =>
             {
                 listBox1.Items.Add(Massage);
                 listBox1.SelectedIndex = listBox1.Items.Count - 1;
             }));

        }

        BinaryWriter bw;

        private void fogadoszal()
        {
            tcpc = new TcpClient();
            tcpc.Connect("10.7.51.141", 60000);


            bw = new BinaryWriter(tcpc.GetStream());


            bw.Write((byte)Jatekos_Uzi_Tipusok.Bemutatkozik);
            bw.Write("Marcell");
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

                                    br.ReadString(); // Nev
                                    br.ReadSByte(); // R
                                    br.ReadSByte(); // G
                                    br.ReadSByte(); // B
                                    br.ReadUInt32(); // x
                                    br.ReadUInt32(); // y
                                }

                                break;
                            case Server_Uzi_Tipusok.Palyakep:

                                uint palya_szelesseg = br.ReadUInt32();
                                uint palya_magassag = br.ReadUInt32();

                                byte[] t = br.ReadBytes((int)(palya_magassag * palya_szelesseg));





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
            }
        }
    }
}


