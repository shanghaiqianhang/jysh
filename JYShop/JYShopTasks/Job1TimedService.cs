using JYShop.Common.Helper;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using JYShop.IServices;

namespace JYShopTasks
{
    public class Job1TimedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IShopInfoService _shoinfoService;
        public Job1TimedService(IShopInfoService shoinfoService)
        {
            _shoinfoService = shoinfoService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is starting.");
            _timer = new Timer(doWork,null,TimeSpan.Zero,TimeSpan.FromSeconds(60*60));//1个小时

            return Task.CompletedTask;

        }

        private async void doWork(object state)
        {
            //try
            //{
            //    var model =await _blogArticleServices.GetBlogDetails(5);
            //    Console.WriteLine($"BlogArticle:{model.bCreateTime}");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error:{ex.Message}");
            //}

            ConsoleHelper.WriteSuccessLine($"Job 1： {DateTime.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Job 1 is stopping.");
            _timer?.Change(Timeout.Infinite,0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
