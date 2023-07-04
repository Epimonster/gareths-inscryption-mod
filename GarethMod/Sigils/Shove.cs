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
        private void AddShove()
        {
            AbilityInfo shove = AbilityManager.New(
                PluginGuid + ".shove",
                "Shove",
                "When a creature bearing this sigil is played, it will move up to the space opposing it. Creatures that are in the way will be pushed in the same direction.", //Can't push things that are too heavy (like the Pack Mule and the Moon)
                typeof(Garethmod_Shove),
                "garethmod_shove.png"
            );
            //We never want leshy to get this on a totem he would just lose, same with you

            shove.powerLevel = -1;
            shove.opponentUsable = false;
            shove.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "You have such fickle allies."
            };
            lines.Add(line);
            shove.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_Shove.ability = shove.ability;
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