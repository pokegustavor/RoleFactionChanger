using Game.Interface;
using HarmonyLib;
using Server.Shared.Utils;
using System.Collections.Generic;
using System;

namespace TOSFactionColor
{
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
                if (text.Contains("[[") && text.Contains("]]"))
                {
                    Dictionary<int, int> start_endPositions = new Dictionary<int, int>();
                    int innerCount = 0;
                    int start = -1;
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
                            // 0[[#1]] , [[#106]] [ ]
                            string original = text.Substring(encap.Key, encap.Value - encap.Key + 1);
                            string replacement = original.Substring(2, original.Length - 4);
                            if (!replacement.Contains("[[") || !replacement.Contains("]]")) continue;
                            // #1, [[#106]] 
                            replacement = replacement.Remove(replacement.IndexOf("[["), 2);
                            replacement = replacement.Remove(replacement.IndexOf("]]"), 2);
                            if (!replacement.Contains("[[") || !replacement.Contains("]]")) continue;
                            // #1, #106 
                            replacement = replacement.Remove(replacement.IndexOf("[["), 2);
                            replacement = replacement.Remove(replacement.IndexOf("]]"), 2);
                            // {#1} {#106}
                            string[] split = replacement.Split(',');
                            if(split.Length == 1) 
                            {
                                split = replacement.Split('.');
                            }
                            if (split.Length == 1) continue;
                            replacement = $"[[{split[0].Trim()},{ConvertToFactionID(split[1].Trim())}]]";
                            temp = temp.Replace(original, replacement);
                        }
                        __instance.chatInput.text = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                __instance.chatInput.text = backup;
                Console.WriteLine($"(FACTIONCHANGER)Error! changes reverted. Error message: {ex.Message}, attempeted to change message {backup}");
            }
        }

        static string ConvertToFactionID(string role) 
        {
            switch (role) 
            {
                default:
                case "#100":
                case "#101":
                case "#102":
                case "#103":
                case "#104":
                case "#105":
                case "#116":
                    return "1";
                case "#106":
                case "#107":
                case "#108":
                case "#109":
                case "#110":
                case "#117":
                    return "2";
                case "#48":
                    return "3";
                case "#40":
                    return "4";
                case "#51":
                    return "5";
                case "#49":
                    return "6";
                case "#114":
                case "#41":
                case "#42":
                case "#47":
                case "#50":
                case "#115":
                    return "7";
                case "#44":
                case "#111":
                case "#112":
                case "#113":
                    return "8";
                case "#45":
                    return "9";
                case "#46":
                    return "10";
                case "#43":
                    return "11";
                case "#52":
                    return "12";
                case "#53":
                    return "13";
                case "#254":
                    return "14";

            }
        }
    }
}
