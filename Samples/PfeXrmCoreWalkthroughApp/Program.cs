/*================================================================================================================================

  This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.  

  THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
  INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  

  We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object 
  code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software 
  product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the 
  Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims 
  or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

 =================================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Text;

namespace Microsoft.Pfe.Xrm.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(10, 10); //Increase min threads available on-demand
            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            
            Console.WriteLine("Target endpoint: {0}", SamplesConfig.CrmDiscoveryHost);
            Console.WriteLine("Target organization: [Discovery={1}]", SamplesConfig.CrmOrganizationHost, SamplesConfig.CrmShouldDiscover);
            Console.WriteLine("Authenticate as: {0}", SamplesConfig.CrmUsername);
            Console.WriteLine("Ready to execute CRM operations scenarios");

            do
            {               
                Console.WriteLine("Enter the ## of requests for each scenario operation:");

                int requestCount = 0;

                if (Int32.TryParse(Console.ReadLine(), out requestCount))
                {
                    Console.WriteLine("{0} requests will be generated for each scenario operation", requestCount);
                    Console.WriteLine("Press any <key> to execute SequentialOperationsScenario, or (Esc) to skip");

                    if (Console.ReadKey().Key != ConsoleKey.Escape)
                    {
                        Console.WriteLine("Executing Scenario 1: SequentialOperationsScenario");

                        //The sequential operations scenario
                        var seqScenario = new SequentialOperationsScenario(requestCount);
                        seqScenario.Execute();
                    }
                    else
                    {
                        Console.WriteLine("Skipping Scenario 1: SequentialOperationsScenario");
                    }

                    Console.WriteLine("Press any <key> to execute ParallelOperationsScenario, or (Esc) to skip");

                    if (Console.ReadKey().Key != ConsoleKey.Escape)
                    {
                        Console.WriteLine("Executing Scenario 2: ParallelOperationsScenario");

                        //The parallelized operations scenario using TPL
                        var parallelScenario = new ParallelOperationsScenario(requestCount);
                        parallelScenario.Execute();
                    }
                    else
                    {
                        Console.WriteLine("Skipping Scenario 2: ParallelOperationsScenario");
                    }

                    Console.WriteLine("Press any <key> to execute XrmCoreOperationsScenario, or (Esc) to skip");

                    if (Console.ReadKey().Key != ConsoleKey.Escape)
                    {
                        Console.WriteLine("Executing Scenario 3: XrmCoreOperationsScenario");

                        //The Xrm.Core operations scenario
                        var coreScenario = new XrmCoreOperationsScenario(requestCount);
                        coreScenario.Execute();
                    }
                    else
                    {
                        Console.WriteLine("Skipping Scenario 3: XrmCoreOperationsScenario");
                    }
                }
                else
                {
                    Console.WriteLine("Entry could not be parsed to System.Int32");
                }

                Console.WriteLine("Press (Esc) to exit, or any other <key> to repeat the scenarios");

            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
