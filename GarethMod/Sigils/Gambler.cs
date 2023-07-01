using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DiskCardGame;
using UnityEngine;
using APIPlugin;
using Artwork = GarethMod.GarethmodResources;

namespace GarethMod
{
    public partial class Plugin
    {
        private NewAbility AddGambler()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Gambling";
            info.rulebookDescription = "When a creature bearing this sigil is played 6 stats will be randomly allocated to it's attack and health. They will re-randomize every turn.";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "1-800-522-4700... The gambling addiction help line.";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            NewAbility ability = new NewAbility(info, typeof(Garethmod_Gambler), generateTex("garethmod_gambler"));
            Garethmod_Gambler.ability = ability.ability;
            return ability;
        }
    }

    public class Garethmod_Gambler : AbilityBehaviour
    {
        public override Ability Ability => ability;
        private void Start()
        {
            this.gamblingMod = new CardModificationInfo();
            this.gamblingMod.nonCopyable = true;
            this.gamblingMod.singletonId = "gambling";
            base.Card.AddTemporaryMod(this.gamblingMod);

            this.saviorMod = new CardModificationInfo();
            this.saviorMod.nonCopyable = true;
            this.saviorMod.singletonId = "savior";
            base.Card.AddTemporaryMod(this.saviorMod);
        }

        public static Ability ability;
        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep;
        }
        public override IEnumerator OnResolveOnBoard()
        {
            yield return base.PreSuccessfulTriggerSequence();
            this.randomizeStats();
            yield return base.LearnAbility(0.5f);
            yield break;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return base.PreSuccessfulTriggerSequence();
            this.randomizeStats();
            yield break;
        }

        private void randomizeStats()
        {
            int h = base.Card.Health-base.Card.GetPassiveHealthBuffs()-1;
            int a = base.Card.Attack-base.Card.GetPassiveAttackBuffs();
            int max = h + a;

            int randOne = SeededRandom.Range(0, max + 1, base.GetRandomSeed());
            int randTwo = max - randOne;
            this.gamblingMod.healthAdjustment = this.gamblingMod.healthAdjustment + randOne - h;
            this.gamblingMod.attackAdjustment = this.gamblingMod.attackAdjustment + randTwo - a;
            base.Card.OnStatsChanged();
            base.Card.Anim.StrongNegationEffect();


        }


        private CardModificationInfo gamblingMod;
        private CardModificationInfo saviorMod;

    }
}