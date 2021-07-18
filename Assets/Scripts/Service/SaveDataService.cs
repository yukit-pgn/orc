using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Main.Data;

namespace Main.Service
{
    public static class SaveDataService
    {
        static readonly string DECK_LIST = "DeckList";

        /// <summary>
        /// デッキリストのセーブデータがあるか
        /// </summary>
        public static bool HasDeckList
        {
            get { return false; }
        }

        /// <summary>
        /// デッキリストをロード
        /// </summary>
        public static List<DeckData> LoadDeckList()
        {
            return null;
        }

        /// <summary>
        /// デッキリストをセーブ
        /// </summary>
        public static void SaveDeckList(List<DeckData> deckList)
        {
            // ES3.Save<List<DeckData>>(DECK_LIST, deckList);
        }
    }
}
