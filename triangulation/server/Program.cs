using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Server
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку

            IPAddress ipAddr = IPAddress.Any;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8080);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение");

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    string reply = null;

                    if (data == "true")
                    {
                        Random random = new Random();
                        int rnd = random.Next(0, 8);

                        switch (rnd)
                        {
                            case 0:
                                reply = "248,99," +
                                "59,437," +
                                "335,321," +
                                "83,767," +
                                "378,619," +
                                "287,1020," +
                                "634,1001," +
                                "658,417," +
                                "249,596," +
                                "613,233," +
                                "287,228," +
                                "622,58," +
                                "30,313";
                                break;
                            case 1:
                                reply = "758,168," +
                                "889,76," +
                                "631,26," +
                                "580,168," +
                                "686,135," +
                                "687,264," +
                                "451,260," +
                                "422,93," +
                                "544,361," +
                                "728,354," +
                                "895,227," +
                                "763,271," +
                                "759,194";
                                break;
                            case 2:
                                reply = "493,125," +
                                "437,268," +
                                "607,364," +
                                "613,247," +
                                "821,284," +
                                "696,359," +
                                "863,416," +
                                "948,182," +
                                "728,206," +
                                "854,117," +
                                "740,91," +
                                "637,192," +
                                "679,60";
                                break;
                            case 3:
                                reply = "659,155," +
                                "411,208," +
                                "458,372," +
                                "602,196," +
                                "684,389," +
                                "844,220," +
                                "754,171," +
                                "916,103," +
                                "836,104," +
                                "798,46," +
                                "793,107," +
                                "711,61," +
                                "689,114";
                                break;
                            case 4:
                                reply = "249,204," +
                                "169,325," +
                                "255,243," +
                                "274,264," +
                                "303,375," +
                                "314,264," +
                                "447,257," +
                                "465,368," +
                                "508,179," +
                                "596,151," +
                                "538,138," +
                                "494,90," +
                                "451,199";
                                break;
                            case 5:
                                reply = "442,156," +
                                "371,249," +
                                "415,311," +
                                "601,357," +
                                "742,363," +
                                "846,291," +
                                "862,137," +
                                "761,123," +
                                "664,185," +
                                "623,133," +
                                "566,222," +
                                "462,228," +
                                "555,132";
                                break;
                            case 6:
                                reply = "480,156," +
                                "516,241," +
                                "493,349," +
                                "662,409," +
                                "703,285," +
                                "829,349," +
                                "812,245," +
                                "939,221," +
                                "907,106," +
                                "797,189," +
                                "694,106," +
                                "597,252," +
                                "541,113";
                                break;
                            case 7:
                                reply = "507,22," +
                                "377,122," +
                                "513,168," +
                                "352,248," +
                                "390,300," +
                                "526,301," +
                                "536,203," +
                                "668,198," +
                                "634,101," +
                                "490,106";
                                break;
                            default:
                                break;
                        }
                    }
              
                    Console.Write("Ответ: " + reply + "\n\n");

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }

        }
    }
}
