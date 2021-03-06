using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using POEApi.Model;

namespace Procurement.ViewModel.Filters
{
    public abstract class StatFilter : StatFilterBase, IFilter
    {
        public abstract FilterGroup Group { get; }

        public StatFilter(string keyword, string help, params string[] stats)
            : base(keyword, help, stats)
        { }

        public bool Applicable(POEApi.Model.Item item)
        {
            Gear gear = item as Gear;
            if (gear == null)
                return false;

            List<Regex> pool = new List<Regex>(stats);
            List<string> all = new List<string>();

            if (gear.Implicitmods != null)
                all.AddRange(gear.Implicitmods.Select(s => s));

            if (gear.Explicitmods != null)
                all.AddRange(gear.Explicitmods.Select(s => s));

            foreach (string stat in all)
            {
                Regex result = pool.Find(s => s.IsMatch(stat));
                pool.Remove(result);
            }

            return pool.Count == 0;
        }
   }
}