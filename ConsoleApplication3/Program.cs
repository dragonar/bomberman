using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using Bomberman.KozosKod;



namespace ConsoleApplication3
{
    class Program
    {
        static Cella[,] Palya;
        static uint palya_szelesseg;
        static uint palya_magassag;
        static Random r = new Random();
        static double kezdeti_palya_telitettseg_faktor = 0.7;
        static uint Jatekos_ID_Szamlalo = 1;
        static Dictionary<uint, Jatekos> Jatekosok = new Dictionary<uint, Jatekos>();
        static uint Bomba_ID_Szamlalo = 1;
        static Dictionary<uint, Bomba> Bombak = new Dictionary<uint, Bomba>();
        static uint Lang_ID_Szamlalo = 1;
        static Dictionary<uint, Lang> Langok = new Dictionary<uint, Lang>();

        static void palya_init(uint pszelesseg, uint pmagassag)
        {
            palya_szelesseg = pszelesseg;
            palya_magassag = pmagassag;

            Palya = new Cella[palya_szelesseg, palya_magassag];

            for (uint i = 0; i < palya_szelesseg; i++)
                for (uint j = 0; j < palya_magassag; j++)
                    Palya[i, j].Tipus = CellaTipus.Ures;

            for (uint i = 0; i < palya_szelesseg; i++)
            {
                Palya[i, 0].Tipus = CellaTipus.Fal;
                Palya[i, palya_magassag - 1].Tipus = CellaTipus.Fal;
            }

            for (uint i = 0; i < palya_magassag; i++)
            {
                Palya[0, i].Tipus = CellaTipus.Fal;
                Palya[palya_szelesseg - 1, i].Tipus = CellaTipus.Fal;
            }

            for (uint i = 2; i < palya_szelesseg; i += 2)
                for (uint j = 2; j < palya_magassag; j += 2)
                    Palya[i, j].Tipus = CellaTipus.Fal;

            for (uint y = 0; y < palya_magassag; y++)
                for (uint x = 0; x < palya_szelesseg; x++)
                    if (Palya[x, y].Tipus == CellaTipus.Ures)
                        if (r.NextDouble() < kezdeti_palya_telitettseg_faktor)
                            Palya[x, y].Tipus = CellaTipus.Robbanthato_Fal;
        }

        static void palya_kirajzol()
        {
            for (uint y = 0; y < palya_magassag; y++)
            {
                for (uint x = 0; x < palya_szelesseg; x++)
                {
                    Console.SetCursorPosition((int)x, (int)y);
                    Console.ForegroundColor = ConsoleColor.White;
                    switch (Palya[x, y].Tipus)
                    {
                        case CellaTipus.Ures: Console.ForegroundColor = ConsoleColor.DarkGreen; Console.Write(' '); break;
                        case CellaTipus.Fal: Console.ForegroundColor = ConsoleColor.Gray; Console.Write('█'); break; //219
                        case CellaTipus.Robbanthato_Fal: Console.ForegroundColor = ConsoleColor.Cyan; Console.Write('▒'); break; //177
                        case CellaTipus.Bomba: Console.ForegroundColor = ConsoleColor.Magenta; Console.Write('☼'); break;
                        case CellaTipus.Lang: Console.ForegroundColor = ConsoleColor.Red; Console.Write('x'); break;
                        default: Console.Write('?'); break;
                    }
                }
            }

            foreach (Jatekos j in Jatekosok.Values.ToList())
            {
                Console.SetCursorPosition((int)j.x, (int)j.y);
                Console.Write(j.Nev);
            }
        }

        static void jatekos_pozicio_generalas()
        {
            uint x_db = (palya_szelesseg - 2 - 1) / 2;
            uint y_db = (palya_magassag - 2 - 1) / 2;

            for (int i = 0; i < Jatekosok.Count; i++)
            {
                while (true)
                {
                    uint x = (uint)(1 + r.Next((int)x_db) * 2);
                    uint y = (uint)(1 + r.Next((int)y_db) * 2);

                    bool talaltunke = false;

                    for (int j = 0; j < i; j++)
                    {
                        Jatekos jj = Jatekosok.Values.ElementAt(j);
                        if (jj.x == x && jj.y == y)
                        {
                            talaltunke = true;
                            break;
                        }
                    }

                    if (!talaltunke)
                    {
                        Jatekos jj = Jatekosok.Values.ElementAt(i);
                        jj.x = x;
                        jj.y = y;
                       // Jatekosok[jj.ID] = jj;
                        Palya[x, y].Tipus = CellaTipus.Ures;
                        Palya[x + 1, y].Tipus = CellaTipus.Ures;
                        Palya[x, y + 1].Tipus = CellaTipus.Ures;
                        break;
                    }
                }

            }

            /*foreach (Jatekos j in Jatekosok.Values.ToList())
                Console.WriteLine("ID={0} Nev={1} x={2} y={3}", j.ID, j.Nev, j.x, j.y);
                */
        }

        static void bomba_telepites(uint jatekos_ID, uint bomba_x, uint bomba_y)
        {
            Jatekos j;
            if (!Jatekosok.TryGetValue(jatekos_ID, out j))
                return;

            if (bomba_x >= palya_szelesseg || bomba_y >= palya_magassag)
                return;

            if (Palya[bomba_x, bomba_y].Tipus != CellaTipus.Ures)
                return;

            Bomba b = new Bomba
            {
                ID = Bomba_ID_Szamlalo++,
                Szin = j.Szin,
                Rendzs = j.Rendzs,
                Mikor_Robban = DateTime.Now.AddMilliseconds(3000),
                Jatekos_ID = j.ID,
                x = bomba_x,
                y = bomba_y
            };

            Bombak.Add(b.ID, b);

            Palya[bomba_x, bomba_y].Tipus = CellaTipus.Bomba;
            Palya[bomba_x, bomba_y].Bomba_ID = b.ID;
        }

        static void bomba_check()
        {
            foreach (Bomba b in Bombak.Values.ToList())
                if (b.Mikor_Robban <= DateTime.Now)
                    bomba_robban(b.ID);
        }

        static void bomba_robban(uint bomba_id)
        {
            Bomba b;
            if (!Bombak.TryGetValue(bomba_id, out b))
                return;

            Bombak.Remove(b.ID);

            Palya[b.x, b.y].Tipus = CellaTipus.Ures;
            lang_telepit(b.x, b.y, b);

            uint x = b.x;
            uint y = b.y;

            bool felmehete = true;
            bool jobbramehet = true;
            bool lemehet = true;
            bool balramehet = true;

            for (uint i = 1; i <= b.Rendzs; i++)
            {
                if (felmehete)
                    felmehete = lang_telepit(b.x, b.y - i, b);
                if (jobbramehet)
                    jobbramehet = lang_telepit(b.x + i, b.y, b);
                if (lemehet)
                    lemehet = lang_telepit(b.x, b.y + i, b);
                if (balramehet)
                    balramehet = lang_telepit(b.x - i, b.y, b);
            }
        }

        static bool lang_telepit(uint lang_x, uint lang_y, Bomba b)
        {
            if (lang_x >= palya_szelesseg || lang_y >= palya_magassag)
                return false;

            switch (Palya[lang_x, lang_y].Tipus)
            {
                case CellaTipus.Ures:
                    {
                        Lang l = new Lang
                        {
                            ID = Lang_ID_Szamlalo++,
                            Szin = b.Szin,
                            Meddig = DateTime.Now.AddMilliseconds(1000),
                            Jatekos_ID = b.Jatekos_ID,
                            x = lang_x,
                            y = lang_y
                        };

                        Langok.Add(l.ID, l);

                        Palya[lang_x, lang_y].Tipus = CellaTipus.Lang;
                        Palya[lang_x, lang_y].Lang_ID = l.ID;
                        return true;
                    }
                case CellaTipus.Fal:
                    {
                        return false;
                    }
                case CellaTipus.Lang:
                    {
                        Langok.Remove(Palya[lang_x, lang_y].Lang_ID);

                        Lang l = new Lang
                        {
                            ID = Lang_ID_Szamlalo++,
                            Szin = b.Szin,
                            Meddig = DateTime.Now.AddMilliseconds(1000),
                            Jatekos_ID = b.Jatekos_ID,
                            x = lang_x,
                            y = lang_y
                        };

                        Langok.Add(l.ID, l);

                        Palya[lang_x, lang_y].Lang_ID = l.ID;
                        return true;
                    }
                case CellaTipus.Bomba:
                    {
                        bomba_robban(Palya[lang_x, lang_y].Bomba_ID);
                        return false;
                    }
                case CellaTipus.Robbanthato_Fal:
                    {
                        Palya[lang_x, lang_y].Tipus = CellaTipus.Ures;
                        kartya_telepit(lang_x, lang_y, false);
                        return false;
                    }
                default:
                    {
                        Palya[lang_x, lang_y].Tipus = CellaTipus.Ures;
                        return false;
                    }
            }

        }

        static void lang_check()
        {
            foreach (Lang l in Langok.Values.ToList())
                if (l.Meddig < DateTime.Now)
                {
                    Langok.Remove(l.ID);
                    Palya[l.x, l.y].Tipus = CellaTipus.Ures;
                }
        }

        struct KartyaSuly
        {
            public CellaTipus KartyaTipus;
            public double Suly;
        };

        static List<KartyaSuly> KartyaSulyok = new List<KartyaSuly>
        {
            new KartyaSuly() {
                KartyaTipus = CellaTipus.Bomba_Kartya,
                Suly = 0.3
            },
            new KartyaSuly() {
                KartyaTipus = CellaTipus.Lang_Kartya,
                Suly = 0.3
            },
                        new KartyaSuly() {
                KartyaTipus = CellaTipus.Halalfej_Kartya,
                Suly = 0.1
            },
                                    new KartyaSuly() {
                KartyaTipus = CellaTipus.Sebesseg_Kartya,
                Suly = 0.1
            },
                                                new KartyaSuly() {
                KartyaTipus = CellaTipus.Lab_Kartya,
                Suly = 0.1
            },
                                                            new KartyaSuly() {
                KartyaTipus = CellaTipus.Kesztyu_Kartya,
                Suly = 0.1
            }
        };

        static void kartya_telepit(uint kartya_x, uint kartya_y, bool force)
        {
            if (kartya_x >= palya_szelesseg || kartya_y >= palya_magassag)
                return;

            if (Palya[kartya_x, kartya_y].Tipus != CellaTipus.Ures)
                return;

            // generáljunk kártyát?
            if (r.NextDouble() > 0.3 && !force)
                return;

            double valszeg = r.NextDouble();

            double also_hatar = 0;

            foreach (KartyaSuly ks in KartyaSulyok)
            {
                if (valszeg < (also_hatar + ks.Suly))
                {
                    Palya[kartya_x, kartya_y].Tipus = ks.KartyaTipus;
                    return;
                }
                else
                    also_hatar += ks.Suly;
            }
        }

        static void Main(string[] args)
        {

            TcpListener tl = new TcpListener(60000);
            tl.Start();//halgatozás inditása

            palya_init(35, 35);
            // jatekos_pozicio_generalas



            while (true)//csatlakozos cikls
            {
                if (tl.Pending())
                {
                    Jatekos j = new Jatekos()
                    {
                        ID = Jatekos_ID_Szamlalo++,
                        Nev = "",
                        Rendzs = 1,
                        Maxbombaszam = 1,
                        Ele = true,
                        Sebesseg = 1,
                        Uzisor = new ConcurrentQueue<string>(),
                        tcp = tl.AcceptTcpClient(),
                        thread = new Thread(new ParameterizedThreadStart(jatekos_szal))
                    };
                    Jatekosok.Add(j.ID, j);

                    j.thread.Start(j);
                }
                if (Console.KeyAvailable)
                    if (Console.ReadKey().KeyChar == 's')
                        break;
            }

            jatekos_pozicio_generalas();

            while (true)
            {
                bomba_check();
                lang_check();
                // palya_kirajzol();
                System.Threading.Thread.Sleep(50);
            }
        }
        static bool jatekos_lep(uint uj_x, uint uj_y, Jatekos j)
        {
            if (
                   uj_y < 0
                   ||
                   uj_x < 0
                   ||
                   uj_y >= palya_magassag
                   ||
                   uj_x >= palya_szelesseg
                   )
                return false;

            lock (Palya)
            {
                switch (Palya[uj_x, uj_y - 1].Tipus)
                {
                    case CellaTipus.Ures: break;
                    case CellaTipus.Fal: return false; ;
                    case CellaTipus.Robbanthato_Fal: return false; ;
                    case CellaTipus.Bomba: return false; ;
                    case CellaTipus.Lang:
                        j.Ele = false;
                        break;
                    case CellaTipus.Bomba_Kartya:
                        j.Maxbombaszam += 1;
                        Palya[uj_x, uj_y].Tipus = CellaTipus.Ures;
                        break;
                    case CellaTipus.Lang_Kartya:
                        j.Rendzs += 1;
                        Palya[uj_x, uj_y].Tipus = CellaTipus.Ures;
                        break;
                    case CellaTipus.Halalfej_Kartya: break;
                    case CellaTipus.Sebesseg_Kartya: break;
                    case CellaTipus.Lab_Kartya: break;
                    case CellaTipus.Kesztyu_Kartya: break;
                }
            }
            return true;
        }

        static void uzi_szoras(String Uzi)
        {
            foreach (Jatekos j in Jatekosok.Values.ToList())
                j.Uzisor.Enqueue(Uzi);
        }




        static void jatekos_szal(Object param)
        {

            Jatekos j = (Jatekos)param;
            try
            {

                using (BinaryWriter bw = new BinaryWriter(j.tcp.GetStream()))
                {
                    using (BinaryReader br = new BinaryReader(j.tcp.GetStream()))
                    {
                        bool Bemutatkozott = false;

                        while (true)
                        {
                            if (j.tcp.Available > 0)
                            {
                                int uzi_tipus = br.Read();
                                switch ((Jatekos_Uzi_Tipusok)uzi_tipus)
                                {
                                    case Jatekos_Uzi_Tipusok.Bemutatkozik:
                                        j.Nev = br.ReadString();
                                        int r = br.ReadByte();
                                        int g = br.ReadByte();
                                        int b = br.ReadByte();
                                        j.Szin = System.Drawing.Color.FromArgb(r, g, b);
                                        break;
                                    case Jatekos_Uzi_Tipusok.Lep_Fel:
                                        if (!Bemutatkozott)
                                            break;
                                        if (jatekos_lep(j.x, j.y - 1, j))
                                            j.y -= 1;

                                        break;
                                    case Jatekos_Uzi_Tipusok.Lep_Jobbra:
                                        if (!Bemutatkozott)
                                            break;
                                        if (jatekos_lep(j.x, j.y - 1, j))
                                            j.x += 1;
                                        break;
                                    case Jatekos_Uzi_Tipusok.Lep_Le:
                                        if (!Bemutatkozott)
                                            break;

                                        if (jatekos_lep(j.x, j.y - 1, j))
                                            j.y += 1;
                                        break;
                                    case Jatekos_Uzi_Tipusok.Lep_Balra:
                                        if (!Bemutatkozott)
                                            break;

                                        if (jatekos_lep(j.x, j.y - 1, j))
                                            j.x -= 1;
                                        break;
                                    case Jatekos_Uzi_Tipusok.Bombat_rak:
                                        if (!Bemutatkozott)
                                            break;
                                        bomba_telepites(j.ID, j.x, j.y);
                                        break;
                                    case Jatekos_Uzi_Tipusok.Chat:
                                        String uzike = br.ReadString();
                                        String s = String.Format("{0}({1}):{2}", j.Nev, j.ID, uzike);
                                        Console.WriteLine(s);
                                        uzi_szoras(s);
                                        break;
                                }
                            }
                            String uzi;

                            if (j.Uzisor.TryDequeue(out uzi))
                            {
                                bw.Write((byte)Server_Uzi_Tipusok.Chat);
                                bw.Write(uzi);
                                bw.Flush();

                            }
                            bw.Write((byte)Server_Uzi_Tipusok.Jatekosok_Pozicioja);


                            foreach (Jatekos jj in Jatekosok.Values.ToList()) 
                            {
                                bw.Write(jj.ID);
                                bw.Write(jj.Nev);
                                bw.Write(jj.Szin.R);
                                bw.Write(jj.Szin.G);
                                bw.Write(jj.Szin.B);
                                bw.Write(jj.x);
                                bw.Write(jj.y);
                            }

                            bw.Write((uint)0);

                            bw.Flush();

                            bw.Write((byte)Server_Uzi_Tipusok.Palyakep);

                            bw.Write(palya_szelesseg);
                            bw.Write(palya_magassag);

                            byte[] t = new byte[palya_szelesseg * palya_magassag];

                            for (int y = 0, tidx=0; y < palya_magassag; y++)
                                for (int x = 0; x < palya_szelesseg; x++)
                                    t[tidx++] = (byte)Palya[x, y].Tipus;

                            bw.Write(t);
                            bw.Flush();

                            System.Threading.Thread.Sleep(20);

                        }
                    }
                }
            }
            catch
            { }
            finally
            {
                Jatekosok.Remove(j.ID);
            }
        }
    }
}
