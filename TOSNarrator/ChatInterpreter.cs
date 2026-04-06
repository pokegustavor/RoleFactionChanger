using Game.Interface;
using HarmonyLib;
using Server.Shared.Utils;
using System.Collections.Generic;
using System;
using Server.Shared.Extensions;
using SimpleSpritePacker;

namespace TOSFactionColor
{
    class Utilities 
    {
        internal static string ProcessChat(string text)
        {
            string backup = text;
            if (text.Contains("[[") && text.Contains("]]"))
            {

                Dictionary<int, int> start_endPositions = new Dictionary<int, int>();
                int innerCount = 0;
                int start = -1;
                string factionID;
                for (int i = 0; i < text.Length; i++)
                {
                    if (i < text.Length - 1 && text[i] == '[' && text[i + 1] == '[')
                    {
                        if (innerCount == 0)
                        {
                            start = i;
                        }
                        innerCount++;
                    }
                    else if (i < text.Length - 1 && text[i] == ']' && text[i + 1] == ']')
                    {
                        innerCount--;
                        if (innerCount == 0 && start != -1)
                        {
                            start_endPositions.Add(start, i + 1);
                            start = -1;
                        }
                    }
                    if (innerCount < 0) innerCount = 0;
                }
                if (start_endPositions.Count > 0)
                {
                    string temp = text;
                    foreach (KeyValuePair<int, int> encap in start_endPositions)
                    {
                        // [[#1]]([[#45]])
                        string original = text.Substring(encap.Key, encap.Value - encap.Key + 1);
                        string replacement = original.Substring(2, original.Length - 4);
                        if (replacement.Contains("[[") && replacement.Contains("]]")) //Method 1  [[ [[#1]] , [[#106]] ]]
                        {
                            // #1, [[#106]] 
                            replacement = replacement.Remove(replacement.IndexOf("[["), 2);
                            replacement = replacement.Remove(replacement.IndexOf("]]"), 2);
                            if ((!replacement.Contains("[[") || !replacement.Contains("]]")) && ((!replacement.Contains("(") || !replacement.Contains(")")))) continue;
                            // #1, #106 
                            if (replacement.Contains("[["))
                            {
                                replacement = replacement.Remove(replacement.IndexOf("[["), 2);
                                replacement = replacement.Remove(replacement.IndexOf("]]"), 2);
                            }
                            else
                            {
                                replacement = replacement.Remove(replacement.IndexOf("("), 1);
                                replacement = replacement.Remove(replacement.IndexOf(")"), 1);
                            }
                            // {#1} {#106}
                            string[] split = replacement.Split(',');
                            if (split.Length == 1)
                            {
                                split = replacement.Split('.');
                            }
                            if (split.Length == 1) continue;
                            factionID = ConvertToFactionID(split[1].Trim());
                            replacement = $"[[{split[0].Trim()},{ConvertToFactionID(split[1].Trim())}]]";
                            temp = temp.Replace(original, replacement);
                            continue;
                        }
                        original = text.Substring(encap.Key).Replace(" ", string.Empty);
                        if (encap.Value - encap.Key + 2 < original.Length && original[encap.Value - encap.Key + 1] == '(') //Method 2 [[#1]]([[#106]])
                        {
                            replacement = text.Substring(encap.Key);
                            int parEnd = replacement.IndexOf(")");
                            if (parEnd == -1) continue;
                            //[[#1]]([[#106]])
                            replacement = replacement.Substring(0, parEnd + 1);
                            original = replacement;
                            //{[[#1]]} {[[#106]])}
                            string[] split = replacement.Split('(');
                            if (split.Length == 1) continue;
                            //{[[#1]]} {[[#106]]}
                            split[1] = split[1].Remove(split[1].IndexOf(")"), 1);
                            //{#1} {#106}
                            if (split[1].Contains("[[") && split[1].Contains("]]"))
                            {
                                split[1] = split[1].Remove(split[1].IndexOf("[["), 2);
                                split[1] = split[1].Remove(split[1].IndexOf("]]"), 2);
                            }
                            split[0] = split[0].Remove(split[0].IndexOf("[["), 2);
                            split[0] = split[0].Remove(split[0].IndexOf("]]"), 2);
                            if (!split[0].Contains("#")) continue;
                            factionID = ConvertToFactionID(split[1].Trim());
                            if (factionID != "-1")
                            {
                                replacement = $"[[{split[0].Trim()},{ConvertToFactionID(split[1].Trim())}]]";
                                temp = temp.Replace(original, replacement);
                            }
                        }
                    }
                    return temp;
                }

            }
            return backup;
        }

        static string ConvertToFactionID(string role)
        {
            switch (role.ToLower())
            {
                default: //None
                    return "-1";
                case "#100": //Town
                case "#101":
                case "#102":
                case "#103":
                case "#104":
                case "#105":
                case "#116":
                    return "1";
                case "#106": //Coven
                case "#107":
                case "#108":
                case "#109":
                case "#110":
                case "#117":
                    return "2";
                case "#48": //SK
                    return "3";
                case "#40": //Arsonist
                    return "4";
                case "#51": //WW
                    return "5";
                case "#49": //Shroud
                    return "6";
                case "#114":
                case "#41":
                case "#42":
                case "#47":
                case "#50":
                case "#115":
                    return "7"; //NA
                case "#44":
                case "#111":
                case "#112":
                case "#113":
                    return "8"; //Executioner
                case "#45":
                    return "9"; //Jester
                case "#46":
                    return "10"; //Pirate
                case "#43":
                    return "11"; //Doom
                case "#52":
                    return "12"; //Vampire
                case "#53":
                    return "13"; //CS

                //BTOS Roles
                case "#55": //Jackal
                    return "33";
                case "blue":// Blue team
                case "frog":
                    return "34";
                case "yellow": //Yellow team
                case "lion":
                    return "35";
                case "hawk": //Red team
                case "red":
                    return "36";
                case "lol": //IDK
                    return "37";
                case "#57": //Judge
                    return "38";
                case "#58": //Auditor
                    return "39";
                case "#60": //Starspawn
                    return "41";
                case "#59": //Inquisitor
                    return "40";
                case "ego": //Ego
                case "egotist":
                    return "42";
                case "pand": //Pandora
                case "pandora":
                case "pan":
                case ":330":
                    return "43";
                case "comp": //Compliance
                case "compliance":
                case "com":
                case ":326":
                    return "44";
                
                

            }
        }
    }

    [HarmonyPatch(typeof(ChatInputController))]
    class ChatControllerPatch
    {
        [HarmonyPatch("SubmitChat")]
        [HarmonyPrefix]
        public static void PrefixToChat(ChatInputController __instance)
        {
            // I hate [[ [[#1]] , [[#106]] ]] that think they are [[ [[#5]], [[#45]] ]]
            string text = __instance.chatInput.mentionPanel.mentionsProvider.EncodeText(__instance.chatInput.text.ResolveUnicodeSequences());
            string backup = text;
            try
            {
                __instance.chatInput.text = Utilities.ProcessChat(text);
            }
            catch (Exception ex)
            {
                __instance.PlaySound("Audio/UI/Error.wav", false);
                __instance.chatInput.text = backup;
                Console.WriteLine($"(FACTIONCHANGER)Error! changes reverted. Error message: {ex.Message}, attempeted to change message {backup}");
            }
        }
        

    }
    [HarmonyPatch(typeof(DeathNotePanel))]
    class DeathNotePatch 
    {
        [HarmonyPatch("SaveDeathNote")]
        [HarmonyPrefix]
        public static void PrefixToDeathNote(DeathNotePanel __instance)
        {
            // I hate [[ [[#1]] , [[#106]] ]] that think they are [[ [[#5]], [[#45]] ]]
            string text = __instance.mentionsPanel.mentionsProvider.EncodeText(__instance.inputField.text.ResolveUnicodeSequences());
            string backup = text;
            try
            {
                __instance.inputField.text = Utilities.ProcessChat(text);
            }
            catch (Exception ex)
            {
                __instance.PlaySound("Audio/UI/Error.wav", false);
                __instance.inputField.text = backup;
                Console.WriteLine($"(FACTIONCHANGER)Error! changes reverted. Error message: {ex.Message}, attempeted to change message {backup}");
            }
        }
    }
    [HarmonyPatch(typeof(LastWillPanel))]
    class LastWillPatch
    {
        [HarmonyPatch("SaveWill")]
        [HarmonyPrefix]
        public static void PrefixToDeathNote(LastWillPanel __instance)
        {
            // I hate [[ [[#1]] , [[#106]] ]] that think they are [[ [[#5]], [[#45]] ]]
            string text = __instance.mentionsPanel.mentionsProvider.EncodeText(__instance.inputField.text.ResolveUnicodeSequences());
            string backup = text;
            try
            {
                __instance.inputField.text = Utilities.ProcessChat(text);
            }
            catch (Exception ex)
            {
                __instance.PlaySound("Audio/UI/Error.wav", false);
                __instance.inputField.text = backup;
                Console.WriteLine($"(FACTIONCHANGER)Error! changes reverted. Error message: {ex.Message}, attempeted to change message {backup}");
            }
        }
    }
}
