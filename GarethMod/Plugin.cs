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

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public partial class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "gareth48.inscryption.garethmod";
        private const string PluginName = "Gareth's Mod";
        private const string PluginVersion = "1.8.0.0";
        private const string env = "GarethMod.dll";

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            RegisterModElements();
            //Harmony harmony = new Harmony(PluginGuid);
            //harmony.PatchAll();
        }


        private void RegisterModElements()
        {
            //Pre sigil card block
            List<Texture> watermark = new() { generateTex("garethmodwatermark") };
            watermark.ElementAt(0).filterMode = FilterMode.Point;

            NewCard.Add("Gareth48", "Sorry, can't do that", 1, 1, new List<CardMetaCategory>(), CardComplexity.Simple, CardTemple.Nature, description: "Whats... he doing here?", bloodCost: 0, abilities: new List<Ability>() { }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_gareth48), decals: watermark, emissionTex: loadTex(Artwork.garethmod_gareth48_emission)); //Texture: generateTex("gareth48"), decals: watermark

            //Sigil registration
            AddDrawAlly();
            AddTastyMorsel();
            AddIdentityTheft();
            AddShove();
            AddStandoffish();
            AddHungry();
            AddFlighty();

            //Post sigil registration block
            NewCard.Add("Garethmod_WoundedAnimal", "Wounded Animal", 0, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "An easy target for hungry predators.", bonesCost: 3, abilities: new List<Ability> { Garethmod_TastyMorsel.ability }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.AlternatingBloodDecal }, defaultTex: loadTex(Artwork.garethmod_woundedanimal), decals: watermark, emissionTex: loadTex(Artwork.garethmod_woundedanimal_emission));
            NewCard.Add("Garethmod_Skinwalker", "Skinwalker", 2, 1, new List<CardMetaCategory>() { CardMetaCategory.Rare }, CardComplexity.Simple, CardTemple.Nature, description: "The enigmatic skinwalker. It mimics the identity of the first creature it slays.", bloodCost: 2, tribes: new List<Tribe> { Tribe.Canine }, abilities: new List<Ability> { Garethmod_IdentityTheft.ability }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_skinwalker), decals: watermark, emissionTex: loadTex(Artwork.garethmod_skinwalker_emission));
            NewCard.Add("Garethmod_Lemming", "Lemming", 1, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The paranoid lemming. Fragile and a favorite treat among many animals.", bloodCost: 2, abilities: new List<Ability> { Garethmod_TastyMorsel.ability, Ability.Brittle }, defaultTex: loadTex(Artwork.garethmod_lemming), decals: watermark, emissionTex: loadTex(Artwork.garethmod_lemming_emission));
            NewCard.Add("Garethmod_Oxpecker", "Oxpecker", 1, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The adored Oxpecker. A bird capable of befriending even the fiercest of foes.", bloodCost: 1, tribes: new List<Tribe> { Tribe.Bird }, abilities: new List<Ability> { Garethmod_DrawAlly.ability, Ability.Flying }, defaultTex: loadTex(Artwork.garethmod_oxpecker), decals: watermark, emissionTex: loadTex(Artwork.garethmod_oxpecker_emission));
            NewCard.Add("Garethmod_Elephant", "Elephant", 2, 8, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The sturdy elephant. Friendly but not weak.", bloodCost: 3, abilities: new List<Ability> { Garethmod_TastyMorsel.ability, Ability.WhackAMole }, defaultTex: loadTex(Artwork.garethmod_elephant), decals: watermark, emissionTex: loadTex(Artwork.garethmod_elephant_emission));
            NewCard.Add("Garethmod_Hyena", "Hyena", 1, 2, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Intermediate, CardTemple.Nature, description: "The cackling hyena. It always has an ally lurking in the shadows.", bonesCost: 3, tribes: new List<Tribe> { Tribe.Canine }, abilities: new List<Ability> { Garethmod_DrawAlly.ability }, defaultTex: loadTex(Artwork.garethmod_hyena), decals: watermark, emissionTex: loadTex(Artwork.garethmod_hyena_emission));
            NewCard.Add("Garethmod_Treant", "Treant", 1, 9, new List<CardMetaCategory>() { }, CardComplexity.Simple, CardTemple.Nature, description: "", bloodCost: 3, abilities: new List<Ability> { Ability.WhackAMole, Ability.Sharp }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_treant), decals: watermark, emissionTex: loadTex(Artwork.garethmod_treant_emission));
            NewCard.Add("Garethmod_Snag", "Snag", 0, 5, new List<CardMetaCategory>() { }, CardComplexity.Simple, CardTemple.Nature, description: "", bloodCost: 2, abilities: new List<Ability> { Ability.Evolve, Ability.Sharp }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_snag), evolveId: new EvolveIdentifier("Garethmod_Treant", 1), decals: watermark, emissionTex: loadTex(Artwork.garethmod_snag_emission));
            NewCard.Add("Garethmod_Sapling", "Sapling", 0, 3, new List<CardMetaCategory>() { CardMetaCategory.Rare }, CardComplexity.Simple, CardTemple.Nature, description: "With time, even the smallest sapling becomes a massive, imposing obstacle.", bloodCost: 1, abilities: new List<Ability> { Ability.Evolve }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_sapling), evolveId: new EvolveIdentifier("Garethmod_Snag", 1), decals: watermark, emissionTex: loadTex(Artwork.garethmod_sapling_emission));
            NewCard.Add("Garethmod_Panther", "Panther", 2, 3, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The agressive panther. Never one to back down from a fight.", bloodCost: 2, abilities: new List<Ability> { Garethmod_Standoffish.ability }, defaultTex: loadTex(Artwork.garethmod_panther), decals: watermark, emissionTex: loadTex(Artwork.garethmod_panther_emission));
            NewCard.Add("Garethmod_PantherCub", "Panther Cub", 0, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The young panther. Even now it's caution is unwavering", bloodCost: 1, abilities: new List<Ability> { Garethmod_Standoffish.ability, Ability.Evolve }, defaultTex: loadTex(Artwork.garethmod_panthercub), evolveId: new EvolveIdentifier("Garethmod_Panther", 1), decals: watermark, emissionTex: loadTex(Artwork.garethmod_panthercub_emission));
            NewCard.Add("Garethmod_TasmanianDevil", "Tasmanian Devil", 1, 4, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The vicious tasmanian devil. A fierce carnivore with a voracious appetite.", bloodCost: 2, abilities: new List<Ability> { Garethmod_Standoffish.ability, Ability.GuardDog }, defaultTex: loadTex(Artwork.garethmod_tasmaniandevil), decals: watermark, emissionTex: loadTex(Artwork.garethmod_tasmaniandevil_emission));
            NewCard.Add("Garethmod_Archerfish", "Archerfish", 2, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The archerfish, an excellent marksman known to shoot bugs out of the air with a blast of water.", bloodCost: 2, abilities: new List<Ability> { Ability.Submerge, Ability.Sniper }, defaultTex: loadTex(Artwork.garethmod_archerfish), decals: watermark, emissionTex: loadTex(Artwork.garethmod_archerfish_emission));
            NewCard.Add("Garethmod_HerculesBeetle", "Hercules Beetle", 0, 4, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The forceful hercules beetle. It can move creatures many times its size.", bloodCost: 1, tribes: new List<Tribe> { Tribe.Insect }, abilities: new List<Ability> { Garethmod_Shove.ability }, defaultTex: loadTex(Artwork.garethmod_herculesbeetle), decals: watermark, emissionTex: loadTex(Artwork.garethmod_herculesbeetle_emission));
            NewCard.Add("Garethmod_Golem", "Golem", 0, 1, new List<CardMetaCategory>() { CardMetaCategory.Rare }, CardComplexity.Simple, CardTemple.Nature, description: "An ancient golem. Strong, but fragile. The magic that binds it is old but powerful. It will reform after death.", bloodCost: 1, abilities: new List<Ability> { Garethmod_Shove.ability, Ability.DrawCopyOnDeath }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_golem), decals: watermark, emissionTex: loadTex(Artwork.garethmod_golem_emission));
            NewCard.Add("Garethmod_Lion", "Lion", 3, 5, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The awe-inspiring lion. Sure to lead your creatures to victory.", bloodCost: 3, abilities: new List<Ability> { Ability.BuffNeighbours }, defaultTex: loadTex(Artwork.garethmod_lion), decals: watermark, emissionTex: loadTex(Artwork.garethmod_lion_emission));
            NewCard.Add("Garethmod_YoungLion", "Juvenile Lion", 1, 3, new List<CardMetaCategory>() { }, CardComplexity.Simple, CardTemple.Nature, description: "", bloodCost: 2, abilities: new List<Ability> { Ability.Evolve }, defaultTex: loadTex(Artwork.garethmod_younglion), evolveId: new EvolveIdentifier("Garethmod_Lion", 1), decals: watermark, emissionTex: loadTex(Artwork.garethmod_younglion_emission));
            NewCard.Add("Garethmod_LionCub", "Lion Cub", 1, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The princley lion cub. Given time it will become the king of its pride.", bloodCost: 1, abilities: new List<Ability> { Ability.Evolve }, defaultTex: loadTex(Artwork.garethmod_lioncub), evolveId: new EvolveIdentifier("Garethmod_YoungLion", 1), decals: watermark, emissionTex: loadTex(Artwork.garethmod_lioncub_emission));
            NewCard.Add("Garethmod_Pig", "Pig", 1, 2, new List<CardMetaCategory>() { CardMetaCategory.Rare }, CardComplexity.Simple, CardTemple.Nature, description: "The ravenous pig. It's hunger is instatiable, it will eat and eat until nothing remains.", bloodCost: 1, tribes: new List<Tribe> { Tribe.Hooved }, abilities: new List<Ability> { Garethmod_Hungry.ability }, appearanceBehaviour: new List<CardAppearanceBehaviour.Appearance> { CardAppearanceBehaviour.Appearance.RareCardBackground }, defaultTex: loadTex(Artwork.garethmod_pig), altTex: loadTex(Artwork.garethmod_pig_alt), decals: watermark, emissionTex: loadTex(Artwork.garethmod_pig_emission));
            NewCard.Add("Garethmod_Piranha", "Piranha", 1, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The ferocious piranha. Schools can devour any living thing in seconds.", bonesCost: 3, abilities: new List<Ability> { Garethmod_Hungry.ability, Ability.Submerge }, defaultTex: loadTex(Artwork.garethmod_piranha), decals: watermark, emissionTex: loadTex(Artwork.garethmod_piranha_emission));
            NewCard.Add("Garethmod_Leopard", "Leopard", 2, 3, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The stealthy leopard. Invisible to its prey until it's too late.", bloodCost: 2, abilities: new List<Ability> { Garethmod_Hungry.ability }, defaultTex: loadTex(Artwork.garethmod_leopard), decals: watermark, emissionTex: loadTex(Artwork.garethmod_leopard_emission));
            NewCard.Add("Garethmod_KillerBees", "Killer Bee Swarm", 1, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "", bloodCost: 1, tribes: new List<Tribe> { Tribe.Insect }, abilities: new List<Ability> { Garethmod_Hungry.ability, Ability.BeesOnHit }, defaultTex: loadTex(Artwork.garethmod_killerbees), decals: watermark, emissionTex: loadTex(Artwork.garethmod_killerbees_emission));
            NewCard.Add("Garethmod_Badger", "Badger", 2, 3, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The stealthy leopard. Invisible to its prey until it's too late.", bloodCost: 0, abilities: new List<Ability> { Garethmod_Flighty.ability }, defaultTex: loadTex(Artwork.garethmod_leopard), decals: watermark, emissionTex: loadTex(Artwork.garethmod_leopard_emission));
            NewCard.Add("Garethmod_Lynx", "Lynx", 1, 2, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "The stealthy leopard. Invisible to its prey until it's too late.", bloodCost: 1, abilities: new List<Ability> { Garethmod_Standoffish.ability,Garethmod_Flighty.ability }, defaultTex: loadTex(Artwork.garethmod_leopard), decals: watermark, emissionTex: loadTex(Artwork.garethmod_leopard_emission));
            NewCard.Add("Garethmod_Dragonfly", "Dragonfly", 3, 1, new List<CardMetaCategory>() { CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer }, CardComplexity.Simple, CardTemple.Nature, description: "", bonesCost: 6, tribes: new List<Tribe> { Tribe.Insect }, abilities: new List<Ability> { Ability.Flying, Garethmod_Flighty.ability }, defaultTex: loadTex(Artwork.garethmod_leopard), decals: watermark, emissionTex: loadTex(Artwork.garethmod_leopard_emission));
        }

        private Texture2D generateTex(String fileName)
        {
            Texture2D tex = new(2, 2);
            tex.LoadImage(System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace(env, ""), "Artwork/" + fileName + ".png")));
            return tex;
        }

        private Texture2D loadTex(byte[] resourceFile)
        {
            Texture2D tex = new(2, 2);
            tex.LoadImage(resourceFile);
            tex.filterMode = FilterMode.Point;
            return tex;
        }
    }
}
