using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Pfe.Xrm.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to endpoint: {0}", SamplesConfig.CrmDiscoveryHost);
            Console.WriteLine("Connecting to {0} organization. [Discovery={1}]", SamplesConfig.CrmOrganization, SamplesConfig.CrmShouldDiscover);
            Console.WriteLine("Connecting as {0}", SamplesConfig.CrmUsername);
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
