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
            get { return ES3.KeyExists(DECK_LIST); }
        }

        /// <summary>
        /// デッキリストをロード
        /// </summary>
        public static List<DeckData> LoadDeckList()
        {
            return ES3.Load<List<DeckData>>(DECK_LIST);
        }

        /// <summary>
        /// デッキリストをセーブ
        /// </summary>
        public static void SaveDeckList(List<DeckData> deckList)
        {
            ES3.Save<List<DeckData>>(DECK_LIST, deckList);
        }
    }
}
