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
        private NewAbility AddShove()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Shove";
            info.rulebookDescription = "When a creature bearing this sigil is played, it will move up to the space opposing it. Creatures that are in the way will be pushed in the same direction. Can't push things that are too heavy (like the Pack Mule and the Moon)";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "You have such fickle allies.";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            NewAbility ability = new NewAbility(info, typeof(Garethmod_Shove), generateTex("garethmod_shove"));
            Garethmod_Shove.ability = ability.ability;
            return ability;
        }
    }

    public class Garethmod_Shove : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            if (base.Card.slot.IsPlayerSlot)
            {
                bool isntCuttable = true;
                CardSlot mySlot = base.Card.Slot;
                yield return base.PreSuccessfulTriggerSequence();
                yield return new WaitForSeconds(0.25f);
                if (mySlot.opposingSlot.Card != null)
                {
                    if (!base.Card.Slot.opposingSlot.Card.Info.traits.Contains(Trait.Uncuttable))
                    {
                        yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(mySlot.opposingSlot.Card, 0.25f);
                    }
                    else
                    {
                        isntCuttable = false;
                    }
                }
                if (isntCuttable)
                {
                    mySlot.Card.SetIsOpponentCard(true);
                    mySlot.Card.transform.eulerAngles += new Vector3(0f, 0f, -180f);
                    CardSlot cardSlot = mySlot;
                    yield return Singleton<BoardManager>.Instance.AssignCardToSlot(cardSlot.Card, cardSlot.opposingSlot, 0.25f, null, true);
                    yield return new WaitForSeconds(0.4f);
                    yield return base.LearnAbility(0.5f);
                }
            }
            yield break;
        }

    }
}