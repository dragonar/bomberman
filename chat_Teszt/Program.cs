using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Bomberman.KozosKod;
using System.IO;

namespace chat_Teszt
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient tcpc = new TcpClient();
            tcpc.Connect("10.0.1.166", 60000);

            using (BinaryWriter bw = new BinaryWriter(tcpc.GetStream()))
            {
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
                        if (Console.KeyAvailable)
                        {
                            String s = Console.ReadLine();

                            bw.Write((byte)Jatekos_Uzi_Tipusok.Chat);
                            bw.Write(s);
                            bw.Flush();
                        }
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
                                    break;
                            }
                        }
                    }
                }

            }
        }
    }
}
