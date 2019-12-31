using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace Infrastructure.Store
{
    public interface IRulesStore
    {
        List<CRule> FindAllRules();

        CRule FindRuleById(Guid ruleId);

        void DeleteRule(Guid ruleId);

        void UpdateRule(CRule rule);

        void InsertRule(CRule rule);
    }
}
