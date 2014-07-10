using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Microsoft.Pfe.Xrm.Samples
{
    public class Requests
    {
        public Requests()
        {
            this.RequestCount = 10;
        }
        public Requests(int requestCount)
        {
            this.RequestCount = requestCount;
        }

        public  int RequestCount { get; set; }

        #region Seed data for Step #1: Create accounts

        /// <summary>
        /// List of accounts to create
        /// </summary>
        private List<Entity> accountCreateTargets;
        public List<Entity> AccountCreateTargets
        {
            get
            {
                if (this.accountCreateTargets == null)
                {
                    this.accountCreateTargets = new List<Entity>();

                    for (var i = 0; i < this.RequestCount; i++)
                    {
                        var account = new Entity("account");
                        account["name"] = String.Format("Created account #{0}", i + 1);
                        account["address1_line1"] = "1234 Highway 6, Suite 200";
                        account["address1_city"] = "Redmond";
                        account["address1_stateorprovince"] = "WA";
                        account["address1_country"] = "United States";

                        this.accountCreateTargets.Add(account);
                    }
                }

                return this.accountCreateTargets;
            }
        } 

        #endregion

        #region Seed data for Step #2: Create related contacts and opportunities

        /// <summary>
        /// List of contacts to create for each created account
        /// </summary>
        private List<Entity> contactCreateTargets;
        public List<Entity> ContactCreateTargets
        {
            get
            {
                if (this.contactCreateTargets == null)
                {
                    this.contactCreateTargets = new List<Entity>();

                    this.AccountCreateTargets.ForEach(a =>
                    {
                        var contact = new Entity("contact");
                        contact["firstname"] = "Created contact";
                        contact["lastname"] = a.GetAttributeValue<string>("name").Substring(15);
                        contact["parentcustomerid"] = a.ToEntityReference();
                        contact["address1_line1"] = a.GetAttributeValue<string>("address1_line1");
                        contact["address1_city"] = a.GetAttributeValue<string>("address1_city");
                        contact["address1_stateorprovince"] = a.GetAttributeValue<string>("address1_stateorprovince");
                        contact["address1_country"] = a.GetAttributeValue<string>("address1_country");

                        this.contactCreateTargets.Add(contact);
                    });
                }

                return this.contactCreateTargets;
            }
        }

        /// <summary>
        /// List of opportunities to create for each created account
        /// </summary>
        private List<Entity> opportunityCreateTargets;
        public List<Entity> OpportunityCreateTargets
        {
            get
            {
                if (this.opportunityCreateTargets == null)
                {
                    this.opportunityCreateTargets = new List<Entity>();

                    this.AccountCreateTargets.ForEach(a =>
                    {
                        var opportunity = new Entity("opportunity");
                        opportunity["customerid"] = a.ToEntityReference();
                        opportunity["name"] = String.Format("Created Opportunity for {0}", a.GetAttributeValue<string>("name"));

                        this.opportunityCreateTargets.Add(opportunity);
                    });
                }

                return this.opportunityCreateTargets;
            }
        } 

        #endregion

        #region Seed data for Step #3: Update accounts and opportunities

        /// <summary>
        /// List of accounts to update with their associated contact as primarycontact
        /// </summary>
        private List<Entity> accountUpdateTargets;
        public List<Entity> AccountUpdateTargets
        {
            get
            {
                if (this.accountUpdateTargets == null)
                {
                    this.accountUpdateTargets = new List<Entity>();

                    this.AccountCreateTargets.ForEach(a =>
                    {
                        var target = new Entity(a.LogicalName) { Id = a.Id };

                        var contact = this.ContactCreateTargets.FirstOrDefault(c => c.GetAttributeValue<EntityReference>("parentcustomerid") != null
                                                                                    && c.GetAttributeValue<EntityReference>("parentcustomerid").Id == a.Id);

                        if (contact != null)
                            target["primarycontactid"] = contact.ToEntityReference();

                        this.accountUpdateTargets.Add(target);
                    });
                }

                return this.accountUpdateTargets;
            }
        }

        /// <summary>
        /// List of opportunities to update estimated close and revenue
        /// </summary>
        private List<Entity> opportunityUpdateTargets;
        public List<Entity> OpportunityUpdateTargets
        {
            get
            {
                if (this.opportunityUpdateTargets == null)
                {
                    this.opportunityUpdateTargets = new List<Entity>();

                    var rndRev = new Random();
                    var rndDays = new Random();

                    this.OpportunityCreateTargets.ForEach(o =>
                    {
                        var opportunity = new Entity("opportunity") { Id = o.Id };
                        opportunity["estimatedvalue"] = new Money(rndRev.Next(1000000));
                        opportunity["estimatedclosedate"] = DateTime.Now.AddDays(rndDays.Next(90));

                        this.opportunityUpdateTargets.Add(opportunity);
                    });
                }

                return this.opportunityUpdateTargets;
            }
        } 

        #endregion

        #region Seed data for Step #4: Execute close opportunity and deactivate contact requests

        /// <summary>
        /// List of all opportunities to close as won
        /// </summary>
        private List<WinOpportunityRequest> winRequests;
        public List<WinOpportunityRequest> WinRequests
        {
            get
            {
                if (this.winRequests == null)
                {
                    this.winRequests = new List<WinOpportunityRequest>();

                    this.OpportunityCreateTargets.ForEach(o =>
                        {
                            var closeActivity = new Entity("opportunityclose");
                            closeActivity["opportunityid"] = o.ToEntityReference();

                            var request = new WinOpportunityRequest()
                            {
                                OpportunityClose = closeActivity,
                                Status = new OptionSetValue(3)
                            };

                            this.winRequests.Add(request);
                        });
                }

                return this.winRequests;
            }
        }

        /// <summary>
        /// List of deactivation requests for all created Contacts
        /// </summary>
        private List<SetStateRequest> setStateRequests;
        public List<SetStateRequest> SetStateRequests
        {
            get
            {
                if (this.setStateRequests == null)
                {
                    this.setStateRequests = new List<SetStateRequest>();
                    
                    this.ContactCreateTargets.ForEach(c =>
                        {
                            var request = new SetStateRequest()
                            {
                                EntityMoniker = c.ToEntityReference(),
                                State = new OptionSetValue(1),
                                Status = new OptionSetValue(2)
                            };

                            this.setStateRequests.Add(request);
                        });
                }

                return this.setStateRequests;
            }
        } 

        #endregion

        #region  Seed data for Step #5: Retrieve multiple entities

        /// <summary>
        /// List of three queries to execute, 2 QueryExpression and 1 FetchExpression
        /// </summary>
        public List<QueryBase> RetrieveMultipleQueries
        {
            get
            {
                var queries = new List<QueryBase>();

                var accQuery = new QueryExpression("account");
                accQuery.ColumnSet.AddColumns("name", "address1_city", "primarycontactid");
                accQuery.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.BeginsWith, "Created"));
                queries.Add(accQuery);

                var cntQuery = new QueryExpression("contact");
                cntQuery.ColumnSet.AddColumns("firstname", "lastname", "parentcustomerid");
                cntQuery.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 1));
                queries.Add(cntQuery);

                var oppQuery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                            <entity name='opportunity'>
                                                <attribute name='name' />
                                                <attribute name='estimatedvalue' />
                                                <attribute name='customerid' />
                                                <filter type='and'>
                                                    <condition attribute='estimatedvalue' operator='gt' value='1000' />
                                                </filter>
                                            </entity> 
                                            </fetch>";
                queries.Add(new FetchExpression(oppQuery));

                return queries;
            }
        }

        #endregion

        #region Seed data for Step #6: Cleanup

        /// <summary>
        /// List of all accounts as entityreferences targeted for deletion
        /// </summary>
        /// <remarks>
        /// Will cascade deletes to contacts and opportunities based on relationship references
        /// </remarks>
        public List<EntityReference> DeleteTargets
        {
            get
            {
                var targets = new List<EntityReference>();
                targets.AddRange(this.AccountCreateTargets.Select(a => a.ToEntityReference()).ToList()); //Will cascade deletes to contacts and opportunities

                return targets;
            }
        } 

        #endregion
    }
}
