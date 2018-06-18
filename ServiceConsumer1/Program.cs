using Consul;
using System;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace ServiceConsumer1
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                using (var consul = new ConsulClient(c =>
                {
                    c.Address = new Uri("http://127.0.0.1:8500");
                }))
                {
                    var services = consul.Agent.Services().Result.Response
                        .Values.Where(item => item.Service.Equals("MsgService", StringComparison.OrdinalIgnoreCase));
                    /*
                    foreach(var s in services.Values)
                    {
                        Console.WriteLine($"id={s.ID},service={s.Service},addr={s.Address},port={s.Port}");
                    }*/
                    //客户端负载均衡
                    Random rand = new Random();
                    int index = rand.Next(services.Count());//[0,services.Count())
                    var s = services.ElementAt(index);
                    Console.WriteLine($"index={index},id={s.ID},service={s.Service},addr={s.Address},port={s.Port}");

                    using (HttpClient http = new HttpClient())
                    using (var httpContent = new StringContent("{phoneNum:'119',msg:'help'}",
                            Encoding.UTF8, "application/json"))
                    {
                        var r = http.PostAsync($"http://{s.Address}:{s.Port}/api/SMS/Send_LX", httpContent).Result;
                        Console.WriteLine(r.StatusCode);
                    }
                }

            }
            Console.ReadKey();
        }
    }
}
