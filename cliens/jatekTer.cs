using Bomberman.KozosKod;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cliens
{
    class jatekTer : Control
    {

        Bitmap buffer;
        Graphics bufferg;

        uint palya_szelesseg = 10;
        uint palya_magassag = 20;

        uint cell_size;
        uint offset_x;
        uint offset_y;

        uint CellaX2PixelX(uint CellaX)
        {
            return offset_x + CellaX * cell_size;
        }
        uint CellaY2PixelY(uint CellaY)
        {
            return offset_y + CellaY * cell_size;
        }

      public  CellaTipus[,] Palya;

       public Dictionary<uint, Jatekos> JatekosLista = new Dictionary<uint, Jatekos>();

        public void palya_init(uint szelesseg, uint magassag)
        {
            palya_szelesseg = szelesseg;
            palya_magassag = magassag;

            Palya = new CellaTipus[szelesseg, magassag];

            Palya[0, 0] = CellaTipus.Fal;
            Palya[1, 1] = CellaTipus.Robbanthato_Fal;
            Palya[2, 2] = CellaTipus.Lab_Kartya;
            Palya[3, 5] = CellaTipus.Bomba;
            Palya[5, 5] = CellaTipus.Bomba_Kartya;
            /* Palya[4, 9] = CellaTipus.Lang;
             Palya[1, 4] = CellaTipus.Lang_Kartya;
             Palya[2, 4] = CellaTipus.Sebesseg_Kartya;
             Palya[6, 5] = CellaTipus.Kesztyu_Kartya;
             Palya[3, 4] = CellaTipus.Halalfej_Kartya;*/

            JatekosLista[1] = new Jatekos()
            {
                ID = 1,
                Nev = "Test jatekos",
                Szin = Color.FromArgb(0xFF, 0xFF, 0x00),
                x = 1,
                y = 1
            };
        }

        public void Palyakep(uint Szelesseg,uint Magassag,byte[] Adatok)
        {

            if (Magassag != palya_magassag
                ||
               Szelesseg != palya_szelesseg)
            {
                Palya = new CellaTipus[Szelesseg, Magassag];
                palya_szelesseg = Szelesseg;
                palya_magassag = Magassag;

            }

            for (int i = 0, y = 0; y < palya_magassag; y++)
                for (int x = 0; x < palya_szelesseg; x++)
                    Palya[x, y] = (CellaTipus)Adatok[i++];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (buffer == null)
                return;
            bufferg.Clear(Color.Green);

            try
            {

                uint cell_width = ((uint)buffer.Width - 150) / palya_szelesseg;
                uint cell_height = ((uint)buffer.Height - 150) / palya_magassag;
                cell_size = (cell_width < cell_height) ? (cell_width) : (cell_height);

                offset_x = ((uint)buffer.Width - cell_size * palya_szelesseg) / 2;
                offset_y = ((uint)buffer.Height - cell_height * palya_magassag) / 2;

                //vízszintes
                /* for (int y = 0; y < (palya_magassag + 1); y++)
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
                 */            

                for (uint i = 0; i < palya_magassag; i++)
                    for (uint j = 0; j < palya_szelesseg; j++)
                        switch (Palya[j, i])
                        {
                            case CellaTipus.Fal:
                                {
                                    bufferg.FillRectangle(Brushes.Gray,
                                        CellaX2PixelX(j),
                                        CellaY2PixelY(i),
                                        cell_size,
                                        cell_size);
                                    break;
                                }

                            case CellaTipus.Robbanthato_Fal:
                                {
                                    bufferg.FillRectangle(new HatchBrush(HatchStyle.DiagonalBrick, Color.Gray, Color.Red),//HorizontalBrick
                                        CellaX2PixelX(j),
                                        CellaY2PixelY(i),
                                        cell_size,
                                        cell_size);

                                    break;
                                }
                            case CellaTipus.Lab_Kartya:
                                {

                                    bufferg.FillRectangle(Brushes.Cyan,
                                       CellaX2PixelX(j),
                                       CellaY2PixelY(i),
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
                                       CellaY2PixelY(i) + soy);

                                    break;
                                }
                            case CellaTipus.Bomba:
                                {
                                    Font f = new Font("wingdings", cell_size * 0.6f, FontStyle.Bold);

                                    SizeF s = bufferg.MeasureString("M", f);
                                    int sox = ((int)cell_size - (int)s.Width) / 2;
                                    int soy = ((int)cell_size - (int)s.Height) / 2;

                                    bufferg.DrawString("M",
                                        f,
                                        new SolidBrush(Color.FromArgb(0xff, 0x00, 0xff)),

                                       CellaX2PixelX(j) + sox,
                                       CellaY2PixelY(i) + soy);
                                    break;
                                }
                            /*      case CellaTipus.Bomba_Kartya:
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
                                             CellaY2PixelY(i) + soy);

                                          break;
                                      }
                                      */

                            case CellaTipus.Lang:
                                {
                                    string ss = Encoding.UTF32.GetString(BitConverter.GetBytes(0x1f525));

                                    Font f = new Font("Segoe UI Symbol", cell_size * 0.6f, FontStyle.Bold);

                                    SizeF s = bufferg.MeasureString(ss, f);
                                    int sox = ((int)cell_size - (int)s.Width) / 2;
                                    int soy = ((int)cell_size - (int)s.Height) / 2;

                                    bufferg.DrawString(ss,
                                        f,
                                        Brushes.Black,
                                       CellaX2PixelX(j) + sox,
                                       CellaY2PixelY(i) + soy);
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
            }
            catch { }

            e.Graphics.DrawImage(buffer, 0, 0);
        }

            
       protected override void OnEnabledChanged(EventArgs pever)
        {           
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (buffer != null)
                buffer.Dispose();

            buffer = new Bitmap(Width, Height);
            bufferg = Graphics.FromImage(buffer);


            Invalidate();

        }
    }
}
