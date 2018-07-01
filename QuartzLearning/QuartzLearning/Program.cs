using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace QuartzLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            RunProgram().GetAwaiter().GetResult();
            //TestNameValueCollection();
            Console.Read();
        }
        private static async Task RunProgram()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(5)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                // some sleep to show what's happening
                await Task.Delay(TimeSpan.FromSeconds(60));

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
            }
        }
        private static void TestNameValueCollection()
        {
            var myCol = new NameValueCollection();
            myCol.Add("red", "rojo");//如果键值red相同结果合并 rojo,rouge  
            myCol.Add("green", "verde");
            myCol.Add("blue", "azul");
            myCol.Add("red", "rouge");
            foreach (var item in myCol.AllKeys)
            {
                Console.WriteLine($"{item}:{myCol[item]}");
            }
        }
    }
}
