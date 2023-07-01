using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DiskCardGame;
using UnityEngine;
using APIPlugin;
using InscryptionAPI.Card;

namespace GarethMod
{
    public partial class Plugin
    {
        private void AddDrawAlly()
        {
            //Settled on a new format at this point
            AbilityInfo drawally = AbilityManager.New(
                PluginGuid + ".drawally",
                "Allies",
                "When a creature bearing this sigil dies, a random ally will temporarily join your hand to avenge it.",
                typeof(Garethmod_DrawAlly),
                "garethmod_drawally.png"
            );
            drawally.powerLevel = 2;
            drawally.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "It seems a creature has come to avenge the passing of its ally."
            };
            lines.Add(line);
            drawally.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_IdentityTheft.ability = drawally.ability;
        }

    }

    /*
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Allies";
            info.rulebookDescription = "When a creature bearing this sigil dies, a random ally will temporarily join your hand to avenge it.";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "It seems a creature has come to avenge the passing of its ally.";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            NewAbility ability = new NewAbility(info, typeof(Garethmod_DrawAlly), loadTex(Artwork.garethmod_drawally));
            Garethmod_DrawAlly.ability = ability.ability;
            return ability;
     */

    public class Garethmod_DrawAlly : DrawCreatedCard
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }

        public override CardInfo CardToDraw
        {
            get
            {
                List<CardInfo> list = ScriptableObjectLoader<CardInfo>.AllData.FindAll((CardInfo x) => (x.metaCategories.Contains(CardMetaCategory.ChoiceNode) && x.temple.Equals(CardTemple.Nature) && !x.name.Equals("!STATIC!GLITCH")));
                list.Add(CardLoader.GetCardByName("Squirrel"));
                return list[SeededRandom.Range(0, list.Count, base.GetRandomSeed())];
            }
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return base.CreateDrawnCard();
            yield return base.LearnAbility(0.5f);
            yield break;
        }

    }
}