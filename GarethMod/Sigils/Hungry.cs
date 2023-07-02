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
        private void AddHungry()
        {
            AbilityInfo hungry = AbilityManager.New(
                PluginGuid + ".hungry",
                "Hungry",
                "When a creature bearing this sigil is played, choose another friendly creature to eat. The eaten creature will have its health and power added to the creature bearing this sigil.",
                typeof(Garethmod_Hungry),
                "garethmod_hungry.png"
            );
            hungry.powerLevel = 2;
            hungry.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "It seems eating another creature has caused yours to grow stronger."
            };
            lines.Add(line);
            hungry.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_Hungry.ability = hungry.ability;
        }
        /*
        private NewAbility AddHungry()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Hungry";
            info.rulebookDescription = "When a creature bearing this sigil is played choose another friendly creature to eat. The eaten creature will have its health and power added to the creature bearing this sigil.";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "It seems eating another creature has caused yours to grow stronger.";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            NewAbility ability = new NewAbility(info, typeof(Garethmod_Hungry), loadTex(Artwork.garethmod_hungry));
            Garethmod_Hungry.ability = ability.ability;
            return ability;
        }
        */
    }

        public class Garethmod_Hungry : AbilityBehaviour
        {
        public override Ability Ability => ability;

        public static Ability ability;

            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            public override IEnumerator OnResolveOnBoard()
            {
                //An unfotunate caveat I'm devloping to get around not wanting to learn how special behaviors work will be refractored later.
                if (base.Card.Info.name.Equals("Garethmod_Pig"))
                {
                    //Debug.Log("Pig");
                    List<CardSlot> validPigTargets = Singleton<BoardManager>.Instance.AllSlotsCopy;
                    validPigTargets.RemoveAll((CardSlot x) => x.Card == null || x.Card.Dead || x.Card == base.Card || x.Card.OpponentCard);

                    int healthAdd = 0;
                    int attackAdd = 0;

                    if (validPigTargets.Count > 0)
                    {
                        foreach (CardSlot s in validPigTargets)
                        {
                            healthAdd += s.Card.Health - s.Card.GetPassiveHealthBuffs();
                            attackAdd += s.Card.Attack - s.Card.GetPassiveAttackBuffs();
                            yield return s.Card.Die(false, null, true);
                            yield return new WaitForSeconds(0.1f);
                        }

                        CardModificationInfo pigMod = new CardModificationInfo();
                        pigMod.nonCopyable = true;
                        pigMod.fromCardMerge = true;
                        pigMod.singletonId = "carnage";
                        pigMod.attackAdjustment = attackAdd;
                        pigMod.healthAdjustment = healthAdd;
                        base.Card.AddTemporaryMod(pigMod);
                        yield return base.Card.Anim.FlipInAir();
                        base.Card.SwitchToAlternatePortrait();
                        yield return new WaitForSeconds(0.75f);
                        yield return base.LearnAbility(0f);
                    }
                }
                else
                {

                    List<CardSlot> validTargets = Singleton<BoardManager>.Instance.AllSlotsCopy;
                    validTargets.RemoveAll((CardSlot x) => x.Card == null || x.Card.Dead || x.Card == base.Card || x.Card.OpponentCard || x.Card.Info.traits.Contains(Trait.Terrain) || x.Card.Info.traits.Contains(Trait.Pelt));
                    if (validTargets.Count > 0)
                    {
                        CardSlot selectedSlot = null;
                        List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                        allSlotsCopy.Remove(base.Card.Slot);
                        yield return Singleton<BoardManager>.Instance.ChooseTarget(allSlotsCopy, validTargets, delegate (CardSlot s)
                        {
                            selectedSlot = s;
                        }, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate (CardSlot s)
                        {
                            if (s.Card != null)
                            {
                                //Debug.Log("found card");
                            }
                        }, null, CursorType.Target);
                        {
                            CardModificationInfo mod = new CardModificationInfo();
                            mod.nonCopyable = true;
                            mod.singletonId = "food";
                            mod.attackAdjustment = selectedSlot.Card.Attack - selectedSlot.Card.GetPassiveAttackBuffs();
                            mod.healthAdjustment = selectedSlot.Card.Health - selectedSlot.Card.GetPassiveHealthBuffs();
                            base.Card.AddTemporaryMod(mod);
                            yield return selectedSlot.Card.Die(false, null, true);
                            yield return new WaitForSeconds(0.75f);
                            yield return base.LearnAbility(0f);
                        }
                    }
                }
                yield break;
                //Singleton<BoardManager>.Instance.CreateCardInSlot(CardToSpawn, slot, 0.15f, true);
            }
            //AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), "zombievirus"))
            protected virtual void OnInvalidTargetSelected(CardSlot targetSlot)
            {
            Debug.Log("Something has gone very very wrong");
            }
        }
    }
