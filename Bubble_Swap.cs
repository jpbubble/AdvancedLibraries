// Lic:
// Bubble_Swap.cs
// Swap linkup code
// version: 20.07.19
// Copyright (C) 2019 Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic

using System;
using TrickyUnits;

namespace Bubble {

    class Bubble_Swap {

        private Bubble_Swap() { }

        string statename = "";
        BubbleState State = null;
        NLua.Lua LuaState = null;
        static public void Init(string statename) {
            var me = new Bubble_Swap();
            try {
                var script = QuickStream.StringFromEmbed("Bubble_Swap.nil");
                me.statename = statename;
                me.State = SBubble.State(statename);
                me.LuaState = me.State.state;
                me.LuaState["Bubble_Swap"] = me;
                SBubble.DoNIL(statename, script, "BUBBLE_Swap Init");
            } catch (Exception DoffeEllende) {
                SBubble.MyError("Init BUBBLE_Swap Error", DoffeEllende.Message, $"{SBubble.TraceLua(statename)}\n\n{DoffeEllende.StackTrace}");
            }
        }


        static Swap SwapMap = null;
        static string SwapFolderSet = "";

        public void SetSwapFolder(string s) {
            if (SwapMap != null) SBubble.MyError("Swap error", "The first swap call has already been done, and the folder cannot be changed after that anymore!", SBubble.TraceLua(statename));
            else SwapFolderSet = s;
        }

        static void InitSwap() {
            if (SwapMap == null) {
                if (SwapFolderSet == "") SwapFolderSet = "Swap";
                var ADir = $"{Bubble_Save.SWorkDir}/{SwapFolderSet}";
                BubConsole.WriteLine($"Swap system initialized to folder: {ADir}");
                SwapMap = new Swap(ADir);
            }
        }

        public void SetValue(string k, string v) { InitSwap(); SwapMap[k] = v; }
        public string GetValue(string k) {
            try {
                InitSwap(); return SwapMap[k];
            } catch (Exception E) {
                SBubble.MyError($"Swap.Data[\"{k}\"]:", E.Message, SBubble.TraceLua(statename));
                return "ERROR";
            }
        }
        public void AppValue(string k, string v) { InitSwap(); SwapMap.App(k, v); }
        public int Length { get { InitSwap(); return SwapMap.Keys.Length; } }
        public void Kill(string k, string v) { InitSwap(); SwapMap.Kill(k); }
        public void Clear() { InitSwap(); SwapMap.Clear(); }
        public static void StaticClear() { InitSwap(); SwapMap.Clear(); } // Only added to make sure it all works from the start!
        public string LuaKeys() { InitSwap(); return SwapMap.KeysLua; }

        public static void SwapSave(UseJCR6.TJCRCreate j, string dir) {
            foreach (string k in SwapMap.Keys) {
                j.AddString(SwapMap[k], $"XTRA/{dir}/{k}", "lzma", "Swap File", "Used for swapping data");
            }
        }

        public static bool SwapLoad(UseJCR6.TJCRDIR j, string dir) {
            try {
                //if (SwapMap == null) SwapMap = new Swap("Swap");
                InitSwap();
                SwapMap.Clear();
                foreach (string k in j.Entries.Keys) {
                    if (qstr.ExtractDir(k) == $"XTRA/{dir.ToUpper()}") {
                        var key = qstr.StripDir(k);
                        SwapMap[key] = j.LoadString(k);
                    }
                }
                return true;
            } catch (Exception EpicFail) {

                SBubble.MyError("Error while extracting swap data", EpicFail.Message,
#if DEBUG
                    EpicFail.StackTrace);
#else
                    "");
#endif
                return false;
            }
        }
    }
}