using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using InscryptionAPI.Card;

namespace GarethMod
{
    public partial class Plugin
    {
        //This is now a void function, no need to return a pre-made ability

        private void AddIdentityTheft()
        {
            //I cant tell if I'm sold on enter per line for large definitions yet
            //the readability goes up but the pain in my scrolling finger does also
            AbilityInfo identitytheft = AbilityManager.New(
                PluginGuid + ".identitytheft",
                "Essence Steal",
                "When a creature bearing this sigil kills another, it transforms into an exact copy of it.",
                typeof(Garethmod_IdentityTheft),
                "garethmod_identitytheft.png"
                );
            identitytheft.powerLevel = 3;
            identitytheft.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "Your creature has slain mine, and will now become an imitation of it."
            };
            lines.Add(line);
            identitytheft.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_IdentityTheft.ability = identitytheft.ability;
        }
        /*
        private NewAbility AddIdentityTheft()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Essence Steal";
            info.rulebookDescription = "When a creature bearing this sigil kills another, it transforms into an exact copy of it.";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };


            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "Your creature has slain mine, and will now become an imitation of it.";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);


            NewAbility ability = new NewAbility(info, typeof(Garethmod_IdentityTheft), loadTex(Artwork.garethmod_identitytheft));
            Garethmod_IdentityTheft.ability = ability.ability;
            return ability;
        }
        */
    }

    public class Garethmod_IdentityTheft : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            return amount > 0;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {

            if (target.Health == 0)
            {
                //Turn into Gareth for weird interactions -- Giant added since there are now multiple uncuttable doesnt necessarily disclude them
                if (target.Info.traits.Contains(Trait.Uncuttable)|| target.Info.traits.Contains(Trait.Giant))
                {
                    yield return base.Card.TransformIntoCard(CardLoader.GetCardByName("Garethmod_Gareth48"));
                    base.Card.HealDamage(base.Card.Status.damageTaken);
                    yield break;
                }
                //Turn into normal card. Final check is to counter a bug involving thorns.
                if (this.Card.Health > 0)
                {
                    yield return base.PreSuccessfulTriggerSequence();
                    yield return new WaitForSeconds(0.5f);
                    yield return base.LearnAbility(0.5f);
                    CardInfo steal = target.Info;
                    yield return base.Card.TransformIntoCard(steal);
                    base.Card.HealDamage(base.Card.Status.damageTaken);
                }
                yield break;
            }
        }

    }

}