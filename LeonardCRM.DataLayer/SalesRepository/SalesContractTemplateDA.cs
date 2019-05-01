using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Linq;
using System.Data;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesContractTemplateDA : EF5RepositoryBase<LeonardUSAEntities, SalesContractTemplate>
    {
        private static volatile SalesContractTemplateDA _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesContractTemplateDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new SalesContractTemplateDA();
                }
                return _instance;
            }
        }
        private SalesContractTemplateDA() : base(Settings.ConnectionString) { }

        public SalesContractTemplate GetContractTmplById(int templateId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {                
                if (templateId > 0)
                {
                    var entity = context.SalesContractTemplates.Include("SalesContractStates").First(x => x.Id == templateId);
                    if (entity != null)
                    {
                        entity.StateIds = entity.SalesContractStates.Select(x => x.StateId).ToArray();
                        entity.SalesContractStates = null;
                        entity.UsedStates = context.SalesContractStates.Where(x => x.ContractId != templateId).Select(x => x.StateId).Distinct().ToArray();
                    }
                    return entity;
                }
                else
                {                    
                    return new SalesContractTemplate()
                    {
                        StateIds = new int[]{},
                        UsedStates = context.SalesContractStates.Select(x => x.StateId).Distinct().ToArray()
                    };
                }
            }
        }

        public int GetContractTmplById(SalesContractTemplate contractTemplate)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var contractId = contractTemplate.Id;
                var newStateIds = contractTemplate.StateIds;

                if (contractId > 0)
                {
                    var relatedStates = context.SalesContractStates.Where(x => x.ContractId == contractId).ToList();
                    var relatedStateIds = relatedStates.Select(x => x.StateId).ToArray();
                    var deletedStates = relatedStates.Where(x => !contractTemplate.StateIds.Contains(x.StateId)).ToList();
                    var updatedStates = relatedStates.Where(x => contractTemplate.StateIds.Contains(x.StateId)).ToList();
                    newStateIds = newStateIds.Except(relatedStateIds).ToArray();

                    if (deletedStates != null && deletedStates.Any())
                    {
                        foreach (var state in deletedStates)
                        {
                            context.Entry(state).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }

                    if (updatedStates != null && updatedStates.Any())
                    {
                        foreach (var state in updatedStates)
                        {
                            context.Entry(state).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }

                if (newStateIds != null && newStateIds.Any())
                {
                    foreach (var i in newStateIds)
                    {
                        var saleState = new SalesContractState()
                        {
                            Id = 0,
                            ContractId = contractId,
                            StateId = i
                        };
                        context.Entry(saleState).State = System.Data.Entity.EntityState.Added;
                    }
                }

                context.Entry(contractTemplate).State = contractTemplate.Id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;

                return context.SaveChanges();
            }
        }
    }
}
