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
using InscryptionAPI.Card;

namespace GarethMod
{
    public partial class Plugin
    {
        //First of many
        private void AddTastyMorsel()
        {
            //Settled on a new format at this point
            AbilityInfo tastymorsel = AbilityManager.New(
                PluginGuid + ".tastymorsel",
                "Tasty",
                "When a card bearing this sigil dies, a random creature shows up to devour it.",
                typeof(Garethmod_TastyMorsel),
                "garethmod_tastymorsel.png"
            );
            tastymorsel.powerLevel = 3;
            tastymorsel.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "It seems that another animal has come to feed. The cycle of life continues."
            };
            lines.Add(line);
            tastymorsel.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_IdentityTheft.ability = tastymorsel.ability;
        }
    }

    public class Garethmod_TastyMorsel : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return !wasSacrifice && base.Card.OnBoard;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return new WaitForSeconds(0.3f);
            List<String> randomCards = new() { "Adder", "Alpha", "Shark", "Bat", "Bee", "Bloodhound", "Bullfrog", "Cat", "Cockroach", "Coyote", "Elk", "ElkCub", "FieldMouse", "Grizzly", "Kingfisher", "Maggots", "Magpie", "Mantis", "Mole", "Moose", "Porcupine", "Pronghorn", "RatKing", "Rattler", "Raven", "Skink", "Skunk", "Snapper", "Squirrel", "Vulture", "Wolf", "WolfCub" };
            name = randomCards[SeededRandom.Range(0, randomCards.Count, base.GetRandomSeed())];
            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(name), base.Card.Slot, 0.15f, true);
            yield return base.LearnAbility(0.5f);
            yield break;

        }
    }

}