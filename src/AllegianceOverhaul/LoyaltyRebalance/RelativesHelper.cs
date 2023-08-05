using AllegianceOverhaul.Extensions;

using System.Linq;

using TaleWorlds.CampaignSystem;

namespace AllegianceOverhaul.LoyaltyRebalance
{
    class RelativesHelper
    {
        //TODO: make new section with relatives in Hero Encyclopedia page; add several categories of relatives, grouping them by how distant they are in the genealogical tree and giving each category it's own weight.
        public static bool BloodRelatives(Hero queriedHero, Hero baseHero)
        {
            return
              //Parrents, siblings, nephews and nieces, children
              baseHero.Father == queriedHero || baseHero.Mother == queriedHero || baseHero.GetAllSiblings().Contains(queriedHero) || baseHero.GetAllSiblings().Exists(h => h.Children.Contains(queriedHero)) || baseHero.Children.Contains(queriedHero)
              //GrandParrents, uncles and aunts
              || (baseHero.Father != null && (baseHero.Father.Father == queriedHero || baseHero.Father.Mother == queriedHero || baseHero.Father.GetAllSiblings().Contains(queriedHero)))
              || (baseHero.Mother != null && (baseHero.Mother.Father == queriedHero || baseHero.Mother.Mother == queriedHero || baseHero.Mother.GetAllSiblings().Contains(queriedHero)))
              //Spouses, parents-in-law, siblings-in-law children-in-law
              || baseHero.Spouse == queriedHero
              || (baseHero.Spouse != null && (baseHero.Spouse.Father == queriedHero || baseHero.Spouse.Mother == queriedHero
                                              || baseHero.Spouse.GetAllSiblings().Contains(queriedHero) || baseHero.GetAllSiblings().Exists(h => h.Spouse == queriedHero)
                                              || baseHero.Spouse.Children.Contains(queriedHero) || baseHero.Children.ToList().Exists(h => h.Spouse == queriedHero)
                                             ));
        }
        public static bool BloodRelatives(Clan? queriedClan, Clan? baseClan)
        {
            if (baseClan is null || queriedClan is null)
                return false;

            foreach (Hero baseHero in baseClan.Heroes)
            {
                foreach (Hero queriedHero in queriedClan.Heroes)
                {
                    if (BloodRelatives(queriedHero, baseHero))
                        return true;
                }
            }
            return false;
        }
    }
}