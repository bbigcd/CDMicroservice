using Polly;
using Polly.Timeout;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pollytest1
{
    class Program
    {
        static void Main(string[] args)
        {
            /* 普通用法
            try
            {
               //捕获指定的异常
                Policy policy = Policy.Handle<ArgumentException>().Fallback(() =>
                {
                    Console.WriteLine("出错了");
                }, ex =>
                {
                    Console.WriteLine("详细异常对象" + ex);
                });

                policy.Execute(() =>
                {
                    Console.WriteLine("开始执行");
                    throw new ArgumentException();
                    Console.WriteLine("执行结束");
                });

            }
            catch
            {
                Console.WriteLine("发生未处理的异常");
            }
            */
            /*
            // 带返回值的情况
            Policy<string> policy = Policy<string>.Handle<Exception>(ex => ex.Message == "数据错误")
                .Fallback(() =>
                {
                    Console.WriteLine("fallback");
                    return "降级后给你的值";
                });

            string result = policy.Execute(() =>
            {
                Console.WriteLine("正常执行");
                throw new Exception("Hello world!");
                Console.WriteLine("执行结束");
                return "正常值";
            });

            Console.WriteLine(result);
            */

            /* 重试机制
            try
            {
                Policy policy = Policy
           .Handle<Exception>()
              .RetryForever();//连环夺命call
              // .Retry(3);
              //.WaitAndRetry(100,i=>TimeSpan.FromSeconds(i));
              //.WaitAndRetry(10000, i => TimeSpan.FromMilliseconds(100));
                policy.Execute(() => {
                    Console.WriteLine("开始任务");
                    if (DateTime.Now.Second % 10 != 0)
                    {
                        throw new Exception("出错");
                    }
                    Console.WriteLine("完成任务");
                });
            }
            catch
            {
                Console.WriteLine("出现未捕获异常");
            }
            */

            /*
            // 重试后熔断
            Policy policy = Policy
           .Handle<Exception>()
           .CircuitBreaker(3, TimeSpan.FromSeconds(5));//连续出错6次之后熔断5秒(不会再去尝试执行业务代码）。

            while (true)
            {
                Console.WriteLine("开始Execute");
                try
                {
                    policy.Execute(() => {
                        Console.WriteLine("开始任务");
                        throw new Exception("出错");
                        Console.WriteLine("完成任务");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("execute出错" + ex);
                }
                Thread.Sleep(500);
            }
            */

            /* 
            // 策略封装
            Policy policyRetry = Policy.Handle<Exception>()
                .Retry(3);
            Policy policyFallback = Policy.Handle<Exception>()
                .Fallback(() => {
                    Console.WriteLine("降级");
                });
            //Wrap：包裹。policyRetry在里面，policyFallback在外面。
            //如果里面出现了故障，则把故障抛出来给外面
            Policy policy = policyFallback.Wrap(policyRetry);
            policy.Execute(() => {
                Console.WriteLine("开始任务");
                if (DateTime.Now.Second % 10 != 0)
                {
                    throw new Exception("出错");
                }
                Console.WriteLine("完成任务");
            });
            */

            /* 超时策略
            Policy policytimeout = Policy.Timeout(3, TimeoutStrategy.Pessimistic);
            Policy policyFallBack = Policy.Handle<TimeoutRejectedException>()
               .Fallback(() => {
                   Console.WriteLine("降级");
               });
            Policy policy = policyFallBack.Wrap(policytimeout);
            policy.Execute(()=> {
                Console.WriteLine("开始任务");
                Thread.Sleep(5000);
                Console.WriteLine("完成任务");
            });
            */

            Test1().Wait();

            Console.ReadKey();
        }

        /// <summary>
        /// 异步方式
        /// </summary>
        /// <returns></returns>
        static async Task Test1()
        {
            /*
            Policy<byte[]> policy = Policy<byte[]>
            .Handle<Exception>()
            .FallbackAsync(async c => {
                Console.WriteLine("降级");
                return new byte[0];
            }, async r => {
                Console.WriteLine(r.Exception);
            });

            //异步策略封装
            policy = policy.WrapAsync(Policy.TimeoutAsync(2, TimeoutStrategy.Pessimistic, async (context, timespan, task) =>
            {
                Console.WriteLine("timeout");
            }));
            var bytes = await policy.ExecuteAsync(async () => {
                Console.WriteLine("开始任务");
                HttpClient httpClient = new HttpClient();
                var result = await httpClient.GetByteArrayAsync("http://static.rupeng.com/upload/chatimage/20183/07EB793A4C247A654B31B4D14EC64BCA.png");
                Console.WriteLine("完成任务");
                return result;
            });
            Console.WriteLine("bytes长度" + bytes.Length);
            */

            Policy policy = Policy
            .Handle<Exception>()
            .FallbackAsync(async c => {
                Console.WriteLine("执行出错");
            }, async ex => {//对于没有返回值的，这个参数直接是异常
                Console.WriteLine(ex);
            });

            policy = policy.WrapAsync(Policy.TimeoutAsync(3, TimeoutStrategy.Pessimistic, async (context, timespan, task) =>
            {
                Console.WriteLine("timeout");
            }));
            await policy.ExecuteAsync(async () => {
                Console.WriteLine("开始任务");
                await Task.Delay(5000);//注意不能用Thread.Sleep(5000);
                Console.WriteLine("完成任务");
            });
        }

    }
}
