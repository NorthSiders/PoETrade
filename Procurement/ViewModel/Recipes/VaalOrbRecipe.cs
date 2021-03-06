using System;
using System.Collections.Generic;
using System.Linq;
using POEApi.Model;

namespace Procurement.ViewModel.Recipes
{
    public class VaalOrbRecipe : Recipe
    {
        private const int NeededVaalGems = 7;
        private const decimal TotalNumberOfNeededItems = 8m;
        public override string Name => "1 Vaal Orb";

        public override IEnumerable<RecipeResult> Matches(IEnumerable<Item> items)
        {
            List<RecipeResult> recipeSets = new List<RecipeResult>();

            var candidateGems = items.OfType<Gem>().Where(
                gem => gem.Corrupted && gem.TypeLine.StartsWith("Vaal ", StringComparison.CurrentCultureIgnoreCase))
                .Cast<Item>().ToList();

            var fragments = items.Where(
                x => x.TypeLine.StartsWith("Sacrifice at", StringComparison.CurrentCultureIgnoreCase)).ToList();

            while(candidateGems.Count > 0)
            {
                var recipeResult = new RecipeResult()
                {
                    Instance = this,
                    MatchedItems = new List<Item>(),
                    Missing = new List<string>(),
                    IsMatch = true
                };

                for (int i = candidateGems.Count - 1; i >= 0; i--)
                {
                    var candidateGem = candidateGems[i];
                    if (recipeResult.MatchedItems.Count == NeededVaalGems)
                    {
                        break;
                    }

                    recipeResult.MatchedItems.Add(candidateGem);
                    candidateGems.Remove(candidateGem);
                }

                var numberOfMissingGems = NeededVaalGems - recipeResult.MatchedItems.Count;

                if (numberOfMissingGems == 1)
                {
                    recipeResult.Missing.Add("1 Vaal Skill gem");
                }
                else if (numberOfMissingGems >= 2)
                {
                    recipeResult.Missing.Add($"{numberOfMissingGems} Vaal Skill gems");
                }

                if (fragments.Any())
                {
                    recipeResult.MatchedItems.Add(fragments.First());
                    fragments.RemoveAt(0);
                }
                else
                {
                    recipeResult.Missing.Add("Sacrifice Fragment");
                }

                recipeResult.PercentMatch = recipeResult.MatchedItems.Count / TotalNumberOfNeededItems * 100 ;
                recipeSets.Add(recipeResult);
            }

            return recipeSets;
        }
    }
}