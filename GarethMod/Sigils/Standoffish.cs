using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;

namespace GarethMod
{
    public partial class Plugin
    {
        private void AddStandoffish()
        {
            //Settled on a new format at this point
            AbilityInfo standoffish = AbilityManager.New(
                PluginGuid + ".standoffish",
                "Standoffish",
                "As long as a creature bearing this sigil is opposed by another, it has +2 attack.",
                typeof(Garethmod_Standoffish),
                "garethmod_standoffish.png"
            );
            standoffish.powerLevel = 3;
            standoffish.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "Your creature will fight with all it has until none oppose it."
            };
            lines.Add(line);
            standoffish.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_Standoffish.ability = standoffish.ability;
        }
    }

    public class Garethmod_Standoffish : AbilityBehaviour
    {
        public override Ability Ability => ability;
        private void Start()
        {
            this.mod = new CardModificationInfo();
            this.mod.nonCopyable = true;
            this.mod.singletonId = "enraged";
            this.mod.attackAdjustment = 0;
            base.Card.AddTemporaryMod(this.mod);
        }

        public static Ability ability;


        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
        {
            return RespondsToTrigger(otherCard);
        }

        public bool RespondsToTrigger(PlayableCard otherCard)
        {
            return !base.Card.Dead && !otherCard.Dead && otherCard.Slot == base.Card.Slot.opposingSlot;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return true;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            int current = this.mod.attackAdjustment;
            if (base.Card.Slot.opposingSlot.Card != null)
            {
                yield return base.PreSuccessfulTriggerSequence();
                this.mod.attackAdjustment = 2;
                base.Card.OnStatsChanged();
                yield return base.LearnAbility(0.5f);
                //TODO: Learn powerup
            }
            else
            {
                yield return base.PreSuccessfulTriggerSequence();
                this.mod.attackAdjustment = 0;
                base.Card.OnStatsChanged();
                //TODO: Learn powerdown
            }

            if (this.mod.attackAdjustment != current)
            {
                base.Card.Anim.StrongNegationEffect();
            }

            yield break;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            if (base.Card.Slot.opposingSlot.Card != null)
            {
                yield return base.PreSuccessfulTriggerSequence();
                this.mod.attackAdjustment = 2;
                base.Card.OnStatsChanged();
                base.Card.Anim.StrongNegationEffect();
                yield return base.LearnAbility(0.5f);
            }
            yield break;
        }

        public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
        {
            /*
            if (otherCard.HasAbility(Pusher.ability))
            {
                if (otherCard.slot.opposingSlot.Card != null && otherCard.slot.opposingSlot.Card.Info.traits.Contains(Trait.Uncuttable))
                {

                }
            }
            */
            if (base.Card.Slot.opposingSlot.Card != null)
            {
                yield return base.PreSuccessfulTriggerSequence();
                this.mod.attackAdjustment = 2;
                base.Card.OnStatsChanged();
                base.Card.Anim.StrongNegationEffect();
                yield return base.LearnAbility(0.5f);
            }
            yield break;
        }


        private CardModificationInfo mod;

    }

}