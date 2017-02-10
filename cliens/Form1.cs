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

        uint palya_szelesseg = 10;
        uint palya_magassag = 20;
        uint cell_size;

        uint offset_x;
        uint offset_y;

        CellaTipus[,] Palya;




        Dictionary<uint, Jatekos> JatekosLista = new Dictionary<uint, Jatekos>();


        void palya_init(uint szelesseg, uint magassag)
        {

            palya_szelesseg = szelesseg;
            palya_magassag = magassag;

            Palya = new CellaTipus[szelesseg, magassag];

            Palya[0, 0] = CellaTipus.Fal;
            Palya[1, 1] = CellaTipus.Robbanthato_Fal;
            Palya[2, 2] = CellaTipus.Lab_Kartya;
            Palya[3, 5] = CellaTipus.Bomba;
            Palya[5, 5] = CellaTipus.Bomba_Kartya;
            Palya[4, 9] = CellaTipus.Lang;
            Palya[1, 4] = CellaTipus.Lang_Kartya;
            Palya[2, 4] = CellaTipus.Sebesseg_Kartya;
            Palya[6, 5] = CellaTipus.Kesztyu_Kartya;
            Palya[3, 4] = CellaTipus.Halalfej_Kartya;

            JatekosLista[1] = new Jatekos()
            {
                ID = 1,
                Nev = "Test jatekos",
                Szin = Color.FromArgb(0xFF, 0xFF, 0x00),
                x = 2,
                y = 5
            };
        }





        uint CellaX2PixelX(uint CellaX)
        {
            return offset_x + CellaX * cell_size;
        }
        uint CellaY2PixelY(uint CellaY)
        {
            return offset_y + CellaY * cell_size;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (buffer == null)
                return;

            uint cell_width = ((uint)buffer.Width - 50) / palya_szelesseg;
            uint cell_height = ((uint)buffer.Height - 50) / palya_magassag;
            cell_size = (cell_width < cell_height) ? (cell_width) : (cell_height);

            offset_x = ((uint)buffer.Width - cell_size * palya_szelesseg) / 2;
            offset_y = ((uint)buffer.Height - cell_height * palya_magassag) / 2;

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


            for (uint i = 0; i < palya_magassag; i++)
                for (uint j = 0; j < palya_szelesseg; j++)
                    switch (Palya[j, i])
                    {
                        case CellaTipus.Fal:
                            {
                                bufferg.FillRectangle(Brushes.Gray,
                                    CellaX2PixelX(j),
                                    CellaY2PixelY(j),
                                    cell_size,
                                    cell_size);
                                break;
                            }

                        case CellaTipus.Robbanthato_Fal:
                            {
                                bufferg.FillRectangle(new HatchBrush(HatchStyle.DiagonalBrick, Color.Gray, Color.Red),//HorizontalBrick
                                    CellaX2PixelX(j),
                                    CellaY2PixelY(j),
                                    cell_size,
                                    cell_size);

                                break;
                            }
                        case CellaTipus.Lab_Kartya:
                            {

                                bufferg.FillRectangle(Brushes.Cyan,
                                   CellaX2PixelX(j),
                                   CellaY2PixelY(j),
                                   cell_size,
                                   cell_size);

                                string ss = Encoding.UTF32.GetString(BitConverter.GetBytes(0x1f463));

                                Font f = new Font("Segoe UI Symbol", cell_size * 0.6f, FontStyle.Bold);

                                SizeF s = bufferg.MeasureString(ss, f);
                                int sox = ((int)cell_size - (int)s.Width) / 2;
                                int soy = ((int)cell_size - (int)s.Height) / 2;

                                bufferg.DrawString(ss,
                                    f,
                                    Brushes.Black,
                                   CellaX2PixelX(j) + sox,
                                   CellaY2PixelY(j) + soy);

                                break;
                            }
                        case CellaTipus.Bomba:
                            {
                                /*
                                                                bufferg.FillRectangle(Brushes.Cyan,
                                                                   CellaX2PixelX(j,
                                                                   CellaY2PixelY(j,
                                                                   cell_size,
                                                                   cell_size);*/

                                Font f = new Font("wingdings", cell_size * 0.6f, FontStyle.Bold);

                                SizeF s = bufferg.MeasureString("M", f);
                                int sox = ((int)cell_size - (int)s.Width) / 2;
                                int soy = ((int)cell_size - (int)s.Height) / 2;

                                bufferg.DrawString("M",
                                    f,
                                    new SolidBrush(Color.FromArgb(0xff, 0x00, 0xff)),

                                   CellaX2PixelX(j) + sox,
                                   CellaY2PixelY(j )+ soy);

                                break;
                            }


                        case CellaTipus.Bomba_Kartya:
                            {
                                bufferg.FillRectangle(Brushes.Cyan,
                              CellaX2PixelX(j),
                              CellaY2PixelY(j),
                                   cell_size,
                                  cell_size);

                                Font f = new Font("wingdings", cell_size * 0.6f, FontStyle.Bold);

                                SizeF s = bufferg.MeasureString("M", f);
                                int sox = ((int)cell_size - (int)s.Width) / 2;
                                int soy = ((int)cell_size - (int)s.Height) / 2;

                                bufferg.DrawString("M",
                                    f,
                                    new SolidBrush(Color.FromArgb(0xff, 0x00, 0xff)),

                                   CellaX2PixelX(j)+ sox,
                                   CellaY2PixelY(j) + soy);

                                break;
                            }


                    }

            foreach (Jatekos j in JatekosLista.Values.ToArray())
            {
               string ss = Encoding.UTF32.GetString(BitConverter.GetBytes(0x1F603));

                Font f = new Font("Segoe UI Symbol", cell_size * 0.6f, FontStyle.Bold);

                SizeF s = bufferg.MeasureString(ss, f);
                int sox = ((int)cell_size - (int)s.Width) / 2;
                int soy = ((int)cell_size - (int)s.Height) / 2;

                bufferg.DrawString(ss,
                    f,
                    new SolidBrush(j.Szin),
                   CellaX2PixelX(j.x) + sox,
                   CellaY2PixelY(j.y) + soy);

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
            t.Start();
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
             tcpc.Connect("10.0.1.166", 60000);
            //tcpc.Connect("10.7.51.141", 60000);

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
                                    Jatekos j;

                                    if (!JatekosLista.TryGetValue(id, out j))
                                        j = new Jatekos();

                                    j.Nev = br.ReadString(); // Nev

                                    byte r = br.ReadByte(); // R
                                    byte g = br.ReadByte(); // G
                                    byte b = br.ReadByte(); // B


                                    j.Szin = Color.FromArgb(r, g, b);
                                    j.x = br.ReadUInt32(); // x
                                    j.y = br.ReadUInt32(); // y

                                    JatekosLista[id] = j;
                                }




                                break;
                            case Server_Uzi_Tipusok.Palyakep:

                                uint tmp_palya_szelesseg = br.ReadUInt32();
                                uint tmp_palya_magassag = br.ReadUInt32();

                                if (tmp_palya_magassag !=palya_magassag
                                    ||
                                        tmp_palya_szelesseg != palya_szelesseg)
                                    {
                                    Palya = new CellaTipus[tmp_palya_szelesseg, tmp_palya_magassag];
                                    palya_szelesseg = tmp_palya_szelesseg;
                                    palya_magassag = tmp_palya_magassag;
                                       
                                }

                                byte[] t = br.ReadBytes((int)(palya_magassag * palya_szelesseg));

                                for (int i=0, y = 0; y < palya_magassag; y++)
                                    for (int x = 0; x < palya_szelesseg; x++)
                                        Palya[x, y] = (CellaTipus) t[i++];




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


