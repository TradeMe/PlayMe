using System;
using Ninject;

namespace PlayMe.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            var service = kernel.Get<MusicWindowsService>();
            if (Environment.UserInteractive)
            {                
                service.Start(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.Stop();
            }
            else
            {         
                System.ServiceProcess.ServiceBase.Run(service);
            }
        }
    }
}
